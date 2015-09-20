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
        setVel(flySpeed, 0.0f);

		possiblyStartFalling();
	}

	private void possiblyStartFalling() {
		if (shouldFall()) {
			isFalling = true;
			setVel(0.0f, dropInitialSpeed);
		}
	}

    private bool shouldFall() {
        GameObject player = PlayerManager.Instance.Player;
        SpritePhysics playerPhysics = player.GetComponent<SpritePhysics>();
        Vector3 playerPos = player.transform.position;
        Vector3 delta = playerPos - transform.position;

        float t = timeToFallFromHeight(delta.y);

        return (delta.x + t * playerPhysics.Vel.x) * flySpeed < 0.0f;
    }

    private float timeToFallFromHeight(float height) {
        // Given h = v_0 * t + 1/2 * a * t^2 ,
        // solve quadratic formula to find t = (-v_0 - sqrt(v_0^2 + 2*a*h)) / a
        float v0 = dropInitialSpeed;
        float a = dropAcceleration;
        return (-v0 - Mathf.Sqrt(v0 * v0 + 2 * a * height)) / a;
    }

    private void setVel(float x, float y) {
        physics.Vel = new Vector2(x, y);
	}

    private void fallUpdate() {
		float vel = physics.Vel.y;
		vel += Time.fixedDeltaTime * dropAcceleration;
        setVel(0.0f, vel);
	}
}
