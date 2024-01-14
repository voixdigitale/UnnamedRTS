using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] private LayerMask unitLayerMask;
    [SerializeField] private RectTransform selectionBox;

    [HideInInspector] public List<Unit> selectedUnits = new List<Unit>();
    private Vector2 selectionStartPos;

    private Camera camera;
    private Player player;

    void Awake()
    {
        camera = Camera.main;
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            DeselectAllUnits();

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
                selectedUnits.Add(unit);
                unit.Select();
            }
        }
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

            Unit unit = hit.collider.GetComponent<Unit>();
            
            if (unit != null && unit.player == player) {
                
                if (player.IsPlayerUnit(unit)) {
                    selectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }
        else
        {
            DeselectAllUnits();
        }

    }

    void DeselectAllUnits()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach (Unit unit in selectedUnits)
            {
                unit.Deselect();
            }

            selectedUnits.Clear();
        }
    }

    public bool HasUnitsSelected() => selectedUnits.Count > 0;

}
