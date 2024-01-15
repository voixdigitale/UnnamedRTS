using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static event Action<ISelectable?> OnSelectionChanged;
    public static SelectionManager Instance { get; private set; }

    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private RectTransform selectionBox;

    [HideInInspector] public List<ISelectable> currentSelection = new List<ISelectable>();
    private Vector2 selectionStartPos;

    private Camera camera;
    private Player player;

    void Awake()
    {
        camera = Camera.main;
        player = GetComponent<Player>();

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            DeselectAll();

            SelectUnit();

            selectionStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }

        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in player.units)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                currentSelection.Add(unit);
                unit.Select();
            }
        }

        foreach (Building building in player.buildings) {
            Vector3 screenPos = camera.WorldToScreenPoint(building.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y) {
                currentSelection.Add(building);
                building.Select();
            }
        }

        OnSelectionChanged?.Invoke(currentSelection.Count > 0 ? currentSelection[0] : null);
    }

    void UpdateSelectionBox(Vector2 currentMousePosition)
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

    void SelectUnit()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayerMask)) {

            ISelectable selection = hit.collider.GetComponent<ISelectable>();

            if (selection != null && selection.BelongsToPlayer(player)) {
                currentSelection.Add(selection);
                selection.Select();
            }
        }
        else
        {
            DeselectAll();
        }

        OnSelectionChanged?.Invoke(currentSelection.Count > 0 ? currentSelection[0] : null);
    }

    void DeselectAll()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach (ISelectable selected in currentSelection)
            {
                selected.Deselect();
            }

            currentSelection.Clear();
        }
    }

    public bool HasUnitsSelected() => currentSelection.Count > 0;

}
