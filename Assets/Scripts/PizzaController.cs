using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PizzaController : MonoBehaviour {
    public float timeToDestroy = 5f;
    public AudioSource audioSource;
    public AudioClip throwClip;
    public AudioClip splashClip;

    private bool isFlying = false;
    public bool canMakeSound = true;

    private void Start() {
        Destroy(gameObject, timeToDestroy);

        if (canMakeSound) {
            audioSource.clip = throwClip;
            audioSource.Play();
            audioSource.volume = 0f;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.DOFade(0.8f, 1f);
        }

        isFlying = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (isFlying) {
            isFlying = false;

            if (canMakeSound) {
                audioSource.clip = splashClip;
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.Play();
            }
        }
    }
}
