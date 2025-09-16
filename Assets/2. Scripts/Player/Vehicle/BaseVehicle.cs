using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVehicle : MonoBehaviour
{
    public VehicleModel vehicleModel;
    public VehicleAnimHandler animHandler;

    protected virtual void Awake()
    {
        vehicleModel = new VehicleModel();
        animHandler = GetComponent<VehicleAnimHandler>();
    }
}
