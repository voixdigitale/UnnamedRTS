using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable {
    public Player player {get;}

    public void Select();
    public void Deselect();

    public bool BelongsToPlayer(Player player) => this.player == player;
}
