﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour {
	[SerializeField] private int _maxHealth = 4;

	private int _health;

	void Start() {
		_health = _maxHealth;
	}

	void OnCollide(CollisionData collision) {
		var bullet = collision.sender.GetComponent<Bullet>();
		if (bullet != null) {
			_health -= 1;
			if (_health <= 0) {
				GameObject.Destroy(gameObject);
			}
		}
	}
}