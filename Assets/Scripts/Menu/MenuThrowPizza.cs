using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuThrowPizza : MonoBehaviour {

    public GameObject pizzaPrefab;
    public Transform startThrowPosition;
    public Transform endThrowPosition;

    private void Start() {
        InvokeRepeating("Throw", 0f, 3f);
    }

    private void Throw() {
        GameObject pizza = Instantiate(pizzaPrefab, startThrowPosition.position, Quaternion.identity);
        pizza.GetComponent<PizzaController>().canMakeSound = false;
        pizza.GetComponent<Rigidbody>().AddForceAtPosition(startThrowPosition.forward * 16f, endThrowPosition.position, ForceMode.Impulse);
    }

}
