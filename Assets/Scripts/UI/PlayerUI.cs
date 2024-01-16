using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Player player;

    [Header("Resources")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI scrapText;
    public TextMeshProUGUI unitText;

    [Header("Action Buttons")]
    public GameObject buildMenu;
    public GameObject actionButtonPrefab;


    void OnEnable()
    {
        Player.OnResourceCollected += HandleOnResourceCollected;
        SelectionManager.OnSelectionChanged += HandleOnSelectionChanged;
    }

    void OnDisable()
    {
        Player.OnResourceCollected -= HandleOnResourceCollected;
        SelectionManager.OnSelectionChanged -= HandleOnSelectionChanged;
    }

    void Start()
    {
        woodText.text = player.GetResource(ResourceType.Wood).ToString();
        scrapText.text = player.GetResource(ResourceType.Scrap).ToString();
        unitText.text = player.GetUnitCount().ToString() + " / 10";
    }

    private void Update() {
        if (buildMenu.active) {
            //Check if the player presses any of the keys assigned to the action buttons
            foreach (Transform child in buildMenu.transform) {

                ActionButton button = child.GetComponent<ActionButton>();

                if (Input.GetKeyDown(button.GetShortcutKey())) {
                    child.GetComponent<ActionButton>().OnClick();
                }
            }
        }
    }
    void HandleOnResourceCollected()
    {
        woodText.text = player.GetResource(ResourceType.Wood).ToString();
        scrapText.text = player.GetResource(ResourceType.Scrap).ToString();
    }

    #nullable enable
    void HandleOnSelectionChanged(ISelectable? selection) {

        foreach (Transform child in buildMenu.transform) {
            Destroy(child.gameObject);
        }

        if (selection == null) {
            buildMenu.SetActive(false);
        } else {
            buildMenu.SetActive(true);
            if (selection is Unit) {
                foreach (ActionButtonSO action in ((Unit)selection).ActionButtons) {
                    GameObject button = Instantiate(actionButtonPrefab, buildMenu.transform);
                    button.GetComponent<ActionButton>().Setup(action);
                }
            }
            
        }
    }
    #nullable disable
}
