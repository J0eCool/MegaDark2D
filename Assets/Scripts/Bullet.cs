using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, Collideable {
	[SerializeField] private float _speed = 15.0f;
	[SerializeField] private float _maxRange = 300.0f;
	[SerializeField] private int _damage = 1;

	private SpritePhysics _physics;
	private float _flownDist = 0.0f;
	private GameObject _shooter;

	public int Damage { get { return _damage; } }

	public void Init(GameObject shooter, Vector3 dir) {
		_shooter = shooter;

		_physics = GetComponent<SpritePhysics>();
		_physics.vel = _speed * dir;
	}

	void FixedUpdate() {
		if (_physics.vel.sqrMagnitude == 0) {
			GameObject.Destroy(gameObject);
		}
		if (_maxRange > 0) {
			_flownDist += _speed * Time.fixedDeltaTime;
			if (_flownDist > _maxRange) {
				GameObject.Destroy(gameObject);
			}
		}
	}

	public void OnCollide(CollisionData collision) {
	}

	public void DidHit(GameObject obj) {
		if (obj != _shooter) {
			GameObject.Destroy(gameObject);
		}
	}
}
