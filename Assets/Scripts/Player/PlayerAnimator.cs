using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAnimator : JComponent {
    [SerializeField] private float shootRaiseGunTime = 0.6f;
    [SerializeField] private float shootEyeShutTime = 0.2f;

    [StartComponent] private SpritePhysics physics;
    [StartComponent] private tk2dSpriteAnimator animator;

    private InputManager input;

    private bool isRunning = false;
    private bool isShooting = false;
    private bool isEyeShut = false;
    private bool isJumping = false;
    private bool isFalling = false;

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
        // TODO: re-enable this when we have more than one level
        //isEyeShut = shotTimer > totalShootTime() - shootEyeShutTime;
        isJumping = physics.Vel.y * Physics2D.gravity.y < 0;
        isFalling = physics.Vel.y * Physics2D.gravity.y > 0;
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
        if (isJumping && isShooting) {
            return "Shoot Jump";
        } else if (isJumping) {
            return "Jump";
        } else if (isFalling && isShooting) {
            return "Shoot Fall";
        } else if (isFalling) {
            return "Fall";
        } else if (isRunning && isShooting && isEyeShut) {
            return "Shut Shoot Run";
        } else if (isRunning && isShooting) {
            return "Shoot Run";
        } else if (isRunning) {
            return "Run";
        } else if (isShooting && isEyeShut) {
            return "Shut Shoot Stand";
        } else if (isShooting) {
            return "Shoot Stand";
        } else {
            return "Idle";
        }
    }
}
