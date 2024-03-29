﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpritePhysics : MonoBehaviour {
    [SerializeField] private int numVerticalRays = 3;
    [SerializeField] private int numHorizontalSegments = 3;
    [SerializeField] private bool ignoreGravity = false;
    [SerializeField] private bool ignoreTerrain = false;
    [SerializeField] private bool debugDrawRays = false;
    [SerializeField] private Vector2 vel;

    public Vector2 Vel {
        get { return vel; }
        set { vel = value; }
    }
    public bool IsOnGround { get; private set; }

    public bool DidHitLeft { get; private set; }
    public bool DidHitRight { get; private set; }
    public bool DidHitUp { get; private set; }
    public bool DidHitDown { get; private set; }

    private BoxCollider2D boxCollider;
    private List<Offset> offsets = new List<Offset>();
    private List<Collideable> collisionListeners = new List<Collideable>();

    private HashSet<GameObject> collidedObjects = new HashSet<GameObject>();

    private const float kEpsilon = 0.001f;
    private static HashSet<int> physicallyCollidingLayers = null;

    private struct Offset {
        public Vector3 position;
        public Vector2 normal;

        public Offset(Vector3 position, Vector2 normal) {
            this.position = position;
            this.normal = normal;
        }
    }

    void Awake() {
        if (physicallyCollidingLayers == null) {
            physicallyCollidingLayers = new HashSet<int>();
            physicallyCollidingLayers.Add(LayerMask.NameToLayer("Default"));
        }
    }

    void Start() {
        IsOnGround = false;
        boxCollider = GetComponent<BoxCollider2D>();

        for (int i = 0; i < numHorizontalSegments; i++) {
            float x = boxCollider.bounds.extents.x * (2 * i - numHorizontalSegments + 1) / (numHorizontalSegments - 1);
            float y = boxCollider.bounds.extents.y;
            offsets.Add(new Offset(new Vector2(x, y), new Vector2(0.0f, 1.0f)));
            offsets.Add(new Offset(new Vector2(x, -y), new Vector2(0.0f, -1.0f)));
        }
        for (int i = 0; i < numVerticalRays; i++) {
            float x = boxCollider.bounds.extents.x;
            float y = boxCollider.bounds.extents.y * (2 * i - numVerticalRays + 1) / (numVerticalRays - 1);
            offsets.Add(new Offset(new Vector2(x, y), new Vector2(1.0f, 0.0f)));
            offsets.Add(new Offset(new Vector2(-x, y), new Vector2(-1.0f, 0.0f)));
        }

        var listeners = GetComponents<Collideable>();
        foreach (var listener in listeners) {
            RegisterListener(listener);
        }
    }

    void FixedUpdate() {
        UpdatePosition();
        FlushHitMessages();
    }

    private void UpdatePosition() {
        Vector2 v = vel;
        if (!ignoreGravity) {
            v += Physics2D.gravity * Time.fixedDeltaTime;
        }

        resetHitFlags(v);

        v.x = GoDir(new Vector2(v.x, 0)).x;
        v.y = GoDir(new Vector2(0, v.y)).y;
        vel = v;
    }

    private void resetHitFlags(Vector2 v) {
        float g = Physics2D.gravity.y;
        if (v.y * g < 0 || v.y * Mathf.Sign(g) > 0.05f * Mathf.Abs(g)) {
            IsOnGround = false;
        }
        DidHitLeft = false;
        DidHitRight = false;
        DidHitUp = false;
        DidHitDown = false;
    }

    private Vector2 GoDir(Vector2 v) {
        bool isY = Mathf.Abs(v.y) > 0;
        Vector3 toMove = v * Time.fixedDeltaTime;
        RaycastHit2D? hit = FindCollision(toMove);
        if (hit != null) {
            if (debugDrawRays) {
                float rad = toMove.magnitude;
                Vector3 p = hit.Value.point;
                Debug.DrawLine(p, p + toMove, Color.red);
                DebugRender.Circle(rad, p, Color.red);
            }

            Vector3 sizeOffset = VectorUtil.Mult(boxCollider.bounds.extents, v.normalized);

            setHitFlags(v);
            v = Vector2.zero;

            float d = Mathf.Max(hit.Value.distance - kEpsilon - sizeOffset.magnitude, 0.0f);
            float s = Mathf.Sign(isY ? toMove.y : toMove.x);
            float dist = s * d;
            if (isY) {
                toMove.y = dist;
            } else {
                toMove.x = dist;
            }
        }
        transform.position += toMove;
        return v;
    }

    private void setHitFlags(Vector2 v) {
        bool isY = Mathf.Abs(v.y) > 0;
        if (isY) {
            if (v.y * Physics2D.gravity.y > 0) {
                IsOnGround = true;
                DidHitDown = true;
            } else {
                DidHitUp = true;
            }
        } else {
            if (v.x > 0) {
                DidHitRight = true;
            } else {
                DidHitLeft = true;
            }
        }
    }

    private RaycastHit2D? FindCollision(Vector3 toMove) {
        RaycastHit2D? minHit = null;
        Vector3 dir = toMove.normalized;
        foreach (Offset offset in offsets) {
            bool doCollide = true;
            if (Vector2.Dot(offset.normal, toMove) <= kEpsilon) {
                doCollide = false;
            }

            if (debugDrawRays) {
                Vector3 offsetPos = offset.position + transform.position;
                Color drawColor = doCollide ? Color.magenta : Color.grey;
                DebugRender.Circle(0.1f, offsetPos, drawColor);
                Debug.DrawLine(offsetPos, offsetPos + (Vector3)offset.normal * 0.2f, drawColor);
            }
            if (!doCollide) {
                continue;
            }

            Vector3 sizeOffset = VectorUtil.Mult(boxCollider.bounds.extents, offset.normal);
            Vector3 origin = transform.position + offset.position - sizeOffset;
            Vector3 moveDelta = toMove + sizeOffset;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, moveDelta.magnitude);

            if (debugDrawRays) {
                Debug.DrawLine(origin, origin + moveDelta, Color.green);
            }

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