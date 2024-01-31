using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float moveSpeed = 10f;
    public float zoomSpeed = 20f;

    public float minZoom = 5f;
    public float maxZoom = 200f;

    private new Camera camera;
    private Vector3 cameraOffset;

    void Awake() {
        camera = Camera.main;
        cameraOffset = camera.transform.position - transform.position;
    }

    void MoveCamera() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 position = transform.position;
        position.x += horizontal * moveSpeed * Time.deltaTime;
        position.z += vertical * moveSpeed * Time.deltaTime;

        transform.position = position;
    }

    // Move the camera by moving the mouse to the edges of the screen
    void MoveCameraWithMouse()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.mousePosition.x < 5)
        {
            horizontal -= 1;
        } else if (Input.mousePosition.x > Screen.width - 5)
        {
            horizontal += 1;
        }

        if (Input.mousePosition.y < 5)
        {
            vertical -= 1;
        } else if (Input.mousePosition.y > Screen.height - 5)
        {
            vertical += 1;
        }

        Vector3 position = transform.position;
        position.x += horizontal * moveSpeed * Time.deltaTime;
        position.z += vertical * moveSpeed * Time.deltaTime;

        transform.position = position;
    }


    void ZoomCamera() {
        float zoom = Input.GetAxis("Mouse ScrollWheel");

        Vector3 position = camera.transform.position;
        Vector3 direction = camera.transform.forward;
        position = position + direction * zoom * zoomSpeed * Time.deltaTime;
        position.y = Mathf.Clamp(position.y, minZoom, maxZoom);

        camera.transform.position = position;
    }

    void Update()
    {
        MoveCameraWithMouse();
        //MoveCamera();
        ZoomCamera();
    }

    public void MoveCameraTo(Vector3 position) {
        transform.position = position - new Vector3(cameraOffset.x, 0, cameraOffset.z + 10f);
    }
}
