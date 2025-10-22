using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMag : MonoBehaviour
{
    float width;
    float height;

    Ray r;
    RaycastHit hitInfo;

    bool isHit;
    bool isMove;
    GameObject selectedObj;
    Rigidbody rb;
    MagneticType magneticType;

    E_MagMode[] e_MagModes;

    public float attractSpeed;

    Vector3 dir;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        e_MagModes = new E_MagMode[2] { E_MagMode.N, E_MagMode.S };
    }

    void Update()
    {
        CheckHitAndDraw();
        //if(selectedObj!=null&&magneticType.magMode!=E_MagMode.None&&)
        Attract();
    }

    void CheckHitAndDraw()
    {
        width = Screen.width;
        height = Screen.height;
        r = Camera.main.ScreenPointToRay(new Vector3(width / 2, height / 2, 0));
        if (Physics.Raycast(r, out hitInfo, 100, 1 << LayerMask.NameToLayer("Magnetometric")))
        {
            if (!isHit)
            {
                isHit = true;
                selectedObj = hitInfo.collider.gameObject;
                Outline ol = selectedObj.GetComponent<Outline>();
                ol.OutlineWidth = 5f;
                magneticType = selectedObj.GetComponent<MagneticType>();
                if (magneticType.magMode == E_MagMode.N)
                    ol.OutlineColor = Color.red;
                else if (magneticType.magMode == E_MagMode.S)
                    ol.OutlineColor = Color.blue;
                else
                    ol.OutlineColor = Color.black;
            }
            if (selectedObj != hitInfo.collider.gameObject)
            {
                Outline ol = selectedObj.GetComponent<Outline>();
                ol.OutlineWidth = 0f;
                selectedObj = hitInfo.collider.gameObject;
                ol = selectedObj.GetComponent<Outline>();
                ol.OutlineWidth = 5f;
                magneticType = selectedObj.GetComponent<MagneticType>();
                if (magneticType.magMode == E_MagMode.N)
                    ol.OutlineColor = Color.red;
                else if (magneticType.magMode == E_MagMode.S)
                    ol.OutlineColor = Color.blue;
                else
                    ol.OutlineColor = Color.black;
            }
        }
        else
        {
            if (isHit)
            {
                isHit = false;
                Outline ol = selectedObj.GetComponent<Outline>();
                ol.OutlineWidth = 0f;
            }
        }
    }

    void Attract()
    {
        if (Input.GetMouseButtonDown(0) && isHit)
        {
            isMove = true;
        }
        if (isMove)
        {
            dir = selectedObj.transform.position - transform.position;
            this.transform.Translate(dir.normalized * attractSpeed * Time.deltaTime, Space.World);
        }
    }

    void Repulsion()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //&&!collision.gameObject.CompareTag("Ground")
        //&& collision.gameObject == selectedObj
        if (isMove && !collision.gameObject.CompareTag("Ground"))
        {
            isMove = false;
            rb.velocity = Vector3.zero;
        }
    }
}
