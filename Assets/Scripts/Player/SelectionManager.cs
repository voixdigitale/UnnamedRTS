using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Timeline;
using Photon.Realtime;

public class SelectionManager : MonoBehaviourPunCallbacks
{
    #nullable enable
    public static event Action<ISelectable?>? OnSelectionChanged;
    #nullable disable

    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] public RectTransform selectionBox;
    [SerializeField] private GameObject selectionMarkerPrefab;

    public List<ISelectable> currentSelection = new List<ISelectable>();
    private Vector2 selectionStartPos;

    private new Camera camera;
    private PlayerController playerController;
    private CommandManager commandManager;

    void Awake()
    {
        camera = Camera.main;
    }

    public void Initialize(Player player) {
        playerController = GameManager.Instance.GetPlayer(player.ActorNumber);
        commandManager = CommandManager.Instance;
        selectionBox = GameManager.Instance.selectionBox;
    }

    public void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in playerController.units)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                currentSelection.Add(unit);
                unit.Select();
            }
        }

        foreach (Building building in playerController.buildings) {
            Vector3 screenPos = camera.WorldToScreenPoint(building.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y) {
                currentSelection.Add(building);
                building.Select();
            }
        }

        OnSelectionChanged?.Invoke(currentSelection.Count > 0 ? currentSelection[0] : null);
    }

    public void UpdateSelectionBox(Vector2 currentMousePosition)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
        {
            selectionBox.gameObject.SetActive(true);
        }
        
        float width = currentMousePosition.x - selectionStartPos.x;
        float height = currentMousePosition.y - selectionStartPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = selectionStartPos + new Vector2(width / 2, height / 2);
    }

    public void SelectUnit()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayerMask)) {

            ISelectable selection = hit.collider.GetComponentInParent<ISelectable>();

            if (selection != null && selection.BelongsToPlayer(playerController)) {
                DeselectAll();
                currentSelection.Add(selection);
                selection.Select();
                OnSelectionChanged?.Invoke(currentSelection[0]);
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject()) {
            DeselectAll();
            OnSelectionChanged?.Invoke(null);
        }

    }

    public void DeselectAll()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && commandManager.State != CommandState.AwaitingForInput)
        {
            foreach (ISelectable selected in currentSelection)
            {
                selected.Deselect();
            }

            currentSelection.Clear();
        }
    }

    public void SpawnSelectionMarker(Vector3 position, float scale = 1.0f, Color? color = null) {
        GameObject marker = Instantiate(selectionMarkerPrefab, position, Quaternion.identity);

        if (color.HasValue) {
            ParticleSystem[] particles = marker.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particles) {
                var main = ps.main;
                main.startColor = Color.red;
            }
        }

        marker.transform.localScale = new Vector3(scale, scale, scale);
    }

    public bool HasUnitsSelected() => currentSelection.Count > 0;

    public void SetSelectionStartPos(Vector2 pos) => selectionStartPos = pos;

}
