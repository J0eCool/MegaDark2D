using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : SingletonComponent<EnemyManager> {
	private int enemyLayer = -1;

	void Start() {
		enemyLayer = LayerMask.NameToLayer("Enemy");
	}

	public bool IsEnemy(GameObject obj) {
		return obj.layer == enemyLayer;
	}
}
