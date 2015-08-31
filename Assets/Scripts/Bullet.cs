using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, Collideable {
	[SerializeField] private float speed = 15.0f;
	[SerializeField] private float maxRange = 50.0f;
	[SerializeField] private int damage = 1;
	[SerializeField] private int manaCost = 0;

	private SpritePhysics physics;
	private float flownDist = 0.0f;
	private GameObject shooter;

	public int Damage { get { return damage; } }
	public int ManaCost { get { return manaCost; } }

	public void Init(GameObject shooter, Vector3 dir) {
		this.shooter = shooter;

		physics = GetComponent<SpritePhysics>();
		physics.vel = speed * dir;
	}

	void FixedUpdate() {
		if (physics.vel.sqrMagnitude == 0) {
			GameObject.Destroy(gameObject);
		}
		if (maxRange > 0) {
			flownDist += speed * Time.fixedDeltaTime;
			if (flownDist > maxRange) {
				GameObject.Destroy(gameObject);
			}
		}
	}

	public void OnCollide(CollisionData collision) {
		if (collision.sender != shooter) {
			Health health = collision.sender.GetComponent<Health>();
			if (health == null || !health.IsInvincible()) {
				GameObject.Destroy(gameObject);
			}
		}
	}
}
