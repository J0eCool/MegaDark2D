using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	[SerializeField] private float _speed = 10;

	private SpritePhysics _physics;

	void Start() {
		_physics = GetComponent<SpritePhysics>();
	}

	void Update() {
		Vector3 vel = _physics.vel;

		vel.x = _speed * Input.GetAxis("Horizontal");

		_physics.vel = vel;
	}
}
