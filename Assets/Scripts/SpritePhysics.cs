using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePhysics : MonoBehaviour {
	[SerializeField] private int _numHorizontalRays = 3;
	[SerializeField] private int _numVerticalRays = 3;
	[SerializeField] private bool _ignoreGravity = false;
	[SerializeField] private bool _debugDrawRays = false;

	public Vector3 vel { get; set; }
	public bool IsOnGround { get; private set; }

	private BoxCollider2D _collider;
	private List<Vector2> _offsets = new List<Vector2>();
	private List<ICollideable> _collisionListeners = new List<ICollideable>();

	private HashSet<GameObject> _collidedObjects = new HashSet<GameObject>();

	private const float kEpsilon = 0.001f;
	private static HashSet<int> _physicallyCollidingLayers = null;

	void Awake() {
		if (_physicallyCollidingLayers == null) {
			_physicallyCollidingLayers = new HashSet<int>();
			_physicallyCollidingLayers.Add(LayerMask.NameToLayer("Default"));
		}
	}

	void Start() {
		IsOnGround = false;
		_collider = GetComponent<BoxCollider2D>();

		for (int i = 0; i < _numHorizontalRays; i++) {
			float x = _collider.bounds.extents.x * (2 * i - _numHorizontalRays + 1) / (_numHorizontalRays - 1);
			float y = _collider.bounds.extents.y;
			_offsets.Add(new Vector2(x, y));
			_offsets.Add(new Vector2(x, -y));
		}
		for (int i = 1; i < _numVerticalRays - 1; i++) {
			float x = _collider.bounds.extents.x;
			float y = _collider.bounds.extents.y * (2 * i - _numVerticalRays + 1) / (_numVerticalRays - 1);
			_offsets.Add(new Vector2(x, y));
			_offsets.Add(new Vector2(-x, y));
		}
	}
		
	void FixedUpdate() {
		UpdatePosition();
		FlushHitMessages();

		if (_debugDrawRays) {
			foreach (Vector3 offset in _offsets) {
				Vector3 origin = transform.position + offset;
				Debug.DrawLine(origin, origin + vel * Time.fixedDeltaTime, Color.green);
			}
		}
	}

	private void UpdatePosition() {
		Vector3 v = vel;
		if (!_ignoreGravity) {
			v += (Vector3)Physics2D.gravity * Time.fixedDeltaTime;
		}

		float g = Physics2D.gravity.y;

		if (v.y * g < 0 || v.y * Mathf.Sign(g) > 0.15f * Mathf.Abs(g)) {
			IsOnGround = false;
		}

		v.x = GoDir(new Vector2(v.x, 0)).x;
		v.y = GoDir(new Vector2(0, v.y)).y;
		vel = v;
	}

	private Vector2 GoDir(Vector2 v) {
		bool isY = Mathf.Abs(v.y) > 0;
		Vector3 toMove = v * Time.fixedDeltaTime;
		RaycastHit2D? hit = FindCollision(toMove);
		if (hit != null) {
			v = Vector2.zero;
			float d = Mathf.Max(hit.Value.distance - kEpsilon, 0.0f);
			if (isY) {
				IsOnGround = toMove.y * Physics2D.gravity.y > 0;
			}
			float s = Mathf.Sign(isY ? toMove.y : toMove.x);
			float dist = s * d;
			if (isY) {
				toMove.y = dist;
			}
			else {
				toMove.x = dist;
			}
		}
		transform.position += toMove;
		return v;
	}

	private RaycastHit2D? FindCollision(Vector3 toMove) {
		RaycastHit2D? minHit = null;
		foreach (Vector3 offset in _offsets) {
			Vector3 origin = transform.position + offset;
			RaycastHit2D[] hits = Physics2D.RaycastAll(origin, toMove.normalized, toMove.magnitude);
			foreach (var hit in hits) {
				var hitObj = hit.collider.gameObject;
				if (CanCollideLogically(hitObj)) {
					_collidedObjects.Add(hitObj);
				}
				if (CanCollidePhysically(hitObj, hit, minHit)) {
					minHit = hit;
				}
			}
		}
		return minHit;
	}

	private bool CanCollideLogically(GameObject obj) {
		bool isNotSelf = obj != gameObject;
		bool shouldNotIgnore = !Physics2D.GetIgnoreLayerCollision(
			gameObject.layer, obj.layer);
		return isNotSelf && shouldNotIgnore;
	}

	private bool CanCollidePhysically(GameObject hitObj, RaycastHit2D hit, RaycastHit2D? minHit) {
		bool canLogicallyHit = CanCollideLogically(hitObj);
		bool isLayerPhysical = _physicallyCollidingLayers.Contains(hitObj.layer);
		bool isShorterHit = minHit == null || hit.distance < minHit.Value.distance;
		return canLogicallyHit && isLayerPhysical && isShorterHit;
	}

	private void FlushHitMessages() {
		foreach (var other in _collidedObjects) {
			CollidedWith(other);
			var otherPhysics = other.GetComponent<SpritePhysics>();
			if (otherPhysics != null) {
				otherPhysics.CollidedWith(gameObject);
			}
		}
		_collidedObjects.Clear();
	}

	private void CollidedWith(GameObject other) {
		foreach (var listener in _collisionListeners) {
			listener.OnCollide(new CollisionData(other));
		}
	}

	public void RegisterListener(ICollideable listener) {
		_collisionListeners.Add(listener);
	}
}

public class CollisionData {
	public GameObject sender;

	public CollisionData(GameObject sender) {
		this.sender = sender;
	}
}

public interface ICollideable {
	void OnCollide(CollisionData collision);
}
