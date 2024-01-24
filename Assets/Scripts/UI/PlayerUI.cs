using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    public PlayerController player;
    public static PlayerUI Instance { get; private set; }

    [Header("Resources")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI scrapText;
    public TextMeshProUGUI unitText;

    [Header("Action Buttons")]
    public GameObject buildMenu;
    public GameObject actionButtonPrefab;

    void Awake()
    {
        //Show the cursor (useful for debugging between windows)
        Cursor.visible = true;
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        PlayerController.OnResourceCollected += HandleOnResourceCollected;
        SelectionManager.OnSelectionChanged += HandleOnSelectionChanged;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        PlayerController.OnResourceCollected -= HandleOnResourceCollected;
        SelectionManager.OnSelectionChanged -= HandleOnSelectionChanged;
    }

    void Start()
    {
        if (!photonView.IsMine)
            return;

        player = GameManager.Instance.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
        woodText.text = player.GetResource(ResourceType.Wood).ToString();
        scrapText.text = player.GetResource(ResourceType.Scrap).ToString();
        unitText.text = player.GetUnitCount().ToString() + " / 10";
    }

    private void Update() {
        if (!photonView.IsMine)
            return;

        if (buildMenu.activeInHierarchy) {
            //Check if the playerController presses any of the keys assigned to the action buttons
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
                    button.GetComponent<ActionButton>().Setup(action, player);
                }
            }
            
        }
    }
    #nullable disable
}
