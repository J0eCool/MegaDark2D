using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {
	[SerializeField] private GameObject target;
	[SerializeField] private bool lockX;
	[SerializeField] private bool lockY;

	void Update() {
		Vector3 pos = transform.position;
		Vector3 targetPos = target.transform.position;
		if (!lockX) {
			pos.x = targetPos.x;
		}
		if (!lockY) {
			pos.y = targetPos.y;
		}
		transform.position = pos;
	}
}
