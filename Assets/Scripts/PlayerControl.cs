using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private Movement movement;
	[SerializeField] private float jumpHeight = 3;
	[SerializeField] private float jumpReleaseDamping = 0.35f;
	[SerializeField] private GameObject bulletPrefab;

	private SpritePhysics physics;
	private InputManager input;
	private bool facingRight = true;

	void Start() {
		input = InputManager.Instance;

		physics = GetComponent<SpritePhysics>();

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
			Vector3 vel = physics.vel;
			float v = vel.x;
			int dX = input.X.Dir;

			float accel = BaseAcceleration() * OffGroundMultiplier() * DirectionMultiplier();

			float dV;
			if (dX == 0) { // pressing no direction
				float s = -Mathf.Sign(v);
				float mag = Mathf.Abs(v);
				dV = s * Mathf.Min(releaseMultiplier * accel, mag);
			}
			else {
				dV = accel * dX;
			}
		
			vel.x = Mathf.Clamp(v + dV, -speed, speed);
			physics.vel = vel;
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
			bool isNotPressingSameDirection = input.X.Dir * physics.vel.x < 0;
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
		Vector3 vel = physics.vel;
		bool isFalling = vel.y * Physics2D.gravity.y > 0;
		if (input.Jump.DidPress && physics.IsOnGround) {
			vel.y = JumpSpeed();
		}
		else if (input.Jump.DidRelease && !isFalling) {
			vel.y *= jumpReleaseDamping;
		}
		physics.vel = vel;
	}

	private float JumpSpeed() {
		float g = Physics2D.gravity.y;
		return -Mathf.Sign(g) * Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(g));
	}

	private void UpdateShooting() {
		if (input.Shoot.DidPress) {
			GameObject bulletObj = GameObject.Instantiate(bulletPrefab);
			bulletObj.transform.position = transform.position;
			Bullet bullet = bulletObj.GetComponent<Bullet>();
			float bulletXDir = facingRight ? 1 : -1;
			bullet.Init(gameObject, new Vector3(bulletXDir, 0, 0));
		}
	}

	private void UpdateReset() {
		if (input.Reset.DidPress) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
