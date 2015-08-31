using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMana : CappedAmount {
	[SerializeField] private float regenPerSecond = 5.0f;

	private float partialAmount = 0.0f;

	void Update() {
		partialAmount += regenPerSecond * Time.deltaTime;
		int delta = (int)partialAmount;
		partialAmount -= delta;
		Current += delta;
	}

	public bool TrySpend(int amount) {
		if (Current < amount) {
			return false;
		}
		Current -= amount;
		return true;
	}
}
