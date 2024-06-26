using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    public GameObject popup;
    public TextMeshProUGUI quantityText;
    
    private Resource resource;

    void OnEnable()
    {
        Resource.OnResourceHarvested += HandleOnResourceCollected;
    }

    void OnDisable()
    {
        Resource.OnResourceHarvested -= HandleOnResourceCollected;
    }

    void Awake()
    {
        resource = GetComponentInParent<Resource>();
    }

    void OnMouseEnter()
    {
        quantityText.text = resource.quantity.ToString();
        popup.SetActive(true);
    }

    void OnMouseExit()
    {
        popup.SetActive(false);
    }

    void HandleOnResourceCollected()
    {
        quantityText.text = resource.quantity.ToString();
    }
}
