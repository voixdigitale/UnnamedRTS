using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static event Action OnResourceCollected;

    [Header("Units")]
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();

    private int wood, scrap;

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.Scrap:
                scrap += amount;
                break;
        }
        OnResourceCollected?.Invoke();
    }

    public int GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood:
                return wood;
            case ResourceType.Scrap:
                return scrap;
        }

        return 0;
    }

    public int GetUnitCount()
    {
        return units.Count;
    }
}
