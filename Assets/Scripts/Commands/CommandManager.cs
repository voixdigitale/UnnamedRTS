using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum CommandState {
    Idle,
    AwaitingForInput,
    ReadyToExecute
}

public class CommandManager : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;

    public CommandState State { get; private set; } = CommandState.Idle;

    public LayerMask layerMask;

    private SelectionManager unitSelection;
    private new Camera camera;
    private Command command;

    void Awake() {
        unitSelection = GetComponent<SelectionManager>();
        camera = Camera.main;
    }

    void Update() {
        switch (State) {
            case CommandState.Idle:
                HandleSelection();

                if (Input.GetMouseButtonDown(1) && unitSelection.HasUnitsSelected()) {
                    HandleRightClick();
                }
                break;

            case CommandState.AwaitingForInput:
                if (Input.GetMouseButtonDown(0) && unitSelection.HasUnitsSelected()) {
                    if(command.ValidateInput(GetMouseHit())) {
                        State = CommandState.ReadyToExecute;
                    }
                }
                break;

            case CommandState.ReadyToExecute:
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                    State = CommandState.Idle;
                }
                ExecuteCommand();
                break;
        }
    }

    private void HandleSelection() {
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
    }

    public RaycastHit? GetMouseHit() {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            return hit;
        }

        return null;
    }

    private void HandleRightClick() {
        command = new MoveCommand();
        if (command.ValidateInput(GetMouseHit()))
            State = CommandState.ReadyToExecute;
    }

    public void SpawnHitMarker(Vector3 position, float scale = 1.0f) {
        GameObject marker = Instantiate(selectionMarkerPrefab, position, Quaternion.identity);

        marker.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetState(CommandState state) {
        State = state;
    }

    public void SetCommand(Command command) {
        this.command = command;
        State = CommandState.AwaitingForInput;
    }
    public void ExecuteCommand() {
        if (command.IsCoroutine)
            ExecuteCoroutineCommand();
        else
            command.Execute();
    }

    public void ExecuteCoroutineCommand() {
        StartCoroutine(command.CommandCoroutine());
    }
}