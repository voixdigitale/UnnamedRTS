using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ActionButton", menuName = "ScriptableObjects/Action Button")]
public class ActionButtonSO : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private KeyCode shortcutKey;
    [SerializeField] private UnitSO unitData;
    [SerializeField] private BuildingSO buildingData;

    public Sprite Icon => icon;
    public KeyCode ShortcutKey => shortcutKey;
    public UnitSO UnitData => unitData;
    public BuildingSO BuildingData => buildingData;
}
