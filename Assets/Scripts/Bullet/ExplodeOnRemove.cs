﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodeOnRemove : JComponent {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private int numBullets = 8;

	protected override void onDestroy() {
		Bullet bullet = GetComponent<Bullet>();
		GameObject shooter = gameObject;
		if (bullet != null) {
			shooter = bullet.Shooter;
		}

		for (int i = 0; i < numBullets; ++i) {
			float angle = 2 * Mathf.PI * i / numBullets;
			Vector2 dir = VectorUtil.Unit(angle);
			Bullet.Create(bulletPrefab, gameObject, dir, shooter:shooter);
		}
	}
}
