using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagTypeController : MonoBehaviour {
    private RawImage backGround;
    private TextMeshProUGUI text;

    void Start() {
        backGround = GetComponentInChildren<RawImage>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        SetMagType(E_MagMode.None);
    }

    public void SetMagType(E_MagMode mode) {
        if (mode == E_MagMode.None) {
            backGround.color = Color.gray;
            text.text = "";
        } else if (mode == E_MagMode.N) {
            backGround.color = Color.red;
            text.text = "N";
        } else {
            backGround.color = Color.blue;
            text.text = "S";
        }
    }
}