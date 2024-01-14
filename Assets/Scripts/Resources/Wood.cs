using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Resource
{
    private void Awake()
    {
        this.quantity = Random.Range(2, 6) * 100;
        type = ResourceType.Wood;
    }
}
