using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EquipSlotType
{
    Main,Aditional
}
public class EquipItemSlot : InventorySlot,IBeginDragHandler
{
    public EquipSlotType equipSlotType;
    protected override void RefreshSlotData()
    {
        var draggableItem = GetComponentInChildren<DraggableItem>();
        if (draggableItem == null)
        {
            item = null;

            ItemInfoHandlerUI.Instance.ClearInfoObject();
            EventManager.Instance.InvokeOnEquipSlotModified(this, null);
        }
    }

    protected override void UpdateVisual()
    {
        var draggableItem = GetComponentInChildren<DraggableItem>();
        if (item == null) 
        { 
            Destroy(draggableItem.gameObject); return;
        }
        

        if (draggableItem == null && item != null)
        {
            if (itemPrefab != null)
            {
                Destroy(itemPrefab);
            }

            itemPrefab = Instantiate(Prefabs.Instance.GetInventoryEquipSlotItemTemplate(), transform);

            if (itemPrefab.TryGetComponent(out DraggableItem draggable))
            {
                draggable.SetGatherableObjSO(item);
                draggable.UpdateSlotImage();
            }
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject droppedObj = eventData.pointerDrag;
            if (droppedObj.TryGetComponent(out DraggableItem droppedItem))
            {
                if (droppedItem.GetGatherableSO() == item) return;

                SetItem(droppedItem.GetGatherableSO());
                UpdateVisual();

                if (droppedItem.GetGatherableSO() != null)
                {
                    EventManager.Instance.InvokeOnEquipSlotModified(this,droppedItem.GetGatherableSO());
                }
                else
                {
                    Debug.LogError("Draggable item Not Has Gatherable SO");
                }
                OnSlotItemModified?.Invoke();
                Destroy(droppedObj);
                
            }
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        item = null;
    }
}
