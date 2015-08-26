using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyDropMovement : MonoBehaviour {
	[SerializeField] private float flySpeed = -2.0f;
    [SerializeField] private float dropInitialSpeed = 2.0f;
    [SerializeField] private float dropAcceleration = -5.0f;

    private SpritePhysics physics = null;

    private bool isFalling = false;

    void Start() {
        physics = GetComponent<SpritePhysics>();
	}

    void FixedUpdate() {
        if (!isFalling) {
	        flyUpdate();
		}
        else {
			fallUpdate();
		}
	}

    private void flyUpdate() {
        GameObject player = PlayerManager.Instance.Player;
        Vector3 playerPos = player.transform.position;
        Vector3 delta = playerPos - transform.position;
        setVel(flySpeed, 0.0f);

		possiblyStartFalling(delta);
	}

	private void possiblyStartFalling(Vector3 delta) {
		if (shouldFall(delta)) {
			isFalling = true;
			setVel(0.0f, dropInitialSpeed);
		}
	}

    private void setVel(float x, float y) {
        physics.vel = new Vector2(x, y);
	}

    private bool shouldFall(Vector3 delta) {
		return delta.x * flySpeed < 0.0f;
	}

    private void fallUpdate() {
		float vel = physics.vel.y;
		vel += Time.fixedDeltaTime * dropAcceleration;
        setVel(0.0f, vel);
	}
}
