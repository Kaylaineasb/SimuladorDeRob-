using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Robot"))
        {
            BatteryManager batteryManager = other.GetComponent<BatteryManager>();
            if (batteryManager != null)
            {
                batteryManager.RechargeBattery();
            }
        }
    }
}
