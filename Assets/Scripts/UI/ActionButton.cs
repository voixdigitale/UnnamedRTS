using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI shortcutText;

    private string methodName;
    private KeyCode shortcutKey;
    private PlayerController _playerController;
    private CommandManager commandManager;
    private UnitSO unitData;
    private BuildingSO buildingData;
    private Building selectedBuilding;

    public void Setup(ActionButtonSO config, PlayerController playerController, ISelectable selection = null) {
        iconImage.sprite = config.Icon;
        shortcutText.text = config.ShortcutKey.ToString();
        methodName = (config.UnitData is null ? config.name : "BuildUnit") + "Command";
        shortcutKey = config.ShortcutKey;
        this._playerController = playerController;
        commandManager = playerController.GetComponent<CommandManager>();
        unitData = config.UnitData;
        buildingData = config.BuildingData;
        if (selection is not null and Building)
        {
            selectedBuilding = (Building)selection;
        }
    }

    public void OnClick() {
        if (string.IsNullOrEmpty(methodName)) {
            Debug.LogError("Method name is null or empty.");
            return;
        }

        var commandType = Type.GetType(methodName);
        if (commandType == null) {
            Debug.LogError($"Type {methodName} does not exist.");
            return;
        }

        Command command;

        if (buildingData != null) {
            command = (Command)Activator.CreateInstance(commandType, new object[] { _playerController, buildingData });
        } else if (unitData != null) {
            command = (Command)Activator.CreateInstance(commandType, new object[] { _playerController, unitData, selectedBuilding });
        } else { 
            command = (Command)Activator.CreateInstance(commandType, new object[] { _playerController });
        }

        Debug.Log($"Executing command {command.GetType().Name}.");
        commandManager.SetCommand(command);
    }



    public KeyCode GetShortcutKey() {
        return shortcutKey;
    }
}
