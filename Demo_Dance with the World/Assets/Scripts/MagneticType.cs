using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_MagMode
{
    None,
    N,
    S,
}
public class MagneticType : MonoBehaviour
{
    public E_MagMode magMode = E_MagMode.None; 
}
