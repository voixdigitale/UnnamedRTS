using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public Player player;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI scrapText;
    public TextMeshProUGUI unitText;


    void OnEnable()
    {
        Resource.OnResourceCollected += HandleOnResourceCollected;
    }

    void OnDisable()
    {
        Resource.OnResourceCollected -= HandleOnResourceCollected;
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
}
