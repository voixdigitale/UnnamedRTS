using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float moveSpeed = 10f;
    public float zoomSpeed = 20f;

    public float edgeSize = 10f;

    public float minZoom = 5f;
    public float maxZoom = 200f;

    float minLimitX = -75;
    float maxLimitX = 45;
    float minLimitY = -50;
    float maxLimitY = 4;

    
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
        #if UNITY_EDITOR
                if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1) return;
        #else
            if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) return;
        #endif

        float horizontal = 0;
        float vertical = 0;
        Vector3 position = transform.position;

        if (Input.mousePosition.x < edgeSize && position.x > minLimitX)
        {
            horizontal -= 1;
        } else if (Input.mousePosition.x > Screen.width - edgeSize && position.x < maxLimitX)
        {
            horizontal += 1;
        }

        if (Input.mousePosition.y < edgeSize && position.z > minLimitY)
        {
            vertical -= 1;
        } else if (Input.mousePosition.y > Screen.height - edgeSize && position.z < maxLimitY)
        {
            vertical += 1;
        }

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
