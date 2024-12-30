using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AdjustCamera : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private CinemachineTransposer cinemachineTransposer;
    [SerializeField] private Player player;

    public Vector3 minFollow;
    public Vector3 maxFollow;

    public int targetCount;


    public float smooth;


    private Vector3 veloicty = Vector3.one;

    private void OnEnable()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        cinemachineTransposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        if (player)
        {
            var _targetOffset = Vector3.Lerp(minFollow, maxFollow, (float)player.bigMonies.Count / targetCount);
            cinemachineTransposer.m_FollowOffset = Vector3.SmoothDamp(cinemachineTransposer.m_FollowOffset,
                _targetOffset, ref veloicty, smooth);
        }
    }
}