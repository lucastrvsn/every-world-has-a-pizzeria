using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CarBehaviour : MonoBehaviour {

    private Rigidbody rigidBody;
    private bool isGrounded = false;
    private float distToGround = 0.4f;
    private int carLife = 4;
    private int amountOfPizza = 0;
    private SimpleCarController simpleCarController;

    public Transform spawnPosition;

    [Header("SFX Configuration")]
    public AudioSource audioSource;
    public AudioSource hornSource;
    public AudioClip[] hornClips;
    public AudioClip hitClip;
    public AudioSource getMorePizzasSource;
    public AudioClip[] getMorePizzasClips;

    [Header("UI Configuration")]
    public Image carDamageImage;
    public Sprite[] carDamageImages;
    public Image amountOfPizzaImage;
    public Sprite[] amountOfPizzaImages;

    [Header("Pizza Throw Configuration")]
    public GameObject pizzaPrefab;
    public Transform throwPositionLeft;
    public Transform throwPositionRight;
    public float throwForce = 10f;

    [Header("Car Explosion Configuration")]
    public ParticleSystem carExplosion;
    public AudioClip explosionClip;

    [Header("Car Smoke Configuration")]
    public Transform roodFrontPosition;
    public ParticleSystem carSmoke;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();

        carExplosion = Instantiate(carExplosion, transform.position, Quaternion.identity);
        carExplosion.Stop();
        carExplosion.gameObject.SetActive(false);

        carSmoke = Instantiate(carSmoke, roodFrontPosition.position, roodFrontPosition.rotation);
        carSmoke.Stop();
        carSmoke.gameObject.SetActive(false);

        LevelController.instance.onGameEnd += OnGameEnd;
        LevelController.instance.onGameEndNow += OnGameEndNow;
        LevelController.instance.onGameStart += OnGameStart;
        LevelController.instance.onGameStartLater += OnGameStartLater;

        simpleCarController = GetComponent<SimpleCarController>();
    }

    private void Update() {
        if (!LevelController.instance.hasGameEnded) {
            if (isGrounded && Vector3.Dot(transform.up, Vector3.up) < 0f) {
                transform.localRotation = Quaternion.identity;

                if (Mathf.Abs(Vector3.Dot(transform.right, Vector3.down)) > 0.825f) {
                    transform.localRotation = Quaternion.identity;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                ThrowPizza(throwPositionLeft.position, throwPositionLeft.forward);
            } else if (Input.GetKeyDown(KeyCode.E)) {
                ThrowPizza(throwPositionRight.position, throwPositionRight.forward);
            }

            if (Input.GetKeyDown(KeyCode.F) && !hornSource.isPlaying) {
                hornSource.clip = hornClips[Random.Range(0, hornClips.Length)];
                hornSource.Play();
                PizzeriaController.instance.CallForPizza(amountOfPizza, out amountOfPizza);
                UpdatePizzasImages();
            }

    #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M)) {
                LevelController.instance.GameOverHelper("EXPLODEEE!");
            }
    #endif

            CheckIfGrounded();
        }
    }

    private void UpdatePizzasImages() {
        if (amountOfPizza == 0) {
            amountOfPizzaImage.sprite = amountOfPizzaImages[0];

            getMorePizzasSource.clip = getMorePizzasClips[Random.Range(0, getMorePizzasClips.Length)];
            getMorePizzasSource.Play();

        } else if (amountOfPizza == 1) {
            amountOfPizzaImage.sprite = amountOfPizzaImages[1];
        } else if (amountOfPizza == 2) {
            amountOfPizzaImage.sprite = amountOfPizzaImages[2];
        } else if (amountOfPizza == 3) {
            amountOfPizzaImage.sprite = amountOfPizzaImages[3];
        } else if (amountOfPizza == 4) {
            amountOfPizzaImage.sprite = amountOfPizzaImages[4];
        }
    }

    private void UpdateCarDamangeImages() {
        if (carLife > 2) {
            carDamageImage.sprite = carDamageImages[2];
        } else if (carLife > 1) {
            carDamageImage.sprite = carDamageImages[1];
        } else if (carLife <= 1) {
            carDamageImage.sprite = carDamageImages[0];
        }
    }

    private void ThrowPizza(Vector3 position, Vector3 forward) {
        if (amountOfPizza > 0) {
            GameObject pizza = Instantiate(pizzaPrefab, position, Quaternion.identity);
            pizza.GetComponent<Rigidbody>().AddForce(forward * throwForce, ForceMode.Impulse);
            amountOfPizza--;
            UpdatePizzasImages();
        }
    }

    private void CheckIfGrounded() {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enviroment")) {

            if (rigidBody.velocity.magnitude > 1.5f) {
                CameraEffects.instance.Shake(0.3f, 1, 5, 90);
            } else if (rigidBody.velocity.magnitude > 0.7f) {
                CameraEffects.instance.Shake(0.2f, 0.7f, 3, 60);
            } else {
                CameraEffects.instance.Shake(0.2f, 0.3f, 1, 40);
            }

            audioSource.clip = hitClip;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();

            Debug.Log(rigidBody.velocity.magnitude);

            if (rigidBody.velocity.magnitude > 0.8f) {
                carLife--;

                UpdateCarDamangeImages();

                if (carLife <= 2) {
                    carSmoke.gameObject.SetActive(true);
                    carSmoke.transform.SetParent(roodFrontPosition);
                    carSmoke.transform.localPosition = new Vector3(0f, 0f, 0f);
                    carSmoke.Play();
                }

                if (carLife <= 0) {
                    LevelController.instance.GameOverHelper("Oh no! You exploded! :(");
                }
            }
        }
    }

    private void OnGameEnd() {
        gameObject.SetActive(false);
    }

    private void OnGameEndNow() {
        if (!LevelController.instance.endOutOfTime) {
            carExplosion.gameObject.SetActive(true);
            carExplosion.transform.position = transform.position;
            carExplosion.Play();

            // Tirando o audioSource do carro para ele nao ser
            // desativado junto com o carro.
            audioSource.clip = explosionClip;
            audioSource.Play();
            audioSource.transform.SetParent(null);

            gameObject.SetActive(false);
        }
    }

    private void OnGameStart() {
        carLife = 4;
        amountOfPizza = 0;
        UpdatePizzasImages();
        UpdateCarDamangeImages();

        transform.position = spawnPosition.position;
        transform.SetParent(spawnPosition);

        carExplosion.gameObject.SetActive(false);
        carExplosion.transform.position = transform.position;
        carExplosion.Play();

        audioSource.transform.SetParent(transform);

        gameObject.SetActive(true);
        transform.rotation = Quaternion.identity;

        carSmoke.Stop();
        carSmoke.gameObject.SetActive(false);
    }

    private void OnGameStartLater() {
        transform.SetParent(null);
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PizzeriaTrigger")) {
            PizzeriaController.instance.InTrigger(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PizzeriaTrigger")) {
            PizzeriaController.instance.InTrigger(false);
        }
    }
}
