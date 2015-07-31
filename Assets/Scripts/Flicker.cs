using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flicker : MonoBehaviour {
	[SerializeField] private float _flickerRate = 0.2f;
	[SerializeField] private float _fadeAlpha = 0.0f;

	private tk2dSprite _sprite;
	private float _baseAlpha;
	private bool _isFaded = false;
	private float _flickerTimer = 0.0f;
	private float _fadeTimer = 0.0f;
	private float _timeToFlicker = -1.0f;

	void Start() {
		_sprite = GetComponent<tk2dSprite>();
		_baseAlpha = _sprite.color.a;
	}
		
	public void BeginFlicker(float flickerTime = float.PositiveInfinity) {
		_timeToFlicker = flickerTime;
		_flickerTimer = 0.0f;
		_fadeTimer = 0.0f;
		SetFaded(true);
	}

	public void EndFlicker() {
		_timeToFlicker = -1.0f;
		SetAlpha(_baseAlpha);
	}

	private void SetAlpha(float alpha) {
		var color = _sprite.color;
		color.a = alpha;
		_sprite.color = color;
	}

	void Update() {
		if (!IsFlickering()) {
			return;
		}

		_flickerTimer += Time.deltaTime;
		_fadeTimer += Time.deltaTime;
		if (_fadeTimer >= _flickerRate) {
			_fadeTimer -= _flickerRate;
			SetFaded(!_isFaded);
		}
		if (_flickerTimer >= _timeToFlicker) {
			EndFlicker();
		}
	}

	private void SetFaded(bool fade) {
		_isFaded = fade;
		float alpha = _isFaded ? _fadeAlpha : _baseAlpha;
		SetAlpha(alpha);
	}

	public bool IsFlickering() {
		return _timeToFlicker > 0.0f;
	}
}
