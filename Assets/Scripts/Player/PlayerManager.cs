using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : SingletonComponent<PlayerManager> {
	public GameObject Player { get; private set; }

	void Start() {
		Player = GameObject.Find("Player");
	}
}
