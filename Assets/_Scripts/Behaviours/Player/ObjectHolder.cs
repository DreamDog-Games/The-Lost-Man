using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ObjectHolder : MonoBehaviour
{
    [SerializeField] private HoldPointData leftHoldPointData;
    [SerializeField] private HoldPointData rightHoldPointData;

    [SerializeField] private ForceMode throwForceMode = ForceMode.Impulse;

    private Transform camTransform;

    private Dictionary<HoldPointData,HoldableObject> holdingObjects = new(2);
    private void OnEnable()
    {
        InputManager.Instance.OnThrowKeyPerformed += InputManager_Instance_OnThrowKeyPerformed;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnThrowKeyPerformed -= InputManager_Instance_OnThrowKeyPerformed;
    }

    private void Start()
    {
        camTransform = Camera.main.transform;
    }

    private void InputManager_Instance_OnThrowKeyPerformed(object sender, EventArgs e)
    {
        HoldableObject holdableObj;

        if (leftHoldPointData.IsAccupied)
        {
            holdableObj = holdingObjects[leftHoldPointData];

            if (holdableObj != null)
            {
                if (holdableObj.TryGetComponent(out Rigidbody rb))
                {
                    ThrowObject(leftHoldPointData,holdableObj, rb);
                }
            }
        }

        if(rightHoldPointData.IsAccupied)
        {
            holdableObj = holdingObjects[rightHoldPointData];

            if (holdableObj != null)
            {
                if (holdableObj.TryGetComponent(out Rigidbody rb))
                {
                    ThrowObject(rightHoldPointData, holdableObj, rb);
                }
            }
        }

        void ThrowObject(HoldPointData holdPointData, HoldableObject holdableObj, Rigidbody rb)
        {
            holdableObj.SetIsThrowedByPlayer(true);
            holdableObj.transform.SetParent(null);
            holdableObj.transform.gameObject.layer = LayerMask.NameToLayer("Interactable");
            rb.isKinematic = false;
            holdableObj.gameObject.GetComponent<Collider>().isTrigger = false;

            holdableObj.transform.rotation = camTransform.rotation;

            Vector3 forceDir = GetTargetDirection(holdableObj,out Vector3 hitTarget);
            float radiusToHitPoint = (holdableObj.transform.position - hitTarget).magnitude;
            Debug.Log("Distance To Target In Rad " + radiusToHitPoint);

            rb.AddForce(forceDir * holdableObj.GetHoldableObjectSO().throwForce, throwForceMode);

            holdingObjects.Remove(holdPointData);
            holdPointData.IsAccupied = false;

            Debug.Log(holdPointData.HoldPointName);
            Debug.Log(holdableObj.GetHoldableObjectSO().throwForce);

            if (holdableObj.GetHoldableObjectSO().Name == "Pole")
            {
                holdableObj.GetComponent<Pole>().isThrowed = true;
                StartCoroutine(CallEventPlayerThrowedSpear(radiusToHitPoint));
            }
        }

    }

    private IEnumerator CallEventPlayerThrowedSpear(float radiusToHitPoint)
    {
        yield return new WaitForSeconds(0.2f);
        EventManager.Instance.InvokeSpearThrowedTowardsFish(transform.position, radiusToHitPoint);
    }

    private Vector3 GetTargetDirection(HoldableObject holdableObj, out Vector3 hitTarget)
    {
        Vector3 forceDir = Vector3.zero;
        Vector3 hitPosition = Vector3.zero;
        Ray ray = new()
        {
            origin = camTransform.position,
            direction = camTransform.forward
        };
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            if (hit.collider != null)
            {
                hitPosition = hit.point;
                forceDir = (hit.point - holdableObj.transform.position).normalized;
            }
        }
        else
        {
            hitPosition = ray.GetPoint(10);
            forceDir = (ray.GetPoint(10) - holdableObj.transform.position).normalized;
        }

        hitTarget = hitPosition;
        return forceDir;
    }

    public void SetParent(HoldableObject holdableObject)
    {
        if (!rightHoldPointData.IsAccupied)
        {
            Set(holdableObject,rightHoldPointData);
        }
        else if(!leftHoldPointData.IsAccupied && !Torch.Instance.IsActive())
        {
            Set(holdableObject, leftHoldPointData);
        }
        else
        {
            Debug.LogWarning("All Holding Slots Accupied");
        }

        void Set(HoldableObject holdableObject,HoldPointData holdPointData)
        {
            if(holdableObject.IsPlacedInTruck())
            {
                FindObjectOfType<ObjectAlignmentAI>().RemovePlacedObject(holdableObject);
            }

            holdableObject.SetIsThrowedByPlayer(false);
            holdableObject.SetIsPlacedInTruck(false);

            holdableObject.transform.SetParent(holdPointData.HoldPointTransform);

            holdableObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            holdableObject.gameObject.GetComponent<Collider>().isTrigger = true;

            holdableObject.transform.localPosition = Vector3.zero + holdableObject.GetHoldableObjectSO().holdOffset;
            holdableObject.transform.localEulerAngles = Vector3.zero + holdableObject.GetHoldableObjectSO().holdOffsetRot;

            holdPointData.IsAccupied = true;
            holdableObject.transform.gameObject.layer = LayerMask.NameToLayer("Default");
            holdableObject.transform.rotation = holdPointData.HoldPointTransform.rotation;
            holdingObjects.Add(holdPointData, holdableObject);

            if(holdableObject.GetHoldableObjectSO().Name == "Pole")
            {
                var pole = holdableObject.GetComponent<Pole>();
                pole.isHitted = false;
                pole.isThrowed = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (leftHoldPointData.HoldPointTransform != null && rightHoldPointData.HoldPointTransform != null)
        {
            Gizmos.DrawSphere(leftHoldPointData.HoldPointTransform.position, 0.05f);
            Gizmos.DrawSphere(rightHoldPointData.HoldPointTransform.position, 0.05f);
        }

    }

    public HoldableObject[] GetHoldingObjectsSO()
    {
        List<HoldableObject> holdingObjectsList = new List<HoldableObject>();

        foreach (var pair in holdingObjects)
        {
            holdingObjectsList.Add(pair.Value);
        }

        return holdingObjectsList.ToArray();
    }

    public void RemoveHoldabe(HoldableObject holdableObject)
    {
        // Find the HoldPointData associated with the holdableObject in the dictionary
        HoldPointData key = holdingObjects.FirstOrDefault(x => x.Value == holdableObject).Key;

        if (key != null)
        {
            key.IsAccupied = false;
            holdingObjects.Remove(key);
        }
    }


    [Serializable]
    private class HoldPointData
    {
        public string HoldPointName;
        public Transform HoldPointTransform;
        public bool IsAccupied;
    }

   
}
