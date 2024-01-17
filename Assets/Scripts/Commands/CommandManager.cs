using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public enum CommandState {
    Idle,
    AwaitingForInput,
    ReadyToExecute
}

public class CommandManager : MonoBehaviour
{
    public CommandState State { get; private set; } = CommandState.Idle;

    public LayerMask layerMask;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D pointerCursor;

    private SelectionManager unitSelection;
    private new Camera camera;
    private Command command;

    void Awake() {
        unitSelection = GetComponent<SelectionManager>();
        camera = Camera.main;

        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
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
                    if (command.ValidateInput(GetMouseHit())) {
                        State = CommandState.ReadyToExecute;
                    }
                }
                break;

            case CommandState.ReadyToExecute:
                if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))) {
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
        else
        {
            command = new GatherCommand();
            if (command.ValidateInput(GetMouseHit()))
                State = CommandState.ReadyToExecute;
        }
    }

    public void SetCommand(Command command) {
        this.command = command;
        State = CommandState.AwaitingForInput;
        Cursor.SetCursor(pointerCursor, Vector2.zero, CursorMode.Auto);
    }
    public void ExecuteCommand() {
        if (command.IsCoroutine)
            ExecuteCoroutineCommand();
        else
            command.Execute();

        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void ExecuteCoroutineCommand() {
        StartCoroutine(command.CommandCoroutine());
    }
}