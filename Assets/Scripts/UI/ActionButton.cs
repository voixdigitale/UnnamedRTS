using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI shortcutText;
    
    private string methodName;
    private KeyCode shortcutKey;

    public void Setup(ActionButtonSO config) {
        iconImage.sprite = config.Icon;
        shortcutText.text = config.ShortcutKey.ToString();
        methodName = config.MethodName;
        shortcutKey = config.ShortcutKey;
    }

    public void OnClick() {
        SelectionManager.Instance.currentSelection.ForEach(x => {
            //Check if method exists
            if (x.GetType().GetMethod(methodName) == null) {
                Debug.LogError($"Method {methodName} does not exist on {x.GetType()}");
                return;
            }
            x.GetType().GetMethod(methodName).Invoke(x, null);
        });
    }

    public KeyCode GetShortcutKey() {
        return shortcutKey;
    }
}
