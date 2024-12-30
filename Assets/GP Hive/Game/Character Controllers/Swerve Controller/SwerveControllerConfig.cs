using UnityEngine;

[CreateAssetMenu(fileName = "Swerve Config", menuName = "GP Hive Objects/Character Controllers/Swerve Config")]
public class SwerveControllerConfig : ScriptableObject
{
    public float forwardSpeed;
    [SerializeField] private float forwardSpeederSpeed;
    [SerializeField] private float forwardSlowerSpeed;
    [SerializeField] private float forwardDefoultSpeed;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float sensitivity;
    [SerializeField] private float swerveLimit;
    [SerializeField] private float inputResetTime;
    [SerializeField] private float threshold;

    [SerializeField] private float smooth;

    //  public float ForwardSpeed => forwardSpeed;
    public float Smooth => smooth;
    public float HorizontalSpeed => horizontalSpeed;
    public float Sensitivity => sensitivity;
    public float SwerveLimit => swerveLimit;
    public float InputResetTime => inputResetTime;
    public float Threshold => threshold;

    public void SpeedUp()
    {
        forwardSpeed = forwardSpeederSpeed;
    }

    public void SlowedDown()
    {
        forwardSpeed = forwardSlowerSpeed;
    }

    public void DefoultSpeed()
    {
        forwardSpeed = forwardDefoultSpeed;
    }
}