using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerArea : MonoBehaviour {
	private int _count = 0;

	public bool IsColliding {
		get {
			return _count > 0;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		_count++;
	}

	void OnTriggerExit2D(Collider2D col) {
		_count--;
	}
}
