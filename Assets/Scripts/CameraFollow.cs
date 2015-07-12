using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {
	[SerializeField] private GameObject _target;

	void FixedUpdate() {
		Vector3 pos = _target.transform.position;
		pos.z = transform.position.z;
		transform.position = pos;
	}
}
