using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public enum CommandState {
    Idle,
    AwaitingForInput,
    BuildingPlacement,
    ReadyToExecute
}

public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance { get; private set; }
    public CommandState State { get; private set; } = CommandState.Idle;

    public LayerMask layerMask;
    public LayerMask groundMask;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D pointerCursor;

    private SelectionManager unitSelection;
    private new Camera camera;
    private Command command;
    private PlayerController player;
    private GameObject buildingPreview;

    void Awake() {
        unitSelection = GetComponent<SelectionManager>();
        player = GetComponent<PlayerController>();

        camera = Camera.main;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

        Instance = this;
    }

    void Update() {
        switch (State) {
            case CommandState.Idle:
                CleanPreview();
                HandleSelection();

                if (Input.GetMouseButtonDown(1) && unitSelection.HasUnitsSelected()) {
                    HandleRightClick();
                }
                break;

            case CommandState.BuildingPlacement:
                buildingPreview.transform.position = GetMouseWorldPosition();
                if (Input.GetMouseButtonDown(0))
                {
                    if (command.ValidateInput(GetMouseHit(), new object[] {buildingPreview.GetComponent<BuildingPreview>()}))
                    {
                        State = CommandState.ReadyToExecute;
                    }
                    else
                    {
                        PlayerUI.Instance.ShowErrorMessage("Invalid placement");
                    }
                }
                break;

            case CommandState.AwaitingForInput:
                CleanPreview();
                if (Input.GetMouseButtonDown(0) && unitSelection.HasUnitsSelected()) {
                    if (command.ValidateInput(GetMouseHit())) {
                        CleanPreview();
                        State = CommandState.ReadyToExecute;
                    }
                }
                break;

            case CommandState.ReadyToExecute:
                CleanPreview();
                if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))) {
                    State = CommandState.Idle;
                }
                ExecuteCommand();
                break;
        }
    }

    private void HandleSelection() {
        if (Input.GetMouseButtonDown(0)) {
            unitSelection.SelectUnit();

            unitSelection.SetSelectionStartPos(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0)) {
            unitSelection.ReleaseSelectionBox();
        }

        if (Input.GetMouseButton(0)) {
            unitSelection.UpdateSelectionBox(Input.mousePosition);
        }
    }

    private void CleanPreview()
    {
        if (buildingPreview != null)
        {
            Destroy(buildingPreview);
            buildingPreview = null;
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

    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void HandleRightClick() {
        command = new MoveCommand(player);
        if (command.ValidateInput(GetMouseHit())) {
            State = CommandState.ReadyToExecute;
            return;
        }

        command = new GatherCommand(player);
        if (command.ValidateInput(GetMouseHit())) {
            State = CommandState.ReadyToExecute;
            return;
        }

        command = new AttackCommand(player);
        if (command.ValidateInput(GetMouseHit())) {
            State = CommandState.ReadyToExecute;
            return;
        }
    }

    public void SetCommand(Command command) {
        this.command = command;
        if (command.RequiresValidation)
        {
            if (command is IPlaceable)
            {
                IPlaceable placeableCommand = (IPlaceable)command;
                CleanPreview();
                buildingPreview = Instantiate(placeableCommand.GetBuildingData().BuildingPreview);

                State = CommandState.BuildingPlacement;
            }
            else
            {
                State = CommandState.AwaitingForInput;
            }
            Cursor.SetCursor(pointerCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            State = CommandState.ReadyToExecute;
        }

    }

    public void ExecuteCommand() {
        if (command.IsCoroutine)
            ExecuteCoroutineCommand();
        else
            command.Execute();

        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        State = CommandState.Idle;
    }

    public void ExecuteCoroutineCommand() {
        StartCoroutine(command.CommandCoroutine());
    }
}