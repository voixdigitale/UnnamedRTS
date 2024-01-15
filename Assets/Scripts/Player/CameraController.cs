using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float moveSpeed = 10f;
    public float zoomSpeed = 20f;

    public float minZoom = 5f;
    public float maxZoom = 200f;

    private new Camera camera;

    void Awake() {
        camera = Camera.main;
    }

    void MoveCamera() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 position = transform.position;
        position.x += horizontal * moveSpeed * Time.deltaTime;
        position.z += vertical * moveSpeed * Time.deltaTime;

        transform.position = position;
    }

    void ZoomCamera() {
        float zoom = Input.GetAxis("Mouse ScrollWheel");

        Vector3 position = camera.transform.position;
        position.y -= zoom * zoomSpeed * Time.deltaTime;
        position.y = Mathf.Clamp(position.y, minZoom, maxZoom);

        camera.transform.position = position;
    }

    void Update() {
        MoveCamera();
        ZoomCamera();
    }
}
