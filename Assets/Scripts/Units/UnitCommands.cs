using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitCommands : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;
    public LayerMask layerMask;

    private SelectionManager unitSelection;
    private new Camera camera;
    private bool awaitingForMovePosition = false;

    public bool AwaitingForMovePosition => awaitingForMovePosition;

    void Awake() {
        unitSelection = GetComponent<SelectionManager>();
        camera = Camera.main;
    }

    void Update()
    {
        if (awaitingForMovePosition) {
            if (Input.GetMouseButtonDown(0) && unitSelection.HasUnitsSelected()) {

                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                    if (hit.collider.gameObject.CompareTag("Ground")) {
                        StartCoroutine(MoveUnits(hit));
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                awaitingForMovePosition = false;
            }

        } else {
            if (Input.GetMouseButtonDown(0)) {
                SelectionManager.Instance.SelectUnit();

                SelectionManager.Instance.SetSelectionStartPos(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0)) {
                SelectionManager.Instance.ReleaseSelectionBox();
            }

            if (Input.GetMouseButton(0)) {
                SelectionManager.Instance.UpdateSelectionBox(Input.mousePosition);
            }

            if (Input.GetMouseButtonDown(1) && unitSelection.HasUnitsSelected()) {
                MouseCommands();
            }
        }
        
    }

    void MouseCommands()
    {
        
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                StartCoroutine(MoveUnits(hit));
            }
            else if (hit.collider.gameObject.CompareTag("Resource"))
            {
                GatherResource(hit.collider.GetComponent<Resource>());
            }
            
        }
    }

    IEnumerator MoveUnits(RaycastHit hit)
    {
        List<Vector3> destinations = UnitMovement.GetGroupDestinations(hit.point, unitSelection.currentSelection.Count, 1f);

        foreach (Unit unit in unitSelection.currentSelection.OfType<Unit>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            SpawnSelectionMarker(destination);

            //Adding a bit of randomness to the movement
            yield return new WaitForSeconds(Random.Range(0, 30) / 100);
            
            unit.SetAgentDestination(destination);
        }
    }



    void GatherResource(Resource resource)
    {
        SpawnSelectionMarker(resource.transform.position, 2f);

        List<Vector3> destinations = UnitMovement.GetSurroundingDestinations(resource.transform.position, unitSelection.currentSelection.Count);

        foreach (Gatherer unit in unitSelection.currentSelection.OfType<Gatherer>())
        {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            unit.Gather(resource, destination);
        }
    }

    void AttackUnit(Unit target) {
        SpawnSelectionMarker(target.transform.position, 1f);

        List<Vector3> destinations = UnitMovement.GetSurroundingDestinations(target.transform.position, unitSelection.currentSelection.Count);

        foreach (Unit attacker in unitSelection.currentSelection.OfType<Unit>()) {
            Vector3 destination = destinations.First();
            destinations.Remove(destination);

            attacker.Attack(target, destination);
        }
    }


    void SpawnSelectionMarker(Vector3 position, float scale = 1.0f)
    {
        GameObject marker = Instantiate(selectionMarkerPrefab, position, Quaternion.identity);

        marker.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void StartMoveCommand() {
        awaitingForMovePosition = true;
    }
}
