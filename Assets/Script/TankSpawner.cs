using UnityEngine;
using Cinemachine; // Sudah benar menggunakan namespace ini untuk versi 2.10.7

public class TankSpawner : MonoBehaviour
{
    // Singleton Instance tetap dipertahankan untuk EnemySpawner
    public static TankSpawner Instance;

    [Header("Daftar Prefab Tank")]
    public GameObject[] tankPrefabs; 

    [Header("Titik Spawn")]
    public Transform spawnPoint;     

    // Wadah penampung data player untuk EnemySpawner
    [HideInInspector] public GameObject spawnedPlayer;

    void Awake()
    {
        // KOREKSI 1: Amankan deklarasi Instance di baris paling pertama Awake
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        int idTankAktif = PlayerPrefs.GetInt("SelectedTank", 0);

        if (tankPrefabs != null && tankPrefabs.Length > idTankAktif)
        {
            if (tankPrefabs[idTankAktif] != null)
            {
                // Ambil posisi spawn (gunakan posisi spawner jika titik spawn kosong di inspector)
                Vector3 pos = (spawnPoint != null) ? spawnPoint.position : transform.position;
                Quaternion rot = (spawnPoint != null) ? spawnPoint.rotation : transform.rotation;

                // Spawn tank dan simpan ke variabel spawnedPlayer
                spawnedPlayer = Instantiate(tankPrefabs[idTankAktif], pos, rot);
                Debug.Log($"[Spawner] Berhasil memunculkan Tank Indeks ke-{idTankAktif} di gameplay.");

                // KOREKSI 2: Gunakan FindObjectOfType untuk Cinemachine versi 2.x bawaan project-mu
                CinemachineVirtualCamera vcam = Object.FindObjectOfType<CinemachineVirtualCamera>();

                if (vcam != null)
                {
                    vcam.Follow = spawnedPlayer.transform;
                    vcam.LookAt = spawnedPlayer.transform;
                    Debug.Log("[Spawner] Kamera Cinemachine v2 berhasil mengikuti tank baru.");
                }
                else
                {
                    Debug.LogWarning("[Spawner] CinemachineVirtualCamera tidak ditemukan di scene!");
                }
            }
            else
            {
                Debug.LogError("[Spawner] Prefab tank di indeks tersebut kosong/belum di-drag!");
            }
        }
        else
        {
            Debug.LogError("[Spawner] Indeks tank ngaco atau array tankPrefabs belum diisi!");
        }
    }

    void Start()
    {
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.LoadWeapon();
        }
    }
}