using UnityEngine;
using System.Collections;

public abstract class Command {
    public bool IsCoroutine;
    public bool RequiresValidation = true;
    public PlayerController player;

    protected Command(PlayerController player) {
        IsCoroutine = false;
        this.player = player;
    }

    public abstract void Execute();
    public abstract bool ValidateInput(RaycastHit? hit, object[] args = null);

    public virtual IEnumerator CommandCoroutine() {
        yield return null;
    }
}