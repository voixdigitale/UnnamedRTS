using System.Collections.Generic;
using UnityEngine;

public class UnitPlacements : MonoBehaviour
{
    public static List<Vector3> GetGroupDestinations(Vector3 moveToPos, int numUnits, float unitGap)
    {
        if (numUnits == 1)
        {
            return new List<Vector3>() { moveToPos };
        }

        List<Vector3> destinations = new List<Vector3>();

        int rowNum = Mathf.CeilToInt(Mathf.Sqrt(numUnits));
        int colNum = Mathf.CeilToInt((float)numUnits / rowNum);


        int row = 0;
        int col = 0;

        float width = (colNum - 1) * unitGap;
        float length = (rowNum - 1) * unitGap;

        for (int i = 0; i < numUnits; i++)
        {
            Vector3 destination = new Vector3(moveToPos.x - width / 2 + col * unitGap, moveToPos.y, moveToPos.z - length / 2 + row * unitGap);
            destination += PlacementNoise(destination, 0.5f);

            destinations.Add(destination);

            col++;

            if (col >= colNum)
            {
                col = 0;
                row++;
            }
        }
        

        return destinations;
    }

    public static List<Vector3> GetSurroundingDestinations(Vector3 moveToPos, int numUnits)
    {
        List<Vector3> destinations = new List<Vector3>();

        float unitSeparation = 360.0f / (float) numUnits;

        int offsetAngle = Random.Range(0, 360);

        for (int i = 0; i < numUnits; i++)
        {
            destinations.Add(moveToPos + Quaternion.Euler(0, offsetAngle + unitSeparation * i, 0) * Vector3.forward * 2);
        }

        return destinations;
    }

    public static Vector3 PlacementNoise(Vector3 pos, float noiseLevel) {
        var noise = Mathf.PerlinNoise(pos.x * noiseLevel, pos.z * noiseLevel);
        return new Vector3(noise, 0, noise);
    }
}
