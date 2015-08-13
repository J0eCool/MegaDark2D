using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BottomlessPit : MonoBehaviour, Collideable {
	public void OnCollide(CollisionData collision) {
		var health = collision.sender.GetComponent<Health>();
		if (health) {
			health.Kill();
		}
	}
}
