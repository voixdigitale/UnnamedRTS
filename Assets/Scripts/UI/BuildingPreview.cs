using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;
    [Header("Collider")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 size;
    [SerializeField] private LayerMask layerMask;

    private bool validPlacement;

    private void SetValid(bool valid)
    {
        renderer.materials[0].SetColor("_BaseColor", valid ? validColor : invalidColor);
    }

    void Update()
    {
        validPlacement = !Physics.CheckBox(new Vector3(transform.position.x + positionOffset.x, transform.position.y + positionOffset.y, transform.position.z + positionOffset.z),
            new Vector3(size.x / 2, size.y / 2, size.z / 2), Quaternion.identity, layerMask);

        SetValid(validPlacement);
    }

    public bool IsValid()
    {
        return validPlacement;
    }
}

