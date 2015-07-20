using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private float _speed = 10;
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

	void Update() {
		UpdateMovement();
		UpdateFacing();
		UpdateJumping();
		UpdateShooting();
	}

	private void UpdateMovement() {
		Vector3 vel = _physics.vel;
		vel.x = _speed * _input.X.Dir;
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
			bullet.Init(new Vector3(bulletXDir, 0, 0));
		}
	}
}
