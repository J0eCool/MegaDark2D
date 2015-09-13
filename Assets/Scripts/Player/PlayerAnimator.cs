using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimator : JComponent {
    [SerializeField] private float shootRaiseGunTime = 0.6f;
    [SerializeField] private float shootEyeShutTime = 0.2f;

    [StartComponent] private SpritePhysics physics;
    [StartComponent] private tk2dSpriteAnimator animator;

    private InputManager input;

    private bool isFiringAnimationPlaying = false;

    private bool isRunning = false;
    private bool isShooting = false;
    private bool isEyeShut = false;

    private float shotTimer = -1.0f;

    protected override void onStart() {
        input = InputManager.Instance;
	}

	void FixedUpdate() {
        updateTimers();
        updateFlags();
        updateAnimation();
    }

    private void updateTimers() {
        shotTimer -= Time.fixedDeltaTime;

        if (input.Shoot.DidPress) {
            shotTimer = totalShootTime();
        }
    }

    private float totalShootTime() {
        return shootRaiseGunTime + shootEyeShutTime;
    }

    private void updateFlags() {
        isRunning = Mathf.Abs(physics.Vel.x) > 0.1f;
        isShooting = shotTimer > 0.0f;
        isEyeShut = shotTimer > totalShootTime() - shootEyeShutTime;
    }

    private void updateAnimation() {
        string animName = currentAnimationName();
        var clipToPlay = animator.GetClipByName(animName);
        if (animator.CurrentClip != clipToPlay) {
            int frame = animator.CurrentFrame % clipToPlay.frames.Length;
            animator.PlayFromFrame(clipToPlay, frame);
        }
    }

    private string currentAnimationName() {
        if (isRunning && isShooting && isEyeShut) {
            return "Shut Shoot Run";
        }
        if (isRunning && isShooting) {
            return "Shoot Run";
        }
        if (isRunning) {
            return "Run";
        }

        if (isShooting && isEyeShut) {
            return "Shut Shoot Stand";
        }
        if (isShooting) {
            return "Shoot Stand";
        }
        return "Idle";
    }
}
