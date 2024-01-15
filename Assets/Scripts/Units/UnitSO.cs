using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UnitCost {
    public ResourceType Resource;
    public int Amount;
}

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitDataScriptableObject")]
public class UnitSO : ScriptableObject
{
    [Header("General")]
    [SerializeField] string unitName;
    [SerializeField] float moveSpeed;
    [SerializeField] float health;
    [SerializeField] float maxHealth;
    [SerializeField] List<UnitCost> unitCost;

    [Header("Gathering")]
    [SerializeField] float gatherAmount;
    [SerializeField] float gatherRate;

    [Header("Combat")]
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] float attackRate;

    [Header("UI")]
    [SerializeField] Sprite unitIcon;
    [SerializeField] KeyCode hotkey;

    public string UnitName { get => unitName; }
    public float MoveSpeed { get => moveSpeed; }
    public float Health { get => health; }
    public float MaxHealth { get => maxHealth; }
    public List<UnitCost> UnitCost { get => unitCost; }
    public float GatherAmount { get => gatherAmount; }
    public float GatherRate { get => gatherRate; }
    public float AttackRange { get => attackRange; }
    public float AttackDamage { get => attackDamage; }
    public float AttackRate { get => attackRate; }
    public Sprite UnitIcon { get => unitIcon; }
    public KeyCode Hotkey { get => hotkey; }
}
