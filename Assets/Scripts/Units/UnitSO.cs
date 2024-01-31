using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ProductionCost {
    public ResourceType Resource;
    public int Amount;
}

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/Unit Data")]
public class UnitSO : ScriptableObject
{
    [Header("General")]
    [SerializeField] string unitName;
    [SerializeField] float moveSpeed;
    [SerializeField] int health;
    [SerializeField] int maxHealth;
    [SerializeField] List<ProductionCost> unitCost;
    [SerializeField] float timeToProduce;
    [SerializeField] GameObject unitPrefab;

    [Header("Gathering")]
    [SerializeField] float gatherAmount;
    [SerializeField] float gatherRate;

    [Header("Combat")]
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage;
    [SerializeField] float attackRate;

    [Header("UI")]
    [SerializeField] Sprite unitIcon;
    [SerializeField] KeyCode hotkey;

    public string UnitName { get => unitName; }
    public float MoveSpeed { get => moveSpeed; }
    public int Health { get => health; }
    public int MaxHealth { get => maxHealth; }
    public List<ProductionCost> UnitCost { get => unitCost; }
    public GameObject UnitPrefab { get => unitPrefab; }
    public float GatherAmount { get => gatherAmount; }
    public float GatherRate { get => gatherRate; }
    public float AttackRange { get => attackRange; }
    public int AttackDamage { get => attackDamage; }
    public float AttackRate { get => attackRate; }
    public Sprite UnitIcon { get => unitIcon; }
    public KeyCode Hotkey { get => hotkey; }
    public float TimeToProduce { get => timeToProduce; }
}
