using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CappedAmount : MonoBehaviour {
	[SerializeField] private int maxAmount = 4;

	private int currentAmount;

	public int Current {
		get { return currentAmount; }
		protected set {
			currentAmount = Mathf.Min(value, maxAmount);
		}
	}
	public int Max { get { return maxAmount; } }

	void Start() {
		onStart();
	}

	protected virtual void onStart() {
		currentAmount = maxAmount;
	}
}
