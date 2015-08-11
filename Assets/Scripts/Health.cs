﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health : MonoBehaviour, Collideable {
	[SerializeField] private int maxHealth = 4;
	[SerializeField] private float invincibleTime = 0.5f;
	
	private int health;
	
	private Flicker flicker;

	public int CurrentHealth { get { return health; } }
	public int MaxHealth { get { return maxHealth; } }

	void Start() {
		health = maxHealth;

		flicker = GetComponent<Flicker>();

		var physics = GetComponent<SpritePhysics>();
		physics.RegisterListener(this);
	}

	void FixedUpdate() {
		if (health <= 0) {
			Kill();
		}
	}

	public virtual void Kill() {
		GameObject.Destroy(gameObject);
	}

	public virtual void OnCollide(CollisionData collision) {
		var bullet = collision.sender.GetComponent<Bullet>();
		if (bullet != null) {
			TakeDamage(bullet.Damage);
		}
	}

	protected void TakeDamage(int damage) {
		if (!IsInvincible()) {
			health -= damage;
			flicker.BeginFlicker(invincibleTime);
		}
	}

	public bool IsInvincible() {
		return flicker.IsFlickering();
	}
}
