using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour, Collideable {
	[SerializeField] private int _maxHealth = 4;
	[SerializeField] private float _invincibleTime = 0.5f;
	
	private int _health;

	private Flicker _flicker;

	void Start() {
		_health = _maxHealth;

		_flicker = GetComponent<Flicker>();

		var physics = GetComponent<SpritePhysics>();
		physics.RegisterListener(this);
	}

	public void OnCollide(CollisionData collision) {
		var bullet = collision.sender.GetComponent<Bullet>();
		if (bullet != null && !IsInvincible()) {
			_health -= bullet.Damage;
			_flicker.BeginFlicker(_invincibleTime);

			if (_health <= 0) {
				GameObject.Destroy(gameObject);
			}
		}
	}

	public bool IsInvincible() {
		return _flicker.IsFlickering();
	}
}
