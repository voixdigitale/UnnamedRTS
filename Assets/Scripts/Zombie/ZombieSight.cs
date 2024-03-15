using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSight : MonoBehaviour
{
    public Zombie zombie;
    void OnTriggerEnter(Collider other) {
        zombie.SetTarget(other.transform);
    }
}
