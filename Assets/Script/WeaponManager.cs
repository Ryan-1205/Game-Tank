using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    [Header("Daftar Prefab Peluru")]
    public GameObject normalBulletPrefab;
    public GameObject plasmaLaserPrefab;
    public GameObject homingRocketPrefab;
    public GameObject ramboBurstPrefab;

    // Prefab yang saat ini sedang aktif digunakan untuk menembak
    [HideInInspector] public GameObject currentBulletPrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        
        // Setiap kali ganti scene atau tank baru lahir, load senjata yang dibeli
        LoadWeapon();
    }

    // Fungsi untuk memuat senjata berdasarkan data yang tersimpan
    public void LoadWeapon()
    {
        // Default (0) = normal bullet
        int activeWeapon = PlayerPrefs.GetInt("ActiveWeapon", 0); 

        switch (activeWeapon)
        {
            case 0:
                currentBulletPrefab = normalBulletPrefab;
                break;
            case 1:
                currentBulletPrefab = plasmaLaserPrefab;
                break;
            case 2:
                currentBulletPrefab = homingRocketPrefab;
                break;
            case 3:
                currentBulletPrefab = ramboBurstPrefab;
                break;
            default:
                currentBulletPrefab = normalBulletPrefab;
                break;
        }
        Debug.Log($"[WeaponManager] Senjata aktif diatur ke indeks: {activeWeapon}");
    }

    // Fungsi instan yang dipanggil oleh Shop agar tank langsung ganti peluru saat itu juga
    public void ChangeWeaponInstantly(int weaponIndex)
    {
        PlayerPrefs.SetInt("ActiveWeapon", weaponIndex);
        PlayerPrefs.Save();
        LoadWeapon();
    }
}