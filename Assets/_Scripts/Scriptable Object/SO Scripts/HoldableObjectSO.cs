using UnityEngine;

[CreateAssetMenu()]
public class HoldableObjectSO : ScriptableObject
{
    public string Name;
    public float throwForce = 0;
    public Vector3 holdOffset;
    public Vector3 holdOffsetRot;
    public bool canPlaceInTruck = false;
}

