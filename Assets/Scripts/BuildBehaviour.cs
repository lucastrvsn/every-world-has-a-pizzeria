using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BuildBehaviour : MonoBehaviour {
    public bool canToggleMaterial = false;
    public Renderer myRenderer;
    public bool isToggle = false;
    public bool isTarget = false;

    public CanvasGroup thanksGroup;

    public Transform arrowPosition;

    public AudioSource audioSource;
    public AudioClip[] thanksClips;

    private void OnTriggerEnter(Collider other) {
        if (isTarget && other.CompareTag("Pizza")) {
            LevelController.instance.CompleteDelivery();
            Thanks();
        }
    }

    public void ToggleGreyscale() {
        if (canToggleMaterial) {
            isToggle = !isToggle;
            myRenderer.material.DOFloat(isToggle ? 0f : 1f, "_Range", 1f);
        }
    }

    private void Thanks() {
        thanksGroup.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 3f, 0f));
        thanksGroup.DOFade(1f, 0.3f).OnComplete(() => {
            thanksGroup.DOFade(0f, 0.2f);
        });
        DOVirtual.Float(3f, 5f, 0.3f, (a) => {
            thanksGroup.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, a, 0f));
        });

        audioSource.clip = thanksClips[Random.Range(0, thanksClips.Length)];
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }
}
