using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float destroyTime = 1f;

    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
    }

    void Start()
    {
        Invoke("DestroyText", destroyTime);
    }

    void DestroyText()
    {
        Destroy(gameObject);
    }
}
