using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI & Koin")]
    public TextMeshProUGUI coinText;         
    public Image tankDisplayImage;    

    [Header("Kontrol Tombol Eksekusi Tank (Hierarchy)")]
    public GameObject btnBuyTank;      // Drag game object Btn_BuyTank dari Hierarchy ke sini
    public GameObject btnUseTank;      // Drag game object Btn_UseTank dari Hierarchy ke sini

    [Header("Aset Gambar Tombol Harga (Photoshop)")]
    public Sprite[] tombolHargaSprites; // Size = 3. Isi berurutan: sprite tombol "99", tombol "300", tombol "500"
    public Sprite tombolUseSprite;      // Masukkan sprite gambar tombol "USE" (Warna hijau)
    public Sprite tombolUsedSprite;     // Masukkan sprite gambar tombol "USED" (Jika ada, atau samakan dengan USE)

    [Header("Harga & Prefab Tank")]
    public GameObject[] tankPrefabs;  // Masukkan daftar prefab tank 1-3
    public Sprite[] tankSprites;      // Masukkan sprite visual tank 1-3 buat di menu
    public int[] hargaTank = { 0, 300, 500 }; 

    [Header("UI Pop-Up Tank (Panel_Notif)")]
    public GameObject panelNotifTank;       // Drag game object Panel_Notif ke sini
    public TextMeshProUGUI teksNotifTank;   // Drag teks harga/konfirmasi di dalam Panel_Notif

    [Header("Harga Senjata")]
    public int hargaPlasma = 50;
    public int hargaRoket = 100;
    public int hargaRambo = 150;

    [Header("UI Pop-Up Peluru (Panel_Notif_Peluru)")]
    public GameObject panelNotifPeluru;     // Drag game object Panel_Notif_Peluru ke sini
    public TextMeshProUGUI teksNotifPeluru; // Drag teks harga/konfirmasi di dalam Panel_Notif_Peluru

    private int TotalKoin;
    private int idTankDipilih = 0; 
    
    // Variabel penampung internal buat proses konfirmasi
    private int idSenjataDipilihSementara = -1;
    private int hargaSenjataSementara = 0;

    void Start()
    {
        TotalKoin = PlayerPrefs.GetInt("TotalKoin", 0);
        idTankDipilih = PlayerPrefs.GetInt("SelectedTank", 0);

        // Pastikan kedua panel pop-up tertutup saat game mulai
        if (panelNotifTank != null) panelNotifTank.SetActive(false);
        if (panelNotifPeluru != null) panelNotifPeluru.SetActive(false);

        UpdateCoinUI();
        UpdateTankShopDisplay();
    }

    // ==========================================
    // LOGIKA TOMBOL GESER & POP-UP TANK
    // ==========================================

    public void TombolTankNext()
    {
        idTankDipilih++;
        if (idTankDipilih >= tankSprites.Length) idTankDipilih = 0; 
        UpdateTankShopDisplay();
    }

    public void TombolTankPrev()
    {
        idTankDipilih--;
        if (idTankDipilih < 0) idTankDipilih = tankSprites.Length - 1; 
        UpdateTankShopDisplay();
    }

    public void TombolEksekusiTank()
    {
        string statusKey = "TankDimiliki_" + idTankDipilih;
        bool sudahPunya = (idTankDipilih == 0) || (PlayerPrefs.GetInt(statusKey, 0) == 1);

        if (sudahPunya)
        {
            // Jika sudah punya, langsung pakai tanpa pop-up
            AktivasiTank();
        }
        else
        {
            // Jika belum punya, buka Panel_Notif (Pop-up Tank)
            if (panelNotifTank != null)
            {
                panelNotifTank.SetActive(true);
                if (teksNotifTank != null)
                {
                    teksNotifTank.text = $"{hargaTank[idTankDipilih]}";
                }
            }
        }
    }

    // Dipanggil saat menekan tombol YES di Panel_Notif (Pop-up Tank)
    public void TombolTankYes()
    {
        int harga = hargaTank[idTankDipilih];
        if (TotalKoin >= harga)
        {
            TotalKoin -= harga;
            PlayerPrefs.SetInt("TotalKoin", TotalKoin);
            
            string statusKey = "TankDimiliki_" + idTankDipilih;
            PlayerPrefs.SetInt(statusKey, 1); // Tandai tank sudah dibeli
            
            AktivasiTank();
            UpdateCoinUI();
            Debug.Log($"[Shop] Sukses beli Tank indeks ke-{idTankDipilih}!");
        }
        else
        {
            Debug.LogWarning("[Shop] Koin tidak cukup untuk beli tank!");
        }

        TombolTankNo(); // Tutup panel setelah eksekusi
    }

    // Dipanggil saat menekan tombol NO di Panel_Notif (Pop-up Tank)
    public void TombolTankNo()
    {
        if (panelNotifTank != null) panelNotifTank.SetActive(false);
    }

    void AktivasiTank()
    {
        PlayerPrefs.SetInt("SelectedTank", idTankDipilih);
        
        // Peluru default balik ke Bullet biasa (0) saat ganti tank
        PlayerPrefs.SetInt("ActiveWeapon", 0);
        PlayerPrefs.Save();
        
        if (WeaponManager.Instance != null) WeaponManager.Instance.LoadWeapon();
        
        UpdateTankShopDisplay();
    }

    // KOREKSI UTAMA: Mengatur nyala-mati dua tombol & mengganti sprite tombol harga secara dinamis
    void UpdateTankShopDisplay()
    {
        if (tankDisplayImage != null && tankSprites.Length > idTankDipilih)
        {
            tankDisplayImage.sprite = tankSprites[idTankDipilih];
        }

        string statusKey = "TankDimiliki_" + idTankDipilih;
        bool sudahPunya = (idTankDipilih == 0) || (PlayerPrefs.GetInt(statusKey, 0) == 1);

        if (sudahPunya)
        {
            // 1. KONDISI SUDAH PUNYA: Aktifkan Btn_UseTank, Matikan Btn_BuyTank
            if (btnBuyTank != null) btnBuyTank.SetActive(false);
            if (btnUseTank != null) btnUseTank.SetActive(true);

            // Atur gambar Use atau Used pada Btn_UseTank
            Image imgUse = btnUseTank.GetComponent<Image>();
            if (imgUse != null)
            {
                int tankAktif = PlayerPrefs.GetInt("SelectedTank", 0);
                if (tankAktif == idTankDipilih)
                {
                    if (tombolUsedSprite != null) imgUse.sprite = tombolUsedSprite;
                }
                else
                {
                    if (tombolUseSprite != null) imgUse.sprite = tombolUseSprite;
                }
            }
        }
        else
        {
            // 2. KONDISI BELUM PUNYA: Aktifkan Btn_BuyTank, Matikan Btn_UseTank
            if (btnBuyTank != null) btnBuyTank.SetActive(true);
            if (btnUseTank != null) btnUseTank.SetActive(false);

            // Ganti sprite Btn_BuyTank secara otomatis sesuai indeks (99, 300, atau 500)
            Image imgBuy = btnBuyTank.GetComponent<Image>();
            if (imgBuy != null && tombolHargaSprites != null && tombolHargaSprites.Length > idTankDipilih)
            {
                imgBuy.sprite = tombolHargaSprites[idTankDipilih];
            }
        }
    }

    // ==========================================
    // LOGIKA TOMBOL & POP-UP PELURU
    // ==========================================

    public void BeliPlasmaLaser()
    {
        BukaPopUpPeluru(1, hargaPlasma, "Plasma Laser");
    }

    public void BeliHomingRocket()
    {
        BukaPopUpPeluru(2, hargaRoket, "Homing Rocket");
    }

    public void BeliRamboBurst()
    {
        BukaPopUpPeluru(3, hargaRambo, "Rambo Burst");
    }

    void BukaPopUpPeluru(int idSenjata, int harga, string namaSenjata)
    {
        idSenjataDipilihSementara = idSenjata;
        hargaSenjataSementara = harga;

        if (panelNotifPeluru != null)
        {
            panelNotifPeluru.SetActive(true);
            if (teksNotifPeluru != null)
            {
                teksNotifPeluru.text = $"{harga}";
            }
        }
    }

    public void TombolPeluruYes()
    {
        if (idSenjataDipilihSementara != -1 && TotalKoin >= hargaSenjataSementara)
        {
            TotalKoin -= hargaSenjataSementara;
            PlayerPrefs.SetInt("TotalKoin", TotalKoin);
            PlayerPrefs.SetInt("ActiveWeapon", idSenjataDipilihSementara);
            PlayerPrefs.Save();

            if (WeaponManager.Instance != null)
            {
                WeaponManager.Instance.ChangeWeaponInstantly(idSenjataDipilihSementara);
            }

            UpdateCoinUI();
            Debug.Log($"[Shop] Sukses beli senjata ID: {idSenjataDipilihSementara}");
        }
        else
        {
            Debug.LogWarning("[Shop] Koin kurang!");
        }

        TombolPeluruNo();
    }

    public void TombolPeluruNo()
    {
        idSenjataDipilihSementara = -1;
        hargaSenjataSementara = 0;
        if (panelNotifPeluru != null) panelNotifPeluru.SetActive(false);
    }

    void UpdateCoinUI()
    {
        TotalKoin = PlayerPrefs.GetInt("TotalKoin", 0);
        if (coinText != null)
        {
            coinText.text = TotalKoin.ToString();
        }
    }
}