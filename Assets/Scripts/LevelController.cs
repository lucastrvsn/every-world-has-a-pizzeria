using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {
    public static LevelController instance = null;

    public Camera mainCamera;
    public Camera xrayCamera;
    public LayerMask cameraLayerMask;
    public LayerMask cameraLayerMaskGlass;
    public AudioSource windAudioSource;
    public AudioSource musicAudioSource;
    public AudioSource musicElevatorAudioSource;
    public GameObject[] thingsToDisable;
    public CanvasGroup gameOverOverlay;
    public Text playerPointsText;

    private List<BuildBehaviour> builds = new List<BuildBehaviour>();
    private BuildBehaviour buildActual = null;
    private int numberOfDeliveries = 0;
    private float timeToDelivery = 20f;
    private float time = 0f;

    public delegate void OnGameEnd();
    public OnGameEnd onGameEnd;
    public delegate void OnGameEndNow();
    public OnGameEndNow onGameEndNow;
    public delegate void OnGameStart();
    public OnGameStart onGameStart;
    public delegate void OnGameStartLater();
    public OnGameStartLater onGameStartLater;

    private bool hasGameEnded = false;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            DestroyImmediate(gameObject);
        }
    }

    private void Start() {
        GameObject[] buildsGO = GameObject.FindGameObjectsWithTag("Build");
        foreach (GameObject b in buildsGO) {
            builds.Add(b.GetComponent<BuildBehaviour>());
        }

        RandomizePlaceToDelivery();
    }

    private void FixedUpdate() {
        time += Time.deltaTime;
        if (time >= timeToDelivery) {
            // Game over happen!
            Debug.LogError("GAME OVER");
        }
    }

    public void CompleteDelivery() {
        ToggleBuild(buildActual);
        buildActual.isTarget = false;
        RandomizePlaceToDelivery();
        numberOfDeliveries++;
        time = 0f;
    }

    private void RandomizePlaceToDelivery() {
        BuildBehaviour b = builds[Random.Range(0, builds.Count)];

        if (buildActual == b) {
            b = builds[Random.Range(0, builds.Count)];
        }

        buildActual = b;
        buildActual.isTarget = true;
        ToggleBuild(buildActual);
    }

    private void ToggleBuild(BuildBehaviour build) {
        build.ToggleGreyscale();
    }

    public void GameOverHelper() {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver() {
        if (onGameEndNow != null) {
            onGameEndNow();
        }

        yield return new WaitForSeconds(2);

        hasGameEnded = true;
        if (onGameEnd != null) {
            onGameEnd();
        }

        for (int i = 0; i < thingsToDisable.Length; i++) {
            thingsToDisable[i].SetActive(false);
        }

        mainCamera.cullingMask = cameraLayerMask;
        mainCamera.DOOrthoSize(65f, 2f);
        xrayCamera.DOOrthoSize(65f, 2f);

        ShowGameOverOverlay();

        mainCamera.DOColor(new Color(0.071f, 0.041f, 0.025f), 2f);
        windAudioSource.DOFade(0f, 2f);
        musicAudioSource.DOFade(0f, 2f);

        musicElevatorAudioSource.Play();
        musicElevatorAudioSource.DOFade(0.02f, 2f);
    }

    public void RestartGame() {
        mainCamera.DOOrthoSize(12f, 2f).OnComplete(() => {
            mainCamera.cullingMask = cameraLayerMaskGlass;

            for (int i = 0; i < thingsToDisable.Length; i++) {
                thingsToDisable[i].SetActive(true);
            }

            if (onGameStartLater != null) {
                onGameStartLater();
            }
        });

        xrayCamera.DOOrthoSize(12f, 2f);

        windAudioSource.DOFade(0.5f, 2f);
        musicAudioSource.DOFade(0.05f, 2f);

        musicElevatorAudioSource.DOFade(0f, 2f).OnComplete(() => {
            musicElevatorAudioSource.Stop();
        });

        mainCamera.DOColor(new Color(0.105f, 0.236f, 1f), 2f);

        if (onGameStart != null) {
            onGameStart();
        }

        HideGameOverOverlay();
    }

    private void HideGameOverOverlay() {
        Vector3 gameOverlayOriginPosition = gameOverOverlay.transform.position;

        gameOverOverlay.transform.DOMove(gameOverOverlay.transform.position + new Vector3(0f, -100f, 0f), 1f).OnComplete(() => {
            gameOverOverlay.transform.position = gameOverlayOriginPosition;
        });
        gameOverOverlay.DOFade(0f, 1f);
    }

    private void ShowGameOverOverlay() {
        playerPointsText.text = numberOfDeliveries.ToString();

        Vector3 gameOverlayOriginPosition = gameOverOverlay.transform.position;
        gameOverOverlay.transform.position += new Vector3(0f, -100f, 0f);
        gameOverOverlay.transform.DOMove(gameOverlayOriginPosition, 1f);
        gameOverOverlay.DOFade(1f, 2f);
    }

    public void BackToMenuButton() {
        SceneManager.LoadScene(0);
    }
}
