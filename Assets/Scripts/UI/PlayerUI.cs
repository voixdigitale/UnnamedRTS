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
    void HandleOnResourceCollected()
    {
        woodText.text = player.GetResource(ResourceType.Wood).ToString();
        scrapText.text = player.GetResource(ResourceType.Scrap).ToString();
    }

    #nullable enable
    void HandleOnSelectionChanged(ISelectable? selection) {
        if (selection == null) {
            buildMenu.SetActive(false);
        } else {
            buildMenu.SetActive(true);
        }
    }
    #nullable disable
}
