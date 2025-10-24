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

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Magnetometric"))
            return;
        MagneticController otherMagnetic = collision.gameObject.GetComponent<MagneticController>();
        if (otherMagnetic != null)
        {
            if (magMode == E_MagMode.N && otherMagnetic.magMode == E_MagMode.N)
            {
                Debug.Log("North-North repulsion");
            }
            else if (magMode == E_MagMode.S && otherMagnetic.magMode == E_MagMode.S)
            {
                Debug.Log("South-South repulsion");
            }
            else if ((magMode == E_MagMode.N && otherMagnetic.magMode == E_MagMode.S) ||
                     (magMode == E_MagMode.S && otherMagnetic.magMode == E_MagMode.N))
            {
                otherMagnetic.SetCanMove(false);
            }
            else
            {
                Debug.Log("One or both objects have no magnetic mode.");
            }
        }
    }
}
