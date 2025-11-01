using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerMagTypesController : MonoBehaviour
{
    private readonly List<PlayerMagTypeController> magTypes = new();

    private void Awake()
    {
        magTypes.Add(GameObject.Find("PlayerMagTypes/MagTypeIcon1").GetComponent<PlayerMagTypeController>());
        magTypes.Add(GameObject.Find("PlayerMagTypes/MagTypeIcon2").GetComponent<PlayerMagTypeController>());
        Messager.Register<MagTypesChangedMessage>(this, ShowTypes);
    }
    void ShowTypes(MagTypesChangedMessage message)
    {
        for (int i = 1; i >= 0; i--)
        {
            magTypes[i].SetMagType(message.Types.TryPop(out var type) ? type : E_MagMode.None);

        }
    }
}