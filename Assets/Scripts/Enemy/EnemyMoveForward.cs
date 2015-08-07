using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMoveForward : MonoBehaviour {
	[SerializeField] private float moveSpeed;

	private SpritePhysics physics;

	void Start() {
		physics = GetComponent<SpritePhysics>();
	}
		
	void Update() {
		var vel = physics.vel;
		vel.x = moveSpeed;
		physics.vel = vel;
	}
}
