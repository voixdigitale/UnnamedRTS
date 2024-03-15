using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform cameraTransform;
    public RectTransform minimapCameraTransform;
    public int sizeDelta = 120;

    private RectTransform minimapTransform;
    private Vector3 cameraOffset;
    private bool isDragging = false;

    void Awake()
    {
        minimapTransform = GetComponent<RectTransform>();
        cameraOffset = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapTransform, mousePosition, null, out Vector2 localPoint);
            localPoint.x = Mathf.Clamp(localPoint.x, -250, 250);
            localPoint.y = Mathf.Clamp(localPoint.y, -125, 125);
            Vector3 newPosition = new Vector3(localPoint.x * sizeDelta / minimapTransform.sizeDelta.x + cameraOffset.x, cameraTransform.position.y, localPoint.y * sizeDelta / minimapTransform.sizeDelta.y + cameraOffset.z);
            cameraTransform.position = newPosition;
            minimapCameraTransform.anchoredPosition = localPoint;
            cameraTransform.position = new Vector3(Mathf.Clamp(cameraTransform.position.x, -75, 45), cameraTransform.position.y, Mathf.Clamp(cameraTransform.position.z, -50, 4));
        }
        else
        {
            Vector2 newPosition = new Vector2(cameraTransform.position.x - cameraOffset.x, cameraTransform.position.z - cameraOffset.z);
            newPosition *= minimapTransform.sizeDelta.x / sizeDelta;
            minimapCameraTransform.anchoredPosition = newPosition;
        }
    }

    public void OnMouseUp()
    {
        isDragging = false;
    }

    public void OnMouseDown()
    {
        isDragging = true;
    }

}
