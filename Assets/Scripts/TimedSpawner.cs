using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimedSpawner : JComponent {
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private float timeBetweenSpawns = 1.0f;

    protected override void onStart() {
        TimerManager.Instance.Every(timeBetweenSpawns, spawn);
    }

    private void spawn() {
        var obj = GameObject.Instantiate(prefabToSpawn);
        obj.transform.position = transform.position;
    }
}
