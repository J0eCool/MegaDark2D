using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JComponent : MonoBehaviour {
	protected void Destroy() {
		var components = GetComponents<JComponent>();
		foreach (var component in components) {
			component.onDestroy();
		}

		GameObject.Destroy(gameObject);
	}

	protected virtual void onDestroy() { }
}
