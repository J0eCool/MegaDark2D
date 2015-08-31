using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Autofire : MonoBehaviour {
	[SerializeField] private float shotsPerSecond = 3.0f;
	[SerializeField] private GameObject bulletPrefab;

	private float shotTimer = 0.0f;
		
	void Update() {
		shotTimer += Time.deltaTime;
		float timePerShot = 1.0f / shotsPerSecond;
		if (shotTimer > timePerShot) {
			shotTimer -= timePerShot;
			shoot(bulletPrefab);
		}
	}

	private void shoot(GameObject prefab) {
		Bullet.Create(prefab, gameObject, new Vector2(1, 0));
	}
}
