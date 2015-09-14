using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Timer : Updateable {
    public delegate void TimerFinishCallback();

    protected float time;
    protected TimerFinishCallback callback = null;

    private bool didFinish = false;
    private bool wasFinished = false;

    protected Timer(TimerFinishCallback callback) {
        this.callback = callback;
    }

    public void Update() {
        time -= Time.fixedDeltaTime;
        updateFinish();
    }

    private void updateFinish() {
        wasFinished = didFinish;
        didFinish = time <= 0.0f;

        if (didFinish && !wasFinished) {
            handleFinish();
        }
    }

    private void handleFinish() {
        onFinish();

        if (callback != null) {
            callback();
        }
    }

    protected virtual void onFinish() { }

    public bool IsDone() {
        return didFinish;
    }
}

public class SingleTimer : Timer {
    public SingleTimer(float delay, TimerFinishCallback callback = null)
            : base(callback) {
        time = delay;
    }
}

public class RepeatTimer : Timer {
    private float interval;

    public RepeatTimer(float interval, TimerFinishCallback callback = null)
            : base(callback) {
        time = interval;
        this.interval = interval;
    }

    protected override void onFinish() {
        time = interval;
    }
}

