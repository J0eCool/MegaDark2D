using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : SingletonComponent<TimerManager> {
    private List<Timer> timers = new List<Timer>();

	void FixedUpdate() {
        foreach (Timer timer in timers) {
            timer.Update();
        }
	}

    public Timer Add(Timer timer) {
        timers.Add(timer);
        return timer;
    }

    public void Remove(Timer timer) {
        timers.Remove(timer);
    }

    public Timer After(float time, Timer.TimerFinishCallback callback) {
        return Add(new SingleTimer(time, callback));
    }

    public Timer Every(float time, Timer.TimerFinishCallback callback) {
        return Add(new RepeatTimer(time, callback));
    }
}
