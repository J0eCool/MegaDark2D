using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private float _speed = 10;
	[SerializeField] private float _jumpHeight = 3;
	[SerializeField] private float _jumpReleaseDamping = 0.35f;

	private SpritePhysics _physics;

	private bool _wasJumpPressed = false;

	void Start() {
		_physics = GetComponent<SpritePhysics>();
	}

	void Update() {
		Vector3 vel = _physics.vel;

		vel.x = _speed * Input.GetAxis("Horizontal");

		bool isJumpPressed = Input.GetAxis("Jump") > 0.5f;
		bool didPress = isJumpPressed && !_wasJumpPressed;
		bool didRelease = !isJumpPressed && _wasJumpPressed;
		if (didPress && _physics.IsOnGround) {
			vel.y = JumpSpeed();
		}
		else if (didRelease && vel.y * Physics2D.gravity.y < 0) {
			vel.y *= _jumpReleaseDamping;
		}
		_wasJumpPressed = isJumpPressed;

		_physics.vel = vel;
	}

	private float JumpSpeed() {
		float g = Physics2D.gravity.y;
		return -Mathf.Sign(g) * Mathf.Sqrt(2 * _jumpHeight * Mathf.Abs(g));
	}
}
