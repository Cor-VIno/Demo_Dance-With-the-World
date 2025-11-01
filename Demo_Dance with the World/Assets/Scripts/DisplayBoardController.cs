using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBoardController : MonoBehaviour {
    private TextMeshProUGUI text;

    void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        Messager.Register<DisplayMessage>(this, ShowMessage);
    }

    void ShowMessage(DisplayMessage message) {
        text.text = message.Message;
        Invoke(nameof(ClearMessage), message.Duration);
    }

    void ClearMessage() {
        text.text = "";
    }
}