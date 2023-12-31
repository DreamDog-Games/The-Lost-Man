using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public static Prefabs Instance { get; private set; }

    [SerializeField] private PrefabSO prefabSO;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetCoconutPrefab()
    {
        return prefabSO.CoconutPrefab;
    }
    public GameObject GetStonePrefab()
    {
        return prefabSO.StonePrefab;
    }
    public GameObject GetEnemyPrefab()
    {
        return prefabSO.EnemyPrefab;
    }
    public GameObject GetEnemyDummyPrefab()
    {
        return prefabSO.EnemyDummyPrefab;
    }

    public GameObject GetPlayerPrefab()
    {
        return prefabSO.PlayerPrefab;
    }

    public GameObject GetInventorySlotPrefab()
    {
        return prefabSO.InventorySlotPrefab;
    }

    public GameObject GetApplePrefab()
    {
        return prefabSO.ApplePrefab;
    }

    public GameObject GetInventorySlotItemTemplate()
    {
        return prefabSO.InventorySlotItemTemplatePrefab;
    }
    public GameObject GetInventoryEquipSlotItemTemplate()
    {
        return prefabSO.InventoryEquipSlotItemTemplatePrefab;
    }

    public GameObject GetItemInfoSingleUiPrefab()
    {
        return prefabSO.ItemInfoSingleUiTemplatePrefab;
    }
}
