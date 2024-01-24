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

    public void Setup(ActionButtonSO config, PlayerController playerController) {
        iconImage.sprite = config.Icon;
        shortcutText.text = config.ShortcutKey.ToString();
        methodName = config.name + "Command";
        shortcutKey = config.ShortcutKey;
        this._playerController = playerController;
        commandManager = playerController.GetComponent<CommandManager>();
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

        Command command = (Command)Activator.CreateInstance(commandType, new System.Object[] { _playerController });

        commandManager.SetCommand(command);
    }



    public KeyCode GetShortcutKey() {
        return shortcutKey;
    }
}
