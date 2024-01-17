using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ActionButton", menuName = "ScriptableObjects/Action Button")]
public class ActionButtonSO : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private KeyCode shortcutKey;

    public Sprite Icon => icon;
    public KeyCode ShortcutKey => shortcutKey;
}
