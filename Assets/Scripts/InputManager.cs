using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : SingletonComponent<InputManager> {
	public Axis X { get; private set; }
	public Axis Y { get; private set; }

	public Button Jump { get; private set; }
	public Button Shoot { get; private set; }
	public Button Special { get; private set; }
	public Button Reset { get; private set; }

	private List<Updateable> inputs = new List<Updateable>();

	void Start() {
		inputs.Add(X = new Axis("Horizontal"));
		inputs.Add(Y = new Axis("Vertical"));

		inputs.Add(Jump = new Button("Jump"));
		inputs.Add(Shoot = new Button("Shoot"));
		inputs.Add(Special = new Button("Special"));
		inputs.Add(Reset = new Button("Reset"));
	}

	void FixedUpdate() {
		foreach (var input in inputs) {
			input.Update();
		}
	}
}

public interface Updateable {
	void Update();
}

public class Axis : Updateable {
	public int Dir { get; private set; }

	private string axisName;

	public Axis(string axisName) {
		this.axisName = axisName;
	}

	public void Update() {
		float raw = Input.GetAxis(axisName);
		if (raw > 0.5f) {
			Dir = 1;
		}
		else if (raw < -0.5f) {
			Dir = -1;
		}
		else {
			Dir = 0;
		}
	}
}

public class Button : Updateable {
	public bool DidPress { get; private set; }
	public bool DidRelease { get; private set; }
	public bool IsHeld { get; private set; }

	private bool wasHeld = false;
	private string axisName;

	public Button(string axisName) {
		this.axisName = axisName;
	}

	public void Update() {
		IsHeld = Input.GetAxis(axisName) > 0.5f;
		DidPress = IsHeld && !wasHeld;
		DidRelease = !IsHeld && wasHeld;
		wasHeld = IsHeld;
	}
}
