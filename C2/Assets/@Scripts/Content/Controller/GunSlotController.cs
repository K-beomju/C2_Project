using System.Collections.Generic;
using C2Project.Setting;
using UnityEngine;

public class GunSlotController : MonoBehaviour
{
    [SerializeField] private Transform[] gunSlots;

    public void AmountGun((string itemId, int equipIndex) gunData)
    {
        GameObject gameObject = new GameObject(gunData.itemId);
        gameObject.transform.position = gunSlots[gunData.equipIndex].position;
    }
}
