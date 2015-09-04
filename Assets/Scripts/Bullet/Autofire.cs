using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Autofire : JComponent {
	[SerializeField] private float shotsPerSecond = 3.0f;
	[SerializeField] private GameObject bulletPrefab;

	private Facing facing;
	private float shotTimer = 0.0f;

	void Start() {
		facing = GetComponent<Facing>();
	}

	void Update() {
		shotTimer += Time.deltaTime;
		float timePerShot = 1.0f / shotsPerSecond;
		if (shotTimer > timePerShot) {
			shotTimer -= timePerShot;
			shoot(bulletPrefab);
		}
	}

	private void shoot(GameObject prefab) {
		int dir = 1;
		if (facing != null) {
			dir = facing.Dir;
		}
		Bullet.Create(prefab, gameObject, new Vector2(dir, 0));
	}
}
