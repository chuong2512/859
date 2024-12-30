using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public enum ConveyorType
    {
        Speeder,
        Slower
    }

    [SerializeField] private ConveyorType conveyorType;

    [SerializeField] private SwerveControllerConfig swerveControllerConfig;

    public void Action()
    {
        if (conveyorType == ConveyorType.Slower) swerveControllerConfig.SlowedDown();
        else if (conveyorType == ConveyorType.Speeder) swerveControllerConfig.SpeedUp();
    }

    public void StopAction()
    {
        swerveControllerConfig.DefoultSpeed();
    }
}