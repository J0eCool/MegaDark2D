using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : Health {
	public override void OnCollide(CollisionData collision) {
		var bullet = collision.sender.GetComponent<Bullet>();
		if (bullet != null) {
			TakeDamage(bullet.Damage);
		}
	}
}
