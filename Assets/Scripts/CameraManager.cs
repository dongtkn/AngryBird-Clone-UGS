using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera idleCam;
    [SerializeField] private CinemachineCamera followCam;

    private void Awake()
    {
        SwitchToIdleCam();
    }

    public void SwitchToIdleCam()
    {
        idleCam.enabled = true;
        followCam.enabled = false;
    }
    public void SwitchToFollowCam(Transform followTranform)
    {
        followCam.Follow = followTranform;
        idleCam.enabled = false;
        followCam.enabled = true;
    }
}
