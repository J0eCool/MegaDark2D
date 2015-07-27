using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : SingletonComponent<InputManager> {
	public Axis X { get; private set; }
	public Axis Y { get; private set; }

	public Button Jump { get; private set; }
	public Button Shoot { get; private set; }

	private List<IUpdateable> _inputs = new List<IUpdateable>();

	void Start() {
		_inputs.Add(X = new Axis("Horizontal"));
		_inputs.Add(Y = new Axis("Vertical"));

		_inputs.Add(Jump = new Button("Jump"));
		_inputs.Add(Shoot = new Button("Shoot"));
	}

	void FixedUpdate() {
		foreach (var input in _inputs) {
			input.Update();
		}
	}
}

public interface IUpdateable {
	void Update();
}

public class Axis : IUpdateable {
	public int Dir { get; private set; }
	private string _axisName;

	public Axis(string axisName) {
		_axisName = axisName;
	}

	public void Update() {
		float raw = Input.GetAxis(_axisName);
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

public class Button : IUpdateable {
	public bool DidPress { get; private set; }
	public bool DidRelease { get; private set; }
	public bool IsHeld { get; private set; }
	private bool _wasHeld = false;
	private string _axisName;

	public Button(string axisName) {
		_axisName = axisName;
	}

	public void Update() {
		IsHeld = Input.GetAxis(_axisName) > 0.5f;
		DidPress = IsHeld && !_wasHeld;
		DidRelease = !IsHeld && _wasHeld;
		_wasHeld = IsHeld;
	}
}
