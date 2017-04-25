using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PizzeriaController : MonoBehaviour {
    public static PizzeriaController instance = null;
    public AudioSource audioSource;
    public AudioClip[] chefClips;
    public Text pizzeriaText;

    public GameObject pizzaPrefab;
    public Transform startThrowPosition;
    public Transform endThrowPosition;
    private int remainingPizzas = 4;

    private bool playerInTrigger = false;
    public BoxCollider triggerCollider;

    private void Start() {
        if (instance == null) {
            instance = this;

            audioSource.Stop();

            InvokeRepeating("AddMorePizzaOverTime", 0f, 2f);
            StartCoroutine(AddMorePizzaOverTime());

            pizzeriaText.text = "+" + remainingPizzas.ToString();
            LevelController.instance.onGameStart += OnGameStart;
        } else {
            DestroyImmediate(gameObject);
        }
    }

    private void Update() {
        pizzeriaText.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0f, 4f, 0f));
    }

    private IEnumerator AddMorePizzaOverTime() {
        while (true) {
            if (remainingPizzas < 4) {
                remainingPizzas++;
                pizzeriaText.text = "+" + remainingPizzas.ToString();
            }

            yield return new WaitForSeconds(Random.Range(4f, 7f));
        }
    }

    public void CallForPizza(int actualAmountOfPizza, out int amountOfPizza) {
        int pizzas = actualAmountOfPizza;

        if (playerInTrigger) {
            audioSource.clip = chefClips[Random.Range(0, chefClips.Length)];
            audioSource.Play();
            ThrowPizzaToPosition(remainingPizzas);

            pizzas = remainingPizzas + actualAmountOfPizza;

            if (pizzas > 4) {
                pizzas = 4;
            }

            remainingPizzas = 0;
            pizzeriaText.text = "0";
        }

        amountOfPizza = pizzas;
    }

    private void ThrowPizzaToPosition(int amount) {
        while (amount > 0) {
            GameObject pizza = Instantiate(pizzaPrefab, startThrowPosition.position, Quaternion.identity);
            pizza.GetComponent<PizzaController>().canMakeSound = false;
            pizza.GetComponent<Rigidbody>().AddForceAtPosition(startThrowPosition.forward * 8f, endThrowPosition.position, ForceMode.Impulse);
            amount--;
        }
    }

    public void InTrigger(bool trigger) {
        playerInTrigger = trigger;
    }

    private void OnGameStart() {
        remainingPizzas = 4;
        pizzeriaText.text = "+" + remainingPizzas.ToString();
    }
}
