using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelShaking : MonoBehaviour {
    public Transform level;

    private bool canShake = true;
    private float timeToShake = 10f;
    private float time = 0f;

    private void Start() {
        LevelController.instance.onGameEnd += OnGameEnd;
        LevelController.instance.onGameStart += OnGameStart;
        timeToShake = RandomizeTime();
    }

    private void Update() {
        if (canShake) {
        
            if (time > timeToShake) {
                timeToShake = RandomizeTime();
                Shake();
                time = 0f;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.L)) {
                level.DORotate(new Vector3(Random.Range(-8f, 8f), Random.Range(-2f, 2f), Random.Range(-8f, 8f)), 0.4f);
            }
#endif

            time += Time.deltaTime;

        }
    }

    private void Shake() {
        level.DORotate(new Vector3(Random.Range(-5f, 5f), Random.Range(-2f, 2f), Random.Range(-5f, 5f)), 2f);
    }

    private float RandomizeTime() {
        return Random.Range(8f, 25f);
        //return Random.Range(1f, 2f);
        //return 0.2f;
    }

    private void OnGameStart() {
        level.DORotate(new Vector3(0f, 0f, 0f), 2f);
        canShake = true;

        Camera.main.GetComponent<CameraSmoothFollow>().target = GameObject.FindGameObjectWithTag("Player").transform;
        Camera.main.GetComponent<CameraSmoothFollow>().offset = new Vector3(5f, 5f, -5f);
    }

    private void OnGameEnd() {
        level.DORotate(new Vector3(0f, 45f, 0f), 2f);
        canShake = false;

        Camera.main.GetComponent<CameraSmoothFollow>().target = level;
        Camera.main.GetComponent<CameraSmoothFollow>().offset = new Vector3(5f, 10f, -5f);
    }
}
