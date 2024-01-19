using UnityEngine;
using System.Collections;

public abstract class Command {
    public bool IsCoroutine;
    public bool RequiresValidation = true;

    public Command() {
        IsCoroutine = false;
    }

    public abstract void Execute();
    public abstract bool ValidateInput(RaycastHit? hit);

    public virtual IEnumerator CommandCoroutine() {
        yield return null;
    }
}