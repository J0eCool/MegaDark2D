﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : JComponent, Collideable {
	[SerializeField] private float speed = 15.0f;
	[SerializeField] private Vector2 baseDir = Vector2.zero;
	[SerializeField] private bool lockRotation = false;
	[SerializeField] private float maxRange = 50.0f;
	[SerializeField] private int damage = 1;
	[SerializeField] private int manaCost = 0;

	private SpritePhysics physics;
	private float flownDist = 0.0f;

	public int Damage { get { return damage; } }
	public int ManaCost { get { return manaCost; } }

	public static GameObject Create(GameObject prefab, GameObject createdAt, Vector2 dir) {
		GameObject bulletObj = GameObject.Instantiate(prefab);
		bulletObj.transform.position = createdAt.transform.position;
		Bullet bullet = bulletObj.GetComponent<Bullet>();
		bullet.Init(dir);
		return bulletObj;
	}

	public void Init(Vector2 dir) {
		physics = GetComponent<SpritePhysics>();
		Vector2 d = (dir + baseDir).normalized;
		physics.Vel = speed * d;
	}

	void FixedUpdate() {
		if (!lockRotation) {
			float angle = Mathf.Atan2(physics.Vel.y, physics.Vel.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}

		if (maxRange > 0) {
			flownDist += speed * Time.fixedDeltaTime;
			if (flownDist > maxRange) {
				Destroy();
			}
		}
	}

	public void OnCollide(CollisionData collision) {
		Health health = collision.sender.GetComponent<Health>();
		bool shouldDestroy = (health == null || !health.IsInvincible());
		if (shouldDestroy) {
			Destroy();
		}
		if (health != null) {
			health.TakeDamage(Damage);
		}
	}
}
