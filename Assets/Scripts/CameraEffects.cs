using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraEffects : MonoBehaviour {
    public static CameraEffects instance = null;
    private bool isShaking = false;

    private void Start() {
        if (instance == null) {
            instance = this;
        } else {
            DestroyImmediate(gameObject);
        }
    }

    public void Shake(float duration = 1f, float strength = 1f, int vibrato = 5, float randomness = 90) {
        if (!isShaking) {
            isShaking = true;
            transform.DOShakePosition(duration, strength, vibrato, randomness).OnComplete(() => {
                isShaking = false;
            });
        }
    }
}
