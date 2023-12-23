using DG.Tweening;
using ProjectMiamiTestInventory;
using System;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public static event EventHandler OnAnyDoorKnobAnimFinished;

    [SerializeField] private GatherableSO validKeySO;
    [SerializeField] private Transform doorHinge;
    [SerializeField] private Transform doorKnob;
    [SerializeField] private Transform handTargetPos;

    [SerializeField] private float targetOpenAngle = -90f;
    [SerializeField] private float targetKnobAngle = 90f;
    [SerializeField] private float knobRotateDuration = 0.3f;
    [SerializeField] private float doorOpenDuration = 0.5f;
    [SerializeField] private float doorCloseDuration = 0.4f;

    [SerializeField] private Ease easeTypeOpen = Ease.Linear;
    [SerializeField] private Ease easeTypeClose = Ease.Linear;

    [SerializeField] private DoorType doorType;
    [SerializeField] private Transform doorFrameCenter;
    [SerializeField] private Transform victimTransform;

    private bool isOpenedDoor;
    private bool isGotKey;
    private bool canTrapVictim;
    private bool isVictimTrapped;

    [SerializeField] private float doorCloseDistance = 2f;
    [SerializeField] private float dotProductThreshold = 0.2f;

    public enum DoorType
    {
        NoKeyDoor,
        KeyDoor,
        GhostDoor
    }
    public void Interact()
    {
        DoorToggle();
    }

    private void DoorToggle()
    {
        if(doorType == DoorType.NoKeyDoor || doorType == DoorType.GhostDoor)
        {
            isOpenedDoor = !isOpenedDoor;
        }
        else if(doorType == DoorType.KeyDoor)
        {
            CheckPlayerHasKey();

            if(isGotKey)
            {
                isOpenedDoor = !isOpenedDoor;
            }
            else
            {
                Debug.Log("No key You Have");
            }
        }
       

        switch(doorType)
        {
            case DoorType.NoKeyDoor:
                if (isOpenedDoor)
                {
                    // open Door
                    doorHinge.DOLocalRotate(new Vector3(0, targetOpenAngle, 0), doorOpenDuration).SetEase(easeTypeOpen);
                    DoKnobAnimation();
                    Debug.Log("Door Opned");
                }
                else
                {
                    //close Door
                    doorHinge.DOLocalRotate(new Vector3(0, 0, 0), doorCloseDuration).SetEase(easeTypeClose);
                    Debug.Log("Door Closed");
                }
                break; 
            
            case DoorType.KeyDoor:
                if (isOpenedDoor)
                {
                     // open Door
                     doorHinge.DOLocalRotate(new Vector3(0, targetOpenAngle, 0), doorOpenDuration).SetEase(easeTypeOpen);
                     Debug.Log("Door Opned");
                }
                else 
                {
                    //close Door
                    CloseDoor();
                    Debug.Log("Door Locked");
                    DoKnobAnimation();
                }
                break;
            case DoorType.GhostDoor:
                if (isOpenedDoor && !isVictimTrapped)
                {
                    // open Door
                    doorHinge.DOLocalRotate(new Vector3(0, targetOpenAngle, 0), doorOpenDuration).SetEase(easeTypeOpen);
                    DoKnobAnimation();
                    canTrapVictim = true;
                    Debug.Log("Door Opened");
                }
                else
                {
                    DoKnobAnimation(); // just Do knob Animation
                }
                break;
        }

       
    }

    private void Update()
    {
        if (canTrapVictim && victimTransform != null)
        {
            DoTrapPlayer();
        }
    }

    private void DoTrapPlayer()
    {
        // Calculate Distanec
        float distanceToVictim = Vector3.Distance(transform.position, victimTransform.position);
        // Calculate the direction from the door to the victim
        Vector3 doorToVictimDirection = victimTransform.position - transform.position;

        // Calculate the direction from the door to the center of the door frame
        Vector3 doorToFrameCenterDirection = doorFrameCenter.position - transform.position;

        // Normalize both directions for the dot product
        doorToVictimDirection.Normalize();
        doorToFrameCenterDirection.Normalize();

        // Calculate the dot product between the two directions
        float dotProduct = Vector3.Dot(doorToVictimDirection, doorToFrameCenterDirection);
        //Debug.Log("Dot Product Difference" +" " + dotProduct);
        //Debug.Log("Distance Difference" + " " + distanceToVictim);
        // Check if the dot product is Greater than your threshold
        if (dotProduct > dotProductThreshold && distanceToVictim > doorCloseDistance)
        {
            Debug.Log("Victim Crossed Door");
            canTrapVictim = false;
            isVictimTrapped = true;
            CloseDoor();
            // Perform your door-closing action here
        }
    }

    private void CloseDoor()
    {
        doorHinge.DOLocalRotate(new Vector3(0, 0, 0), doorOpenDuration).SetEase(easeTypeClose); // Close Door
    }

    private void DoKnobAnimation()
    {
        doorKnob.DOLocalRotate(new Vector3(0, 0, targetKnobAngle), knobRotateDuration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            doorKnob.DOLocalRotate(new Vector3(0, 0, 0), knobRotateDuration).OnComplete(() =>
            {
                OnAnyDoorKnobAnimFinished?.Invoke(this, EventArgs.Empty);
            });
        });
    }

    private void CheckPlayerHasKey()
    {
        if (isGotKey) return;

        isGotKey = EventManager.Instance.InvokeTryOpenDoor(validKeySO);
    }

    public Transform GetHandTargetTransform()
    {
        return handTargetPos;
    }

}
