using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVehicle : MonoBehaviour
{
    public VehicleModel vehicleModel;
    public VehicleAnimHandler animHandler;
    public VehicleHandler vehicleHandler;

    protected virtual void Awake()
    {
        GameManager.Unit.Vehicle = this;
        vehicleModel = new VehicleModel();
        animHandler = GetComponent<VehicleAnimHandler>();
        vehicleHandler = GetComponent<VehicleHandler>();
    }
}
