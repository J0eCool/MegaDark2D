using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : JComponent {
	[SerializeField] private Movement movement;
	[SerializeField] private float jumpHeight = 3;
	[SerializeField] private float jumpReleaseDamping = 0.35f;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject specialBulletPrefab;

	[SerializeField] private float debugTrailTime = -1.0f;

	[StartComponent] private SpritePhysics physics;
	[StartComponent] private Facing facing;
	[StartComponent] private PlayerMana mana;
    [StartComponent] private tk2dSpriteAnimator animator;

	private InputManager input;

	protected override void onStart() {
		input = InputManager.Instance;

		movement.Init(physics, input);
	}

	void FixedUpdate() {
		movement.Update();
		UpdateFacing();
		UpdateJumping();
		UpdateShooting();
		UpdateReset();
        UpdateAnimation();

		if (debugTrailTime > 0.0f) {
			Debug.DrawLine(transform.position,
				transform.position + (Vector3)physics.Vel * Time.fixedDeltaTime,
				Color.cyan,
				debugTrailTime);
		}
	}

	[System.Serializable]
	private class Movement : Updateable {
		[SerializeField] private float speed = 10;
		[SerializeField] private float timeToMaxSpeed = 0.25f;
		[SerializeField] private float oppositeDirectionMultiplier = 2.5f;
		[SerializeField] private float releaseMultiplier = 1.0f;
		[SerializeField] private float offGroundMultiplier = 0.35f;

		private SpritePhysics physics;
		private InputManager input;

		public void Init(SpritePhysics physics, InputManager input) {
			this.physics = physics;
			this.input = input;
		}

		public void Update() {
			Vector3 vel = physics.Vel;
			float v = vel.x;
			int dX = input.X.Dir;

			float accel = baseAcceleration() * accelerationMultiplier();

			float dV;
			bool isReleasingXDir = (dX == 0);
			if (isReleasingXDir) {
				float s = -Mathf.Sign(v);
				float mag = Mathf.Abs(v);
				dV = s * Mathf.Min(releaseMultiplier * accel, mag);
			}
			else {
				dV = accel * dX;
			}
		
			vel.x = Mathf.Clamp(v + dV, -speed, speed);
			physics.Vel = vel;
		}

		private float baseAcceleration() {
			return speed / timeToMaxSpeed * Time.fixedDeltaTime;
		}

		private float accelerationMultiplier() {
			if (!physics.IsOnGround) {
				return offGroundMultiplier;
			}

			bool isNotPressingSameDirection = input.X.Dir * physics.Vel.x < 0;
			if (isNotPressingSameDirection) {
				return oppositeDirectionMultiplier;
			}
			return 1;
		}
	}

	private void UpdateFacing() {
		var dir = input.X.Dir;
		if (dir > 0) {
			facing.FacingRight = true;
		}
		else if (dir < 0) {
			facing.FacingRight = false;
		}
	}

	private void UpdateJumping() {
		Vector3 vel = physics.Vel;
		bool isFalling = vel.y * Physics2D.gravity.y > 0;
		if (input.Jump.DidPress && physics.IsOnGround) {
			vel.y = JumpSpeed();
		}
		else if (input.Jump.DidRelease && !isFalling) {
			vel.y *= jumpReleaseDamping;
		}
		physics.Vel = vel;
	}

	private float JumpSpeed() {
		float g = Physics2D.gravity.y;
		return -Mathf.Sign(g) * Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(g));
	}

	private void UpdateShooting() {
		var shotPrefab = getShotBulletPrefab();
		if (shotPrefab != null) {
			tryShoot(shotPrefab);
		}
	}

	private GameObject getShotBulletPrefab() {
		if (input.Shoot.DidPress) {
			return bulletPrefab;
		}
		else if (input.Special.DidPress) {
			return specialBulletPrefab;
		}
		return null;
	}

	private void tryShoot(GameObject prefab) {
		Bullet bullet = prefab.GetComponent<Bullet>();
		if (mana.TrySpend(bullet.ManaCost)) {
			shoot(prefab);
		}
	}

	private void shoot(GameObject prefab) {
		Vector2 dir = new Vector2(facing.Dir, 0);
		Bullet.Create(prefab, gameObject, dir);
	}

	private void UpdateReset() {
		if (input.Reset.DidPress) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}

    private bool isFiringAnimationPlaying = false;
    private void UpdateAnimation() {
        if (input.Shoot.DidPress) {
            animator.Stop();
            animator.Play("Shoot");
            isFiringAnimationPlaying = true;
            animator.AnimationCompleted = (x, y) => {
                isFiringAnimationPlaying = false;
                animator.AnimationCompleted = null;
            };
        }

        if (isFiringAnimationPlaying) {
            return;
        }

        if (Mathf.Abs(physics.Vel.x) > 0.1f) {
            animator.Play("Run");
        } else {
            animator.Play("Idle");
        }
    }
}
