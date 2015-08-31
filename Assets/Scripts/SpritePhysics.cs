﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePhysics : MonoBehaviour {
	[SerializeField] private int numHorizontalRays = 3;
	[SerializeField] private int numVerticalRays = 3;
	[SerializeField] private bool ignoreGravity = false;
	[SerializeField] private bool ignoreTerrain = false;
	[SerializeField] private bool debugDrawRays = false;

	public Vector2 Vel { get; set; }
	public bool IsOnGround { get; private set; }

	private BoxCollider2D boxCollider;
	private List<Vector2> offsets = new List<Vector2>();
	private List<Collideable> collisionListeners = new List<Collideable>();

	private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();

	private const float kEpsilon = 0.001f;
	private static HashSet<int> physicallyCollidingLayers = null;

	void Awake() {
		if (physicallyCollidingLayers == null) {
			physicallyCollidingLayers = new HashSet<int>();
			physicallyCollidingLayers.Add(LayerMask.NameToLayer("Default"));
		}
	}

	void Start() {
		IsOnGround = false;
		boxCollider = GetComponent<BoxCollider2D>();

		for (int i = 0; i < numHorizontalRays; i++) {
			float x = boxCollider.bounds.extents.x * (2 * i - numHorizontalRays + 1) / (numHorizontalRays - 1);
			float y = boxCollider.bounds.extents.y;
			offsets.Add(new Vector2(x, y));
			offsets.Add(new Vector2(x, -y));
		}
		for (int i = 1; i < numVerticalRays - 1; i++) {
			float x = boxCollider.bounds.extents.x;
			float y = boxCollider.bounds.extents.y * (2 * i - numVerticalRays + 1) / (numVerticalRays - 1);
			offsets.Add(new Vector2(x, y));
			offsets.Add(new Vector2(-x, y));
		}

		var listeners = GetComponents<Collideable>();
		foreach (var listener in listeners) {
			RegisterListener(listener);
		}
	}
		
	void FixedUpdate() {
		UpdatePosition();
		FlushHitMessages();

		if (debugDrawRays) {
			foreach (Vector3 offset in offsets) {
				Vector3 origin = transform.position + offset;
				Debug.DrawLine(origin, origin + (Vector3)Vel * Time.fixedDeltaTime, Color.green);
			}
		}
	}

	private void UpdatePosition() {
		Vector3 v = Vel;
		if (!ignoreGravity) {
			v += (Vector3)Physics2D.gravity * Time.fixedDeltaTime;
		}

		float g = Physics2D.gravity.y;

		if (v.y * g < 0 || v.y * Mathf.Sign(g) > 0.15f * Mathf.Abs(g)) {
			IsOnGround = false;
		}

		v.x = GoDir(new Vector2(v.x, 0)).x;
		v.y = GoDir(new Vector2(0, v.y)).y;
		Vel = v;
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
		foreach (Vector3 offset in offsets) {
			Vector3 origin = transform.position + offset;
			RaycastHit2D[] hits = Physics2D.RaycastAll(origin, toMove.normalized, toMove.magnitude);
			foreach (var hit in hits) {
				var hitObj = hit.collider.gameObject;
				if (CanCollideLogically(hitObj)) {
					collidedObjects.Add(hitObj);
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
		bool isLayerPhysical = physicallyCollidingLayers.Contains(hitObj.layer);
		bool isShorterHit = minHit == null || hit.distance < minHit.Value.distance;
		return !ignoreTerrain && canLogicallyHit && isLayerPhysical && isShorterHit;
	}

	private void FlushHitMessages() {
		foreach (var other in collidedObjects) {
			CollidedWith(other);
			var otherPhysics = other.GetComponent<SpritePhysics>();
			if (otherPhysics != null) {
				otherPhysics.CollidedWith(gameObject);
			}
		}
		collidedObjects.Clear();
	}

	private void CollidedWith(GameObject other) {
		foreach (var listener in collisionListeners) {
			listener.OnCollide(new CollisionData(other));
		}
	}

	public void RegisterListener(Collideable listener) {
		collisionListeners.Add(listener);
	}
}

public class CollisionData {
	public GameObject sender;

	public CollisionData(GameObject sender) {
		this.sender = sender;
	}
}

public interface Collideable {
	void OnCollide(CollisionData collision);
}
