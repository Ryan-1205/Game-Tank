using UnityEngine;
using Cinemachine;

public class CameraTargetBinder : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private bool targetTerpasang = false;

    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();

        if (vCam == null)
        {
            Debug.LogError("[CameraTargetBinder] CinemachineVirtualCamera tidak ditemukan!");
        }
    }

    void LateUpdate()
    {
        if (targetTerpasang) return;

        if (TankSpawner.Instance == null) return;

        if (TankSpawner.Instance.spawnedPlayer == null) return;

        Transform playerTransform = TankSpawner.Instance.spawnedPlayer.transform;

        vCam.Follow = playerTransform;
        vCam.LookAt = playerTransform;

        targetTerpasang = true;

        Debug.Log("[CameraTargetBinder] Kamera berhasil mengikuti player.");
    }
}