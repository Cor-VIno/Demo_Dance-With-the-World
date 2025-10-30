using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumnChangedMessage {
    public readonly float[] NewVolumns;

    public VolumnChangedMessage(float[] newVolumns) {
        NewVolumns = newVolumns;
    }
}

public class DebugUIOutside : MonoBehaviour {
    public GameObject volumnPrefab;
    private readonly List<DebugUIInside> columns = new();

    void Awake() {
        for (float i = -472.5f; i <= 472.5f; i += 15f) {
            GameObject temp = Instantiate(volumnPrefab, gameObject.transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, 0);
            columns.Add(temp.GetComponent<DebugUIInside>());
        }

        Messager.Register<VolumnChangedMessage>(this, SetColumns);
    }

    void SetColumns(VolumnChangedMessage message) {
        float[] newVolumns = message.NewVolumns;
        for (int i = 0; i < Mathf.Min(columns.Count, 64); i++) {
            columns[i].SetHeight(newVolumns[i]);
        }
    }
}