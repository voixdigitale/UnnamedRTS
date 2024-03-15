using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerUI : MonoBehaviour
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

    [Header("Messaging")]
    public TextMeshProUGUI errorMessage;

    [Header("Floating Text")]
    public GameObject floatingTextPrefab;

    void Awake()
    {
        //Show the cursor (useful for debugging between windows)
        Cursor.visible = true;
        Instance = this;
    }

    public void OnEnable()
    {
        PlayerController.OnResourceCollected += HandleOnResourceCollected;
        SelectionManager.OnSelectionChanged += HandleOnSelectionChanged;
        Unit.OnDamageTaken += ShowFloatingText;
    }

    public void OnDisable()
    {
        PlayerController.OnResourceCollected -= HandleOnResourceCollected;
        SelectionManager.OnSelectionChanged -= HandleOnSelectionChanged;
        Unit.OnDamageTaken -= ShowFloatingText;
    }

    public void Initialize(PlayerController player) {
        this.player = player;
        woodText.text = player.GetResource(ResourceType.Wood).ToString();
        scrapText.text = player.GetResource(ResourceType.Scrap).ToString();
        unitText.text = player.GetUnitCount().ToString() + " / 10";
    }

    private void Update() {
        if (buildMenu.activeInHierarchy) {
            //Check if the playerController presses any of the keys assigned to the action buttons
            foreach (Transform child in buildMenu.transform) {

                ActionButton button = child.GetComponent<ActionButton>();

                if (Input.GetKeyUp(button.GetShortcutKey())) {
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

        if (selection is Unit) {
            buildMenu.SetActive(true);
            foreach (ActionButtonSO action in ((Unit)selection).ActionButtons) {
                GameObject button = Instantiate(actionButtonPrefab, buildMenu.transform);
                button.GetComponent<ActionButton>().Setup(action, player);
            }
        } else if (selection is Building)  {
            buildMenu.SetActive(true);
            foreach (ActionButtonSO action in ((Building)selection).ActionButtons)
            {
                GameObject button = Instantiate(actionButtonPrefab, buildMenu.transform);
                button.GetComponent<ActionButton>().Setup(action, player, selection);
            }
        } else {
            buildMenu.SetActive(false);
        }
    }
    #nullable disable

    public void ShowErrorMessage(string message)
    {
        errorMessage.text = message;
        errorMessage.canvasRenderer.SetAlpha(1f);
        StartCoroutine(Instance.ClearErrorMessage());
    }

    private IEnumerator ClearErrorMessage()
    {
        yield return new WaitForSeconds(2f);
        errorMessage.CrossFadeAlpha(0f, 1f, false);
    }

    public void ShowFloatingText(Unit unit, int amount)
    {
        GameObject floatingTextObj = Instantiate(floatingTextPrefab, unit.transform.position, Quaternion.identity);
        FloatingText floatingText = floatingTextObj.GetComponent<FloatingText>();
        floatingText.Setup(amount.ToString(), Color.red);
    }
}
