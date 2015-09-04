using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class JComponent : MonoBehaviour {
	protected virtual void onStart() { }

	protected virtual void onDestroy() { }
	protected void Destroy() {
		var components = GetComponents<JComponent>();
		foreach (var component in components) {
			component.onDestroy();
		}

		GameObject.Destroy(gameObject);
	}

	void Start() {
		setupStartComponents();
		onStart();
	}

	private void setupStartComponents() {
		foreach (FieldInfo field in getPrivateFields()) {
			if (isFieldAnnotated<StartComponentAttribute>(field)) {
				field.SetValue(this, GetComponent(field.FieldType));
			}
		}
	}

	private FieldInfo[] getPrivateFields() {
		return GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
	}

	private static bool isFieldAnnotated<T>(FieldInfo field) where T : Attribute{
		return Attribute.GetCustomAttribute(field, typeof(T)) != null;
	}
}

[AttributeUsage(AttributeTargets.Field)]
public class StartComponentAttribute : Attribute {
}
