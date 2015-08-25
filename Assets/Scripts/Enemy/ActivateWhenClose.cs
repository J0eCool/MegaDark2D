using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateWhenClose : MonoBehaviour {
	[SerializeField] private float range = 8.0f;

	private List<MonoBehaviour> componentsToActivate = new List<MonoBehaviour>();

	void Start() {
		var components = GetComponents<MonoBehaviour>();
		foreach (var component in components) {
			if (component.enabled && component != this) {
				component.enabled = false;
				componentsToActivate.Add(component);
			}
		}
	}

	void Update() {
		GameObject player = PlayerManager.Instance.Player;
		Vector3 pos = transform.position;
		Vector3 playerPos = player.transform.position;
		Vector3 delta = pos - playerPos;
		delta.z = 0;
		bool isInRange = delta.sqrMagnitude < range * range;
		if (isInRange) {
			foreach (var component in componentsToActivate) {
				component.enabled = true;
			}
			enabled = false;
		}
	}
}
