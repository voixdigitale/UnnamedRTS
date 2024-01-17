using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI shortcutText;

    private string methodName;
    private KeyCode shortcutKey;
    private Player player;
    private CommandManager commandManager;

    public void Setup(ActionButtonSO config, Player player) {
        iconImage.sprite = config.Icon;
        shortcutText.text = config.ShortcutKey.ToString();
        methodName = config.MethodName;
        shortcutKey = config.ShortcutKey;
        this.player = player;
        commandManager = player.GetComponent<CommandManager>();
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

        Command command = (Command)Activator.CreateInstance(commandType);
        commandManager.SetCommand(command);
    }



    public KeyCode GetShortcutKey() {
        return shortcutKey;
    }
}
