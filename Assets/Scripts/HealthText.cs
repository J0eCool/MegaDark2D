using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthText : MonoBehaviour {
	[SerializeField] private Health health;

	private Text text;
	private string formatText;

	void Start() {
		text = GetComponent<Text>();

		formatText = text.text;
	}

	void Update() {
		text.text = string.Format(formatText, health.CurrentHealth, health.MaxHealth);
	}
}
