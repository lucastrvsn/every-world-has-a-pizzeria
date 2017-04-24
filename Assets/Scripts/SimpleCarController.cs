using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class SimpleCarController : MonoBehaviour {
    [Header("Car Configuration")]
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float maxBreakTorque;

    [Header("Sounds Configuration")]
    public AudioSource motorIdle;
    public AudioSource motorTorque;

    private void Start() {
        motorIdle.DOFade(1f, 1f);
        motorTorque.Stop();
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
        if (collider.transform.childCount != 0) {
            Transform visualWheel = collider.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }
    }

    private bool isMotor = false;

    public void FixedUpdate() {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float breaking = maxBreakTorque * Input.GetAxis("Jump");

        if (motor != 0f) {
            isMotor = true;
        } else {
            isMotor = false;
        }

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor) {
                if (Mathf.Abs(axleInfo.leftWheel.rpm) > 1000 || Mathf.Abs(axleInfo.rightWheel.rpm) > 1000) {
                    axleInfo.leftWheel.motorTorque = 0f;
                    axleInfo.rightWheel.motorTorque = 0f;
                } else {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;

                }
                
                axleInfo.leftWheel.brakeTorque = breaking;
                axleInfo.rightWheel.brakeTorque = breaking;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        AudioController();
    }

    private void AudioController() {
        if (isMotor && !motorTorque.isPlaying) {
            motorTorque.Play();
            motorTorque.pitch = Random.Range(0.8f, 1.2f);
            motorIdle.DOFade(0f, 1f).OnComplete(() => {
                motorIdle.Stop();
            });
            motorTorque.DOFade(0.2f, 4f);
        } else if (!isMotor && !motorIdle.isPlaying) {
            motorIdle.Play();
            motorIdle.pitch = Random.Range(0.95f, 1.05f);
            motorIdle.DOFade(0.2f, 1f);
            motorTorque.DOFade(0f, 1f).OnComplete(() => {
                motorTorque.Stop();
            });
        }
    }
}
