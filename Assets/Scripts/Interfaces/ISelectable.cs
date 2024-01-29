using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable {
    public PlayerController player {get;}

    public void Select();
    public void Deselect();

    public bool BelongsToPlayer(PlayerController player) => this.player == player;
}
