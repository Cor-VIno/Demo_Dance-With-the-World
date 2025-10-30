using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIInside : MonoBehaviour {
    private RectTransform column;

    void Awake() {
        column = GetComponent<RectTransform>();
    }

    public void SetHeight(float percent) {
        column.anchoredPosition = new Vector2(column.anchoredPosition.x, 500 * percent / 2f - 250);
        column.sizeDelta = new Vector2(column.sizeDelta.x, 500 * percent);
    }
}