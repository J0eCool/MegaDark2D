using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHealth : Health {
	public override void Kill() {
		Application.LoadLevel(Application.loadedLevel);
	}

	public override void OnCollide(CollisionData collision) {
		bool isEnemy = EnemyManager.Instance.IsEnemy(collision.sender);
		if (isEnemy) {
			TakeDamage(2);
		}
	}
}
