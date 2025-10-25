using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Geomagnetism : MagneticController
{
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        UpdateColor();
        UpdateCanMove();

        SetCanMove(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Magnetometric"))
        {
            return;
        }
        MagneticController otherMagnetic = collision.gameObject.GetComponent<MagneticController>();
        otherMagnetic.SetCanMove(true);
        if (magMode != E_MagMode.None && otherMagnetic.magMode != E_MagMode.None)
        {
            if (magMode != otherMagnetic.magMode)
            {
                otherMagnetic.SetCanMove(false);
            }
            else
            {
                otherMagnetic.GenerateInteractionForce(gameObject, 100, false);
            }
        }
    }
}
