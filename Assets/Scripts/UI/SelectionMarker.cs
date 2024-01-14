using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMarker : MonoBehaviour
{
    public float lifeTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
