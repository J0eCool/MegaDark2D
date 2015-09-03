using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMoveForward : MonoBehaviour {
	[SerializeField] private float moveSpeed;
	[SerializeField] private bool facingRight = false;

	private SpritePhysics physics;


	void Start() {
		physics = GetComponent<SpritePhysics>();
	}
		
	void FixedUpdate() {
		if (physics.DidHitLeft && !facingRight) {
			facingRight = true;
		}
		else if (physics.DidHitRight && facingRight) {
			facingRight = false;
		}

		var vel = physics.Vel;
		vel.x = moveSpeed * (facingRight ? 1 : -1);
		physics.Vel = vel;
	}
}
