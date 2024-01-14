using System;
using UnityEngine;
using UnityEngine.Events;

public enum ResourceType {
    Wood,
    Scrap
}

public abstract class Resource : MonoBehaviour
{
    protected ResourceType type;
    public float quantity { get; protected set; }

    public static event Action OnResourceCollected;

    public void Collect(float amount, Player targetPlayer)
    {
        quantity -= amount;

        float amountToGive = amount;

        if (quantity < 0)
            amountToGive = amount + quantity;

        targetPlayer.AddResource(type, (int)amountToGive);

        if (quantity <= 0)
            Destroy(gameObject);

        OnResourceCollected?.Invoke();
    }
}
