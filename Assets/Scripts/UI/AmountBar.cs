using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AmountBar : MonoBehaviour {
	[SerializeField] private CappedAmount amount;
	[SerializeField] private RectTransform bar;
	[SerializeField] private Text text;
	
	private string formatText;
	private float baseWidth;

	void Start() {
		formatText = text.text;
		baseWidth = bar.localScale.x;
	}

	void Update() {
		text.text = string.Format(formatText, amount.Current, amount.Max);

		float pct = (float)amount.Current / amount.Max;
		var scale = bar.localScale;
		scale.x = baseWidth * pct;
		bar.localScale = scale;
	}
}
