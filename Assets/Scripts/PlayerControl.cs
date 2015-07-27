using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private float _speed = 10;
	[SerializeField] private float _timeToMaxSpeed = 0.25f;
	[SerializeField] private float _moveOppositeMultiplier = 2.5f;
	[SerializeField] private float _moveReleaseMultiplier = 1.0f;
	[SerializeField] private float _moveOffGroundMultiplier = 0.35f;
	[SerializeField] private float _jumpHeight = 3;
	[SerializeField] private float _jumpReleaseDamping = 0.35f;
	[SerializeField] private GameObject _bulletPrefab;

	private SpritePhysics _physics;
	private InputManager _input;
	private bool _facingRight = true;

	void Start() {
		_input = InputManager.Instance;

		_physics = GetComponent<SpritePhysics>();
	}

	void FixedUpdate() {
		UpdateMovement();
		UpdateFacing();
		UpdateJumping();
		UpdateShooting();
	}

	private void UpdateMovement() {
		Vector3 vel = _physics.vel;
		float accel = _speed / _timeToMaxSpeed * Time.fixedDeltaTime;
		float v = vel.x;
		int dX = _input.X.Dir;

		if (!_physics.IsOnGround) {
			accel *= _moveOffGroundMultiplier;
		}

		float dV;
		if (dX == 0) { // pressing no direction
			float s = -Mathf.Sign(v);
			float mag = Mathf.Abs(v);
			dV = s * Mathf.Min(_moveReleaseMultiplier * accel, mag);
		}
		else if (v * dX >= 0) { // pressing same direction
			dV = accel * dX;
		}
		else { // pressing opposite direction
			dV = _moveOppositeMultiplier * accel * dX;
		}
		
		vel.x = Mathf.Clamp(v + dV, -_speed, _speed);
		_physics.vel = vel;
	}

	private void UpdateFacing() {
		if (_physics.vel.x > 0) {
			_facingRight = true;
		}
		else if (_physics.vel.x < 0) {
			_facingRight = false;
		}
	}

	private void UpdateJumping() {
		Vector3 vel = _physics.vel;
		bool isFalling = vel.y * Physics2D.gravity.y > 0;
		if (_input.Jump.DidPress && _physics.IsOnGround) {
			vel.y = JumpSpeed();
		}
		else if (_input.Jump.DidRelease && !isFalling) {
			vel.y *= _jumpReleaseDamping;
		}
		_physics.vel = vel;
	}

	private float JumpSpeed() {
		float g = Physics2D.gravity.y;
		return -Mathf.Sign(g) * Mathf.Sqrt(2 * _jumpHeight * Mathf.Abs(g));
	}

	private void UpdateShooting() {
		if (_input.Shoot.DidPress) {
			GameObject bulletObj = GameObject.Instantiate(_bulletPrefab);
			bulletObj.transform.position = transform.position;
			Bullet bullet = bulletObj.GetComponent<Bullet>();
			float bulletXDir = _facingRight ? 1 : -1;
			bullet.Init(gameObject, new Vector3(bulletXDir, 0, 0));
		}
	}
}
