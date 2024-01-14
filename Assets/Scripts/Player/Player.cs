using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Units")]
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();

    private int wood, scrap;

    public bool IsPlayerUnit(Unit unit)
    {
        return units.Contains(unit);
    }

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
