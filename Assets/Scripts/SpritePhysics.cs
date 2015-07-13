using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePhysics : MonoBehaviour {
	[SerializeField] private int _numHorizontalRays = 3;
	[SerializeField] private bool _debugDrawRays = false;

	public Vector3 vel { get; set; }
	public bool IsOnGround { get; private set; }

	private BoxCollider2D _collider;
	private List<Vector2> _offsets = new List<Vector2>();

	private const float kEpsilon = 0.001f;

	void Start() {
		IsOnGround = false;
		_collider = GetComponent<BoxCollider2D>();

		for (int i = 0; i < _numHorizontalRays; i++) {
			float x = _collider.bounds.extents.x * (2 * i - _numHorizontalRays + 1) / (_numHorizontalRays - 1);
			float y = -_collider.bounds.extents.y;
			_offsets.Add(new Vector2(x, y));
		}
	}
		
	void FixedUpdate() {
		Vector3 toMove = CalculateMoveDistance();
		UpdatePosition(toMove);

		if (_debugDrawRays) {
			foreach (Vector3 offset in _offsets) {
				Vector3 origin = transform.position + offset;
				Debug.DrawLine(origin, origin + vel * Time.fixedDeltaTime, Color.green);
			}
		}
	}

	private void UpdatePosition(Vector3 toMove) {
		Vector3 pos = transform.position;
		pos += toMove;
		transform.position = pos;
	}

	private Vector3 CalculateMoveDistance() {
		Vector3 v = vel;
		v += (Vector3)Physics2D.gravity * Time.fixedDeltaTime;
		Vector3 toMove = v * Time.fixedDeltaTime;

		IsOnGround = false;

		//RaycastHit2D? minHit = FindCollision(toMove);
		//if (minHit != null) {
		//	toMove = v.normalized * (minHit.Value.distance);
		//}
		RaycastHit2D? xHit = FindCollision(toMove.x * Vector2.right);
		if (xHit != null) {
			v.x = 0;
			float d = xHit.Value.distance - kEpsilon;
			if (d < 0) { d = 0; }
			toMove.x = d * Mathf.Sign(toMove.x);
		}
		RaycastHit2D? yHit = FindCollision(toMove.y * Vector2.up);
		if (yHit != null) {
			v.y = 0;
			float d = yHit.Value.distance - kEpsilon;
			if (d < 0) { d = 0; }
			IsOnGround = toMove.y * Physics2D.gravity.y > 0;
			toMove.y = d * Mathf.Sign(toMove.y);
		}
		vel = v;
		return toMove;
	}

	private RaycastHit2D? FindCollision(Vector3 toMove) {
		RaycastHit2D? minHit = null;
		foreach (Vector3 offset in _offsets) {
			Vector3 origin = transform.position + offset;
			RaycastHit2D[] hits = Physics2D.RaycastAll(origin, toMove.normalized, toMove.magnitude);
			foreach (var hit in hits) {
				if (hit.collider.gameObject != gameObject && (minHit == null || hit.distance < minHit.Value.distance)) {
					minHit = hit;
				}
			}
		}
		return minHit;
	}
}
