using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMoveForward : JComponent {
	[SerializeField] private float moveSpeed;

	private SpritePhysics physics;
	private Facing facing;

	void Start() {
		physics = GetComponent<SpritePhysics>();
		facing = GetComponent<Facing>();
	}
		
	void FixedUpdate() {
		if (physics.DidHitLeft && !facing.FacingRight) {
			facing.FacingRight = true;
		}
		else if (physics.DidHitRight && facing.FacingRight) {
			facing.FacingRight = false;
		}

		var vel = physics.Vel;
		vel.x = moveSpeed * facing.Dir;
		physics.Vel = vel;
	}
}
