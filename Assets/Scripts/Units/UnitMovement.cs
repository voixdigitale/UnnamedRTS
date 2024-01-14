using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public static List<Vector3> GetGroupDestinations(Vector3 moveToPos, int numUnits, float unitGap)
    {
        List<Vector3> destinations = new List<Vector3>();

        int rowNum = Mathf.CeilToInt(Mathf.Sqrt(numUnits));
        int colNum = Mathf.CeilToInt((float)numUnits / rowNum);


        int row = 0;
        int col = 0;

        float width = (colNum - 1) * unitGap;
        float length = (rowNum - 1) * unitGap;

        for (int i = 0; i < numUnits; i++)
        {
            destinations.Add(new Vector3(moveToPos.x - width / 2 + col * unitGap, moveToPos.y, moveToPos.z - length / 2 + row * unitGap));

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
}
