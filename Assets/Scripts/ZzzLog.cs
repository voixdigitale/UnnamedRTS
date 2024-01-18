using UnityEngine;
using System.Collections;

public enum LogPosition { TopLeft, TopRight, BottomLeft, BottomRight }

public class ZzzLog : MonoBehaviour {
    [SerializeField] uint qsize = 15;  // number of messages to keep
    [SerializeField] LogPosition position = LogPosition.TopLeft;
    [SerializeField] int padding = 10;  // padding from edge of screen
    [SerializeField] int width = 400;  // width of log window
    [SerializeField] Color color = Color.white;
    int height = 0;  // height of log windows, calculated from number of messages

    Queue myLogQueue = new Queue();

    void Start() {
        Debug.Log("Started up logging.");
    }

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI() {
        height = 15 * (myLogQueue.Count + 1) + padding;
        GUILayout.BeginArea(GetRect());
        GUI.contentColor = color;
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }

    Rect GetRect() => position switch {
        LogPosition.TopLeft => new Rect(padding, padding, width, Screen.height - padding * 2),
        LogPosition.TopRight => new Rect(Screen.width - width - padding, padding, width, Screen.height - padding * 2),
        LogPosition.BottomLeft => new Rect(padding, Screen.height - height - padding, width, Screen.height - padding * 2),
        LogPosition.BottomRight => new Rect(Screen.width - width - padding, Screen.height - height - padding, width, Screen.height - padding * 2),
        _ => new Rect(padding, padding, width, Screen.height - padding * 2)
    };
}