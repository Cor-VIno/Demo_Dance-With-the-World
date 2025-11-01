using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMag : MonoBehaviour {
    float width;
    float height;

    private int nowLevelId;

    struct TargetObj {
        public GameObject Obj;
        public float Energy;
        public float Time;
        public float ResEnergy;
        public E_MagMode PlayerMagType;

        public TargetObj(GameObject obj, float energy, E_MagMode playerMagType) {
            Obj = obj;
            Energy = energy;
            Time = 0f;
            ResEnergy = energy;
            PlayerMagType = playerMagType;
        }
    }

    public float magneticForceBase = 3f;
    public float defaultEnergy = 1000f;
    GameObject lookAtObj;

    List<TargetObj> targetObjs = new();
    HashSet<GameObject> contactedObjs = new();

    Rigidbody rb;

    // E_MagMode[] playerHasMagTypes;
    Stack<E_MagMode> playerHasMagTypes = new();
    public int maxTypeCount = 2;

    private void Awake() {
        Messager.Register<CheckPointMessage>(this, SetCheckPoint);
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        // playerHasMagTypes = new[] { E_MagMode.N, E_MagMode.S };
        // playerHasMagTypes.Push(E_MagMode.N);
        // playerHasMagTypes.Push(E_MagMode.S);
        UpdateHasMagTypes();
    }

    void Update() {
        LookObj();
        CheckAndLockObj();
        if (Input.GetKeyDown(KeyCode.R)) {
            Messager.Send(new PlayerNeedResetMessage(nowLevelId, rb, transform, this, false));
        }
    }

    private void FixedUpdate() {
        GenerateForce();
    }

    private void UpdateHasMagTypes() {
        Messager.Send(new MagTypesChangedMessage(new Stack<E_MagMode>(playerHasMagTypes)));
    }

    private void LookObj() {
        width = Screen.width;
        height = Screen.height;
        Ray ray = Camera.main!.ScreenPointToRay(new Vector3(width / 2, height / 2, 0));
        if (Physics.Raycast(ray, out var raycastHit, 100 /*, 1 << LayerMask.NameToLayer("Magnetometric")*/)) {
            GameObject colliderGameObject = raycastHit.collider.gameObject;
            if (lookAtObj) {
                lookAtObj.GetComponent<MagneticController>().NotLookingAt();
                lookAtObj = null;
            }

            if (colliderGameObject.CompareTag("Magnetometric") && (!lookAtObj || colliderGameObject != lookAtObj)) {
                lookAtObj = colliderGameObject;
                lookAtObj.GetComponent<MagneticController>().LookingAt();
            }
        }
    }

    void CheckAndLockObj() {
        if (!lookAtObj) {
            return;
        }

        MagneticController lookAtObjMag = lookAtObj.GetComponent<MagneticController>();
        if (Input.GetMouseButtonDown(0) && playerHasMagTypes.Count > 0) {
            if (lookAtObjMag.magMode != E_MagMode.None) {
                if (!contactedObjs.Contains(lookAtObj) || lookAtObjMag.magMode == playerHasMagTypes.Peek()) {
                    targetObjs.Add(new TargetObj(lookAtObj, defaultEnergy, playerHasMagTypes.Pop()));
                    UpdateHasMagTypes();
                }
            } else {
                lookAtObjMag.SetMagMode(playerHasMagTypes.Pop());
                UpdateHasMagTypes();
            }
        } else if (Input.GetMouseButtonDown(1) && playerHasMagTypes.Count < maxTypeCount) {
            if (lookAtObjMag.magMode != E_MagMode.None) {
                playerHasMagTypes.Push(lookAtObjMag.TakeMagMode());
                UpdateHasMagTypes();
            }
        }
    }

    bool IsAttract(TargetObj targetObj) {
        return targetObj.Obj.GetComponent<MagneticController>().magMode != targetObj.PlayerMagType;
    }

    void GenerateForce() {
        List<TargetObj> newTargetObjs = new();

        foreach (TargetObj targetObj in targetObjs) {
            TargetObj newTargetObj = targetObj;
            if (targetObj.ResEnergy > 0 && targetObj.Obj.GetComponent<MagneticController>().magMode != E_MagMode.None) {
                if (IsAttract(targetObj)) {
                    Attract(targetObj);
                } else {
                    Repulse(targetObj);
                }

                newTargetObj.Time += Time.fixedDeltaTime;
                newTargetObj.ResEnergy = Mathf.Lerp(newTargetObj.Energy, 0, newTargetObj.Time);
            }

            if (newTargetObj.ResEnergy > 0.05f) {
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

    void Attract(TargetObj targetObj) {
        if (targetObj.ResEnergy > 0) {
            Vector3 dis = targetObj.Obj.transform.position - transform.position;
            float force = Mathf.Min(Mathf.Max(Mathf.Min(
                magneticForceBase * targetObj.ResEnergy / (dis.magnitude * dis.magnitude),
                magneticForceBase * targetObj.ResEnergy), 10f), 300f);
            rb.AddForce(dis.normalized * (1 * force), ForceMode.Force);
            targetObj.Obj.GetComponent<MagneticController>().GenerateInteractionForce(gameObject, force, true);
        }
    }

    void Repulse(TargetObj targetObj) {
        if (targetObj.ResEnergy > 0) {
            Vector3 dis = targetObj.Obj.transform.position - transform.position;
            float force = Mathf.Min(
                Mathf.Max(magneticForceBase * targetObj.ResEnergy / (dis.magnitude * dis.magnitude * dis.magnitude),
                    10f),
                magneticForceBase * targetObj.ResEnergy * 0.05f);
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
    // void Repulse() {
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

    void SetCheckPoint(CheckPointMessage message) {
        nowLevelId = message.LevelId;
    }

    public void Rebirth() {
        Messager.Send(new PlayerNeedResetMessage(nowLevelId, rb, transform, this, true));
    }

    public void InsideReset(List<E_MagMode> rebirthMagTypes) {
        targetObjs.Clear();
        playerHasMagTypes.Clear();
        foreach (E_MagMode magType in rebirthMagTypes) {
            playerHasMagTypes.Push(magType);
        }

        UpdateHasMagTypes();
    }

    private void OnCollisionEnter(Collision collision) {
        //&&!collision.gameObject.CompareTag("Ground")
        //&& collision.gameObject == selectedObj
        if (!collision.gameObject.CompareTag("Ground")) {
            List<TargetObj> newTargetObjs = new();
            contactedObjs.Add(collision.gameObject);

            foreach (TargetObj targetObj in targetObjs) {
                if (targetObj.Obj != collision.gameObject) {
                    newTargetObjs.Add(targetObj);
                } else {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }

            targetObjs = newTargetObjs;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (!collision.gameObject.CompareTag("Ground") && contactedObjs.Contains(collision.gameObject)) {
            contactedObjs.Remove(collision.gameObject);
        }
    }
}