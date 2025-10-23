using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMag : MonoBehaviour
{
    float width;
    float height;

    Ray r;
    RaycastHit hitInfo;
    RaycastHit hitI;
    RaycastHit hitIA;
    RaycastHit hitIR;

    bool isHit;
    bool isAttractMove;
    bool isRepulMove;

    GameObject selectedObj;
    Rigidbody rb;
    MagneticType magneticType;
    MagneticType pickMagneticType;

    E_MagMode[] e_MagModes;
    private int selfTypeIndex = 0;

    public float attractSpeed;
    float imaAttractSpeed;
    Vector3 dirAtt;

    public float repulForce = 5000;
    float imaRepulForce;
    Vector3 dirRep;

    float imaSpeed;
    Vector3 dirIma;

    PlayerMovement pm;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        e_MagModes = new E_MagMode[2] { E_MagMode.N, E_MagMode.S };
    }

    void Update()
    {
        CheckHitAndDraw();
        if (Input.GetMouseButtonDown(0) && isHit)
        {
            if (!(magneticType.magMode == E_MagMode.None || selfTypeIndex >= 2 || e_MagModes[selfTypeIndex] == E_MagMode.None))
            {
                hitI = hitInfo;
                pickMagneticType = magneticType;
            }

            if (selfTypeIndex < 2 && selectedObj && pickMagneticType.magMode != E_MagMode.None)
            {
                //吸引
                if (pickMagneticType.magMode != e_MagModes[selfTypeIndex])
                {
                    print("Attract!");
                    isAttractMove = true;
                    imaAttractSpeed = attractSpeed;
                    Attract();
                }
                //排斥
                else
                {
                    isRepulMove = true; ;
                    print("Repul!");
                    imaRepulForce = repulForce;
                    Repul();

                }
            }
        }



    }
    private void FixedUpdate()
    {
        MovePlayer();
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

            if (selectedObj && isHit && selectedObj != hitInfo.collider.gameObject)
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
            if (selectedObj && isHit)
            {
                isHit = false;
                Outline ol = selectedObj.GetComponent<Outline>();
                ol.OutlineWidth = 0f;
            }
        }
    }

    void Attract()
    {
        dirIma = hitI.point - transform.position;
        if (dirIma.magnitude < 0.005f)
            return;


        //可能要根据距离算一下初始速度大小
        //this.transform.Translate(dir.normalized * (attractSpeed * Time.deltaTime), Space.World);

    }

    void Repul()
    {
        dirIma = -hitIR.point + transform.position;
        //可能要根据距离算一下初始速度大小
        //rb.AddForce(dir.normalized * (-1 * repulForce / (dir.magnitude * dir.magnitude)), ForceMode.Force);


        // if (isRepulMove) {
        //     dir = selectedObj.transform.position - transform.position;
        //     this.transform.Translate(dir.normalized * (-1 * attractSpeed * Time.deltaTime), Space.World);
        // }
    }

    void MovePlayer()
    {
        if (isRepulMove)
        {
            imaRepulForce = Mathf.Lerp(imaRepulForce, 0, Time.deltaTime);
            imaSpeed = imaRepulForce;
        }
            
        if (isAttractMove)
        {
            imaAttractSpeed = Mathf.Lerp(imaAttractSpeed, 0, Time.deltaTime);
            imaSpeed = imaAttractSpeed;
        }
            

        //imaSpeed = Mathf.Sqrt(imaRepulForce* imaRepulForce+ imaAttractSpeed* imaAttractSpeed);
        //print(imaSpeed);

        if(imaSpeed < 0.5f)
        {
            dirIma = Vector3.zero;
            isAttractMove = false;
            isRepulMove = false;
        }

     
        //this.transform.Translate(dirIma.normalized * (imaSpeed * Time.deltaTime), Space.World);
        if(!pm.isGrounded)
            rb.AddForce(dirIma.normalized * imaSpeed * pm.airMultiplier, ForceMode.Force);
        else
            rb.AddForce(dirIma.normalized * (imaSpeed + pm.groundDrag), ForceMode.Force);


    }

    private void OnCollisionEnter(Collision collision)
    {
        //&&!collision.gameObject.CompareTag("Ground")
        //&& collision.gameObject == selectedObj
        if (isAttractMove && !collision.gameObject.CompareTag("Ground"))
        {
            isAttractMove = false;
            isRepulMove = false;
            dirAtt = Vector3.zero;
            dirRep = Vector3.zero;
            rb.velocity = Vector3.zero;
            //if(collision.gameObject == selectedObj)
            //    selectedObj = null;
        }
    }
}