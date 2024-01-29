using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/Building Data")]
public class BuildingSO : ScriptableObject
{
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    [SerializeField] List<ProductionCost> buildingCost;
    [SerializeField] float timeToProduce;
    [SerializeField] GameObject buildingPreview;
    [SerializeField] string buildingPath;

    public float Health { get => health; }
    public float MaxHealth { get => maxHealth; }
    public List<ProductionCost> BuildingCost { get => buildingCost; }
    public float TimeToProduce { get => timeToProduce; }
    public GameObject BuildingPreview { get => buildingPreview; }
    public string BuildingPath { get => buildingPath; }
}
