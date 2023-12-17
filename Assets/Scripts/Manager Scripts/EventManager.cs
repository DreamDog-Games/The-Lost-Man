using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public event Action<GatherableSO> OnSlotItemButtonPerformed;

    public event Predicate<GatherableSO> OnPlayerPickupedItem;
    public event Predicate<GatherableSO> OnPlayerTryOpenDoor;

    public event EventHandler OnSelectedItemChanged;

    public event Action<GatherableSO> OnHealableItemUsed;

    public event EventHandler OnInventoryItemsModified;

    public event EventHandler OnInventoryOpened;
    public event EventHandler OninventoryClosed;

    public event Action<GatherableSO> OnEquipableItemEquipped;

    public event EventHandler OnStaminaFinished;
    public event EventHandler OnStaminaTopUpped;

    public event Action<Transform,TruckDoor> OnPlayerTryGetInTruck;

    public event EventHandler OnPlayerGetsInTruck;
    public event EventHandler OnPlayerGetsOutTruck;

    private void Awake()
    {
        Instance = this;
    }

    public bool InvokeTryPickupedItem(GatherableSO pickupItem)
    {
        return OnPlayerPickupedItem?.Invoke(pickupItem) == true;
    }

    public bool InvokeTryOpenDoor(GatherableSO validKeySO)
    {
        return OnPlayerTryOpenDoor?.Invoke(validKeySO) == true;
    }

    public void InvokeSelectedItemChanged()
    {
        OnSelectedItemChanged?.Invoke(this,EventArgs.Empty);
    }

    public void InvokeUseItemHealable(GatherableSO currentSelectedItem)
    {
        OnHealableItemUsed?.Invoke(currentSelectedItem);
    }

    public void InvokeInventoryItemsModified()
    {
        OnInventoryItemsModified?.Invoke(this,EventArgs.Empty);
    }

    public void InvokeInventoryOpened()
    {
        OnInventoryOpened?.Invoke(this,EventArgs.Empty);
    }

    public void InvokeInventoryClosed()
    {
        OninventoryClosed?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeEquipItemEquipable(GatherableSO currentSelectedItem)
    {
        OnEquipableItemEquipped?.Invoke(currentSelectedItem);
    }

    public void InvokeOnSlotItemBtnPerformed(GatherableSO item)
    {
        OnSlotItemButtonPerformed?.Invoke(item);
    }

    public void InvokeStaminaFinished()
    {
        OnStaminaFinished?.Invoke(this, EventArgs.Empty);
    }

    public void InvokeStaminaTopUpped()
    {
        OnStaminaTopUpped?.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerTryGetInTruck(Transform playerTransform, TruckDoor truckDoor)
    {
        OnPlayerTryGetInTruck?.Invoke(playerTransform,truckDoor);
    }
    public void InvokePlayerGetsInTruck()
    {
        OnPlayerGetsInTruck?.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerGetsOutTruck()
    {
       OnPlayerGetsOutTruck?.Invoke(this, EventArgs.Empty);
    }

}
