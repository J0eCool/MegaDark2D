using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private float _speed = 10;
	[SerializeField] private float _jumpHeight = 3;
	[SerializeField] private float _jumpReleaseDamping = 0.35f;

	private SpritePhysics _physics;
	private InputManager _input;

	void Start() {
		_input = InputManager.Instance;

		_physics = GetComponent<SpritePhysics>();
	}

	void Update() {
		Vector3 vel = _physics.vel;

		vel.x = _speed * _input.X.Dir;

		if (_input.Jump.DidPress && _physics.IsOnGround) {
			vel.y = JumpSpeed();
		}
		else if (_input.Jump.DidRelease && vel.y * Physics2D.gravity.y < 0) {
			vel.y *= _jumpReleaseDamping;
		}

		_physics.vel = vel;
	}

	private float JumpSpeed() {
		float g = Physics2D.gravity.y;
		return -Mathf.Sign(g) * Mathf.Sqrt(2 * _jumpHeight * Mathf.Abs(g));
	}
}
