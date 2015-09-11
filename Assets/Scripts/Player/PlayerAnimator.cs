using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimator : JComponent {
    [StartComponent] private SpritePhysics physics;
    [StartComponent] private tk2dSpriteAnimator animator;

    private InputManager input;

    private bool isFiringAnimationPlaying = false;

    protected override void onStart() {
        input = InputManager.Instance;
	}

	void Update() {
        if (input.Shoot.DidPress) {
            animator.Stop();
            animator.Play("Shoot");
            isFiringAnimationPlaying = true;
            animator.AnimationCompleted = (x, y) => {
                isFiringAnimationPlaying = false;
                animator.AnimationCompleted = null;
            };
        }

        if (isFiringAnimationPlaying) {
            return;
        }

        if (Mathf.Abs(physics.Vel.x) > 0.1f) {
            animator.Play("Run");
        } else {
            animator.Play("Idle");
        }
    }
}
