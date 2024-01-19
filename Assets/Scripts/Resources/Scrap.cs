using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : Resource
{
    private void Awake()
    {
        this.quantity = Random.Range(2, 6) * 30;
        type = ResourceType.Scrap;
    }
}
