using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private Movement movement;
	[SerializeField] private float jumpHeight = 3;
	[SerializeField] private float jumpReleaseDamping = 0.35f;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject specialBulletPrefab;

	private InputManager input;
	private SpritePhysics physics;
	private PlayerMana mana;
	private bool facingRight = true;

	void Start() {
		input = InputManager.Instance;

		physics = GetComponent<SpritePhysics>();
		mana = GetComponent<PlayerMana>();

		movement.Init(physics, input);
	}

	void FixedUpdate() {
		movement.Update();
		UpdateFacing();
		UpdateJumping();
		UpdateShooting();
		UpdateReset();
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

			float accel = BaseAcceleration() * OffGroundMultiplier() * DirectionMultiplier();

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

		private float BaseAcceleration() {
			return speed / timeToMaxSpeed * Time.fixedDeltaTime;
		}

		private float OffGroundMultiplier() {
			if (!physics.IsOnGround) {
				return offGroundMultiplier;
			}
			return 1;
		}

		private float DirectionMultiplier() {
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
			facingRight = true;
		}
		else if (dir < 0) {
			facingRight = false;
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
		float xDir = facingRight ? 1 : -1;
		Vector2 dir = new Vector2(xDir, 0);
		Bullet.Create(prefab, gameObject, dir);
	}

	private void UpdateReset() {
		if (input.Reset.DidPress) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
