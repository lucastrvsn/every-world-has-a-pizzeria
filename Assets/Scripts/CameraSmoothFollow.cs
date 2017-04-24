using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour {
    public Transform target;
    public float speed;
    public Vector3 offset;

    private void Start() {
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void FixedUpdate() {
        if (target != null) {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, speed * Time.deltaTime);
        } else {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

}
