using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StopCommand : Command
{
    public StopCommand()
    {
        IsCoroutine = false;
        RequiresValidation = false;
    }

    public override void Execute()
    {
        StopUnits();
    }

    public override bool ValidateInput(RaycastHit? hit)
    {
        return true;
    }

    public void StopUnits()
    {
        foreach (Unit unit in SelectionManager.Instance.currentSelection.OfType<Unit>())
        {
            unit.Stop();
        }
    }
}
