using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : Health {
	public override void Kill() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public override void OnCollide(CollisionData collision) {
		var enemy = collision.sender.GetComponent<EnemyMoveForward>();
		if (enemy != null) {
			TakeDamage(2);
		}
	}
}
