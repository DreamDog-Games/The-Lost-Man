using ProjectMiamiTestInventory;
using System;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public static Torch Instance { get; private set; }

    public event EventHandler OnTorchEquiped;
    public event EventHandler OnTorchUnEquiped;

    [SerializeField] private GatherableSO batterySO;   // Ref to the BatterySo 
    [SerializeField] private Light torchFlash;         // Ref to the BatterySo 

    private GatherableSO currentUsingBatterySO;
    [SerializeField] private float currentBatteryHealth = 0;
    public bool canUseTorch = false;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        OnTorchEquiped?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        currentUsingBatterySO = Instantiate(batterySO);            // Creating new instance So scene Pickup BAtteries Dont Affect
        InputManager.Instance.OnReloadKeyPerformed += InputManager_Instance_OnReloadKeyPerformed;

        currentBatteryHealth = currentUsingBatterySO.value;

        OnTorchEquiped?.Invoke(this, EventArgs.Empty);
    }

    private void InputManager_Instance_OnReloadKeyPerformed(object sender, EventArgs e)
    {
        if (!gameObject.activeInHierarchy) return; // When Torch Only Active

        if (InventoryTest.Instance.TryGetGatherableObject(batterySO,out GatherableSO newBatterySO))
        {
            RemoveOldBatery();
            LoadNewBattery(newBatterySO);
        }
        else
        {
            Debug.Log("Dont Have Batteries To Reload");
        }
    }

    private void Update()
    {
        currentBatteryHealth -= Time.deltaTime;

        if(currentBatteryHealth <  0)
        {
            currentBatteryHealth = 0;
            canUseTorch = false;
        }
        else
        {
            canUseTorch = true;
        }

        if(canUseTorch)
        {
            // Light Working
            SetActiveFlash(true);
        }
        else
        {
            // Battery Dead
            SetActiveFlash(false);
        }
    }

    private void RemoveOldBatery()
    {
       currentUsingBatterySO = null;
       Debug.Log("Old Battery Removed");
    }
    private void LoadNewBattery(GatherableSO batterySO)
    {
       currentUsingBatterySO = batterySO;
       currentBatteryHealth = currentUsingBatterySO.value;
       Debug.Log("New Battery Loaded");
    }

    private void SetActiveFlash(bool active)
    {
        torchFlash.enabled = active;
    }

    public float CurrentBatteryHealth()
    {
        return currentBatteryHealth;
    }

    public float GetBatteryMaxHealth()
    {
        return batterySO.value;
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy && canUseTorch;
    }
    private void OnDisable()
    {
        //InputManager.Instance.OnReloadKeyPerformed -= InputManager_Instance_OnReloadKeyPerformed;
        OnTorchUnEquiped?.Invoke(this, EventArgs.Empty);
    }


}
