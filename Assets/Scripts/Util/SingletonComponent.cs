using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class SingletonComponent<T> : JComponent where T : SingletonComponent<T> {
	public static T Instance { get; private set; }

	void Awake() {
		Instance = this as T;
	}
}
