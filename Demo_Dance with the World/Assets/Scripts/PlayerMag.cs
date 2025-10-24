using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMag : MonoBehaviour
{
    float width;
    float height;

    //Outline outline;
    // Ray r;
    // RaycastHit hitInfo;

    struct TargetObj
    {
        public GameObject Obj;
        public float Energy;
        public float time;
        public float resEnergy;

        public TargetObj(GameObject obj, float energy)
        {
            Obj = obj;
            Energy = energy;
            time = 0f;
            resEnergy = energy;
        }
    }


    public float magneticForceBase = 3f;
    public float defaultEnergy = 1000f;
    GameObject lookAtObj;

    List<TargetObj> targetObjs = new();
    HashSet<GameObject> contactedObjs = new();

    Rigidbody rb;

    E_MagMode[] eMagModes;
    private int selfTypeIndex = 0;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        eMagModes = new[] { E_MagMode.N, E_MagMode.S };
    }

    void Update()
    {
        LookObj();
        CheckAndLockObj();
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
        //if (Physics.Raycast(ray, out raycastHit, 100, 1 << LayerMask.NameToLayer("whatIsGround")))
        //{
        //    print(1);
        //    return;
        //}
        if (Physics.Raycast(ray, out raycastHit, 100/*, 1 << LayerMask.NameToLayer("Magnetometric")*/))
        {
            GameObject colliderGameObject = raycastHit.collider.gameObject;
            if (!colliderGameObject.CompareTag("Magnetometric"))
            {
                if (lookAtObj)
                {
                    lookAtObj.GetComponent<MagneticController>().NotLookingAt();
                    lookAtObj = null;
                }
                return;
            }
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
            lookAtObj.GetComponent<MagneticController>().magMode != E_MagMode.None &&
            (!contactedObjs.Contains(lookAtObj) || !IsAttract(lookAtObj)))
        {
            targetObjs.Add(new TargetObj(lookAtObj, defaultEnergy));
        }
    }

    bool IsAttract(GameObject obj)
    {
        return obj.GetComponent<MagneticController>().magMode != eMagModes[selfTypeIndex];
    }

    void GenerateForce()
    {
        List<TargetObj> newTargetObjs = new();

        foreach (TargetObj targetObj in targetObjs)
        {
            TargetObj newTargetObj = targetObj;
            if (targetObj.resEnergy > 0)
            {
                if (IsAttract(targetObj.Obj))
                {
                    Attract(targetObj);
                }
                else
                {
                    Repul(targetObj);
                }

                newTargetObj.time += Time.fixedDeltaTime;
                newTargetObj.resEnergy = Mathf.Lerp(newTargetObj.Energy, 0, newTargetObj.time);
            }

            if (newTargetObj.resEnergy > 0.05f)
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
        if (targetObj.resEnergy > 0)
        {
            Vector3 dis = targetObj.Obj.transform.position - transform.position;
            float force = Mathf.Min(Mathf.Max(Mathf.Min(magneticForceBase * targetObj.resEnergy / (dis.magnitude * dis.magnitude),
                magneticForceBase * targetObj.resEnergy), 10f), 300f);
            rb.AddForce(dis.normalized * (1 * force), ForceMode.Force);
            targetObj.Obj.GetComponent<MagneticController>().GenerateInteractionForce(gameObject, force, true);
        }
    }

    void Repul(TargetObj targetObj)
    {
        if (targetObj.resEnergy > 0)
        {
            Vector3 dis = targetObj.Obj.transform.position - transform.position;
            float force = Mathf.Min(Mathf.Max(magneticForceBase * targetObj.resEnergy / (dis.magnitude * dis.magnitude * dis.magnitude), 10f),
                magneticForceBase * targetObj.resEnergy * 0.05f);
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
            contactedObjs.Add(collision.gameObject);

            foreach (TargetObj targetObj in targetObjs)
            {
                if (targetObj.Obj != collision.gameObject)
                {
                    newTargetObjs.Add(targetObj);
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }

            targetObjs = newTargetObjs;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground") && contactedObjs.Contains(collision.gameObject))
        {
            contactedObjs.Remove(collision.gameObject);
            
        }
    }
}