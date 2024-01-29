using System;
using UnityEngine;
using UnityEngine.Events;

public enum ResourceType {
    None,
    Wood,
    Scrap
}

public abstract class Resource : MonoBehaviour
{
    protected ResourceType type;
    
    public float quantity { get; protected set; }

    public static event Action OnResourceHarvested;

    public void Collect(float amount, Gatherer targetUnit)
    {
        quantity -= amount;

        float amountToGive = amount;

        if (quantity < 0)
            amountToGive = amount + quantity;

        targetUnit.backpackResource = type;
        targetUnit.backpackQuantity += (int)amountToGive;

        if (quantity <= 0)
            Destroy(gameObject);

        OnResourceHarvested?.Invoke();
    }
}
