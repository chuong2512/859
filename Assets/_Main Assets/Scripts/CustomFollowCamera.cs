using Cinemachine;
using UnityEngine;

public class CustomFollowCamera : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public CinemachineBrain cinemachineBrain;
    public Transform playerTransform;
    public float xFollowRate = 0.5f;
    public float smoothTime = 0.3f; // The approximate time it will take to reach the target

    private Vector3 initialOffset;
    private Vector3 targetPosition;

    private Vector3
        velocity = Vector3.zero; // The current velocity, this value is modified by the function every time you call it

    private void Start()
    {
        initialOffset = vcam.transform.position - playerTransform.position;
        vcam.Follow = playerTransform;

        targetPosition = new Vector3(playerTransform.position.x * xFollowRate, playerTransform.position.y,
            playerTransform.position.z) + initialOffset;
        vcam.transform.position = targetPosition;
    }


    private void FixedUpdate()
    {
        if (cinemachineBrain.m_UpdateMethod == CinemachineBrain.UpdateMethod.FixedUpdate)
            UpdateCameraPosition();
    }

    private void Update()
    {
        if (cinemachineBrain.m_UpdateMethod == CinemachineBrain.UpdateMethod.SmartUpdate)
            UpdateCameraPosition();
    }

    private void LateUpdate()
    {
        if (cinemachineBrain.m_UpdateMethod == CinemachineBrain.UpdateMethod.LateUpdate)
            UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        targetPosition = new Vector3(playerTransform.position.x * xFollowRate, playerTransform.position.y,
            playerTransform.position.z);
        vcam.transform.position = new Vector3(
            Vector3.SmoothDamp(vcam.transform.position, targetPosition + initialOffset, ref velocity, smoothTime).x,
            Vector3.SmoothDamp(vcam.transform.position, targetPosition + initialOffset, ref velocity, smoothTime).y,
            targetPosition.z + initialOffset.z
        );
    }
}