using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMag : MonoBehaviour
{
    float width;
    float height;

    // Ray r;
    // RaycastHit hitInfo;

    struct TargetObj
    {
        public GameObject Obj;
        public float Energy;

        public TargetObj(GameObject obj, float energy)
        {
            Obj = obj;
            Energy = energy;
        }
    }

    // bool isHit;
    // bool isAttractMove;
    public int magneticForceBase = 3;
    public float defaultEnergy = 1000f;
    GameObject lookAtObj;

    List<TargetObj> targetObjs = new();

    // GameObject selectedObj;
    Rigidbody rb;
    // MagneticController magneticController;

    E_MagMode[] eMagModes;
    private int selfTypeIndex = 0;

    // public float attractSpeed;

    // Vector3 dir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        eMagModes = new[] { E_MagMode.N, E_MagMode.S };
    }

    void Update()
    {
        // CheckHitAndDraw();
        LookObj();
        CheckAndLockObj();
        // if (selectedObj && magneticController.magMode != E_MagMode.None) {
        //     if (magneticController.magMode != e_MagModes[selfTypeIndex]) {
        //         Attract();
        //     } else {
        //         Repul();
        //     }
        // }
    }

    private void FixedUpdate()
    {
        GenerateForce();
    }

    private void LookObj()
    {
        width = Screen.width;
        height = Screen.height;
        Ray ray = Camera.main!.ScreenPointToRay(new Vector3(width / 2, height / 2, 0));
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 100, 1 << LayerMask.NameToLayer("Magnetometric")))
        {
            GameObject colliderGameObject = raycastHit.collider.gameObject;
            if (!lookAtObj || colliderGameObject != lookAtObj)
            {
                if (lookAtObj)
                {
                    lookAtObj.GetComponent<MagneticController>().NotLookingAt();
                }

                lookAtObj = colliderGameObject;
                lookAtObj.GetComponent<MagneticController>().LookingAt();
            }
        }
        else
        {
            if (lookAtObj)
            {
                lookAtObj.GetComponent<MagneticController>().NotLookingAt();
                lookAtObj = null;
            }
        }
    }

    void CheckAndLockObj()
    {
        if (Input.GetMouseButtonDown(0) && lookAtObj &&
            lookAtObj.GetComponent<MagneticController>().magMode != E_MagMode.None)
        {
            targetObjs.Add(new TargetObj(lookAtObj, defaultEnergy));
        }
    }

    void GenerateForce()
    {
        List<TargetObj> newTargetObjs = new();

        foreach (TargetObj targetObj in targetObjs)
        {
            TargetObj newTargetObj = targetObj;
            if (targetObj.Energy > 0)
            {
                if (targetObj.Obj.GetComponent<MagneticController>().magMode != eMagModes[selfTypeIndex])
                {
                    Attract(targetObj);
                }
                else
                {
                    Repul(targetObj);
                }

                newTargetObj.Energy = Mathf.Lerp(newTargetObj.Energy, 0, Time.fixedDeltaTime);
            }

            if (newTargetObj.Energy > 0.1f)
            {
                newTargetObjs.Add(newTargetObj);
            }
        }

        targetObjs = newTargetObjs;
    }

    // void CheckHitAndDraw() {
    //     width = Screen.width;
    //     height = Screen.height;
    //     r = Camera.main.ScreenPointToRay(new Vector3(width / 2, height / 2, 0));
    //     if (Physics.Raycast(r, out hitInfo, 100, 1 << LayerMask.NameToLayer("Magnetometric"))) {
    //         if (!isHit) {
    //             isHit = true;
    //             selectedObj = hitInfo.collider.gameObject;
    //             Outline ol = selectedObj.GetComponent<Outline>();
    //             ol.OutlineWidth = 5f;
    //             magneticController = selectedObj.GetComponent<MagneticController>();
    //             if (magneticController.magMode == E_MagMode.N)
    //                 ol.OutlineColor = Color.red;
    //             else if (magneticController.magMode == E_MagMode.S)
    //                 ol.OutlineColor = Color.blue;
    //             else
    //                 ol.OutlineColor = Color.black;
    //         }
    //
    //         if (selectedObj != hitInfo.collider.gameObject) {
    //             Outline ol = selectedObj.GetComponent<Outline>();
    //             ol.OutlineWidth = 0f;
    //             selectedObj = hitInfo.collider.gameObject;
    //             ol = selectedObj.GetComponent<Outline>();
    //             ol.OutlineWidth = 5f;
    //             magneticController = selectedObj.GetComponent<MagneticController>();
    //             if (magneticController.magMode == E_MagMode.N)
    //                 ol.OutlineColor = Color.red;
    //             else if (magneticController.magMode == E_MagMode.S)
    //                 ol.OutlineColor = Color.blue;
    //             else
    //                 ol.OutlineColor = Color.black;
    //         }
    //     } else {
    //         if (isHit) {
    //             isHit = false;
    //             Outline ol = selectedObj.GetComponent<Outline>();
    //             ol.OutlineWidth = 0f;
    //         }
    //     }
    // }

    void Attract(TargetObj targetObj)
    {
        if (targetObj.Energy > 0)
        {
            Vector3 dis = targetObj.Obj.transform.position - transform.position;
            float force = Math.Min(magneticForceBase * targetObj.Energy / (dis.magnitude * dis.magnitude),
                magneticForceBase * targetObj.Energy);
            rb.AddForce(dis.normalized * (1 * force), ForceMode.Force);
            targetObj.Obj.GetComponent<MagneticController>().GenerateInteractionForce(gameObject, force, true);
        }
    }

    void Repul(TargetObj targetObj)
    {
        if (targetObj.Energy > 0)
        {
            Vector3 dis = targetObj.Obj.transform.position - transform.position;
            float force = Math.Min(magneticForceBase * targetObj.Energy / (dis.magnitude * dis.magnitude),
                magneticForceBase * targetObj.Energy);
            rb.AddForce(dis.normalized * (-1 * force), ForceMode.Force);
            targetObj.Obj.GetComponent<MagneticController>().GenerateInteractionForce(gameObject, force, false);
        }
    }

    // void Attract() {
    //     if (Input.GetMouseButton(0) && isHit && energy > 0) {
    //         isAttractMove = true;
    //         dir = selectedObj.transform.position - transform.position;
    //         rb.AddForce(
    //             dir.normalized * (1 * Math.Min(magneticForceBase * energy-- / (dir.magnitude * dir.magnitude), magneticForceBase)),
    //             ForceMode.Force);
    //     } else if (energy < 1000) {
    //         energy++;
    //     }
    //
    //     // if (isAttractMove) {
    //     //     dir = selectedObj.transform.position - transform.position;
    //     //     this.transform.Translate(dir.normalized * (attractSpeed * Time.deltaTime), Space.World);
    //     // }
    // }
    //
    // void Repul() {
    //     if (Input.GetMouseButton(0) && isHit && energy > 0) {
    //         dir = selectedObj.transform.position - transform.position;
    //         rb.AddForce(
    //             dir.normalized * (-1 * Math.Min(magneticForceBase * energy-- / (dir.magnitude * dir.magnitude), magneticForceBase)),
    //             ForceMode.Force);
    //     } else if (energy < 1000) {
    //         energy++;
    //     }
    //
    //     // if (isRepulMove) {
    //     //     dir = selectedObj.transform.position - transform.position;
    //     //     this.transform.Translate(dir.normalized * (-1 * attractSpeed * Time.deltaTime), Space.World);
    //     // }
    // }

    private void OnCollisionEnter(Collision collision)
    {
        //&&!collision.gameObject.CompareTag("Ground")
        //&& collision.gameObject == selectedObj
        if (!collision.gameObject.CompareTag("Ground"))
        {
            List<TargetObj> newTargetObjs = new();

            foreach (TargetObj targetObj in targetObjs)
            {
                if (targetObj.Obj != collision.gameObject)
                {
                    newTargetObjs.Add(targetObj);
                }
            }

            targetObjs = newTargetObjs;
        }
    }
}