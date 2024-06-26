using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBuilding : Building
{
    [SerializeField] private Transform exitPos;
    [SerializeField] private Transform entrancePos;
    [SerializeField] private float unloadTime = 1f;

    private List<Gatherer> unitsInside = new List<Gatherer>();
    private List<float> timesOfArrival = new List<float>();

    void StoreUnit(Gatherer unit) {
        unitsInside.Add(unit);
        timesOfArrival.Add(Time.time);
        unit.gameObject.SetActive(false);
    }

    private void Update() {
        for (int i = 0; i < unitsInside.Count; i++) {
            if (Time.time - timesOfArrival[i] > unloadTime) {
                unitsInside[i].transform.position = exitPos.position;
                unitsInside[i].gameObject.SetActive(true);
                unitsInside[i].UnloadBackPack();
                unitsInside.RemoveAt(i);
                timesOfArrival.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        Gatherer unit = other.GetComponent<Gatherer>();

        if (unit != null) {
            StoreUnit(unit);
        }
    }

    public Vector3 GetEntrance() {
        return entrancePos.position;
    }
}
