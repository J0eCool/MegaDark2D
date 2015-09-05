using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Health : CappedAmount, Collideable {
	[SerializeField] private float invincibleTime = 0.05f;
		
	private Flicker flicker;

	protected override void onStart() {
		base.onStart();
		flicker = GetComponent<Flicker>();
	}

	void FixedUpdate() {
		if (Current <= 0) {
			Kill();
		}
	}

	public virtual void Kill() {
		GameObject.Destroy(gameObject);
	}

	public virtual void OnCollide(CollisionData collision) { }

	public void TakeDamage(int damage) {
		if (!IsInvincible()) {
			Current -= damage;
			flicker.BeginFlicker(invincibleTime);
		}
	}

	public bool IsInvincible() {
		return !enabled || flicker.IsFlickering();
	}
}
