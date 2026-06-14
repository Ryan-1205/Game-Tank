using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI & Koin")]
    public TextMeshProUGUI coinText;
    public Image tankDisplayImage;

    [Header("Kontrol Tombol Eksekusi Tank (Hierarchy)")]
    public GameObject btnBuyTank;
    public GameObject btnUseTank;

    [Header("Aset Gambar Tombol (Photoshop)")]
    public Sprite[] tombolHargaSprites;   // Masukin array gambar harga tank (Tank 0, 1, 2)
    public Sprite tombolUseSprite;          // MASUKIN GAMBAR: Grafis tulisan "USE" (Udah beli tapi belum dipake)
    public Sprite tombolUsedSprite;         // MASUKIN GAMBAR: Grafis tulisan "USED" / "SELECTED" (Lagi dipake)

    [Header("Gambar Dasar Senjata Before Dibeli")]
    public Sprite normalSpritePlasma;     // MASUKIN GAMBAR: Tombol dasar Plasma Laser asli lu
    public Sprite normalSpriteRoket;      // MASUKIN GAMBAR: Tombol dasar Homing Rocket asli lu
    public Sprite normalSpriteRambo;      // MASUKIN GAMBAR: Tombol dasar Rambo Burst asli lu

    [Header("Harga & Prefab Tank")]
    public GameObject[] tankPrefabs;
    public Sprite[] tankSprites;
    public int[] hargaTank = { 0, 300, 500 };

    [Header("UI Pop-Up Tank (Panel_Notif)")]
    public GameObject panelNotifTank;
    public TextMeshProUGUI teksNotifTank;

    [Header("Harga Senjata")]
    public int hargaPlasma = 50;
    public int hargaRoket = 100;
    public int hargaRambo = 150;

    [Header("UI Pop-Up Peluru (Panel_Notif_Peluru)")]
    public GameObject panelNotifPeluru;
    public TextMeshProUGUI teksNotifPeluru;

    [Header("KOMPONEN SENJATA (DYNAMIC TEXT)")]
    public TextMeshProUGUI teksTombolPlasma; // Tarik objek Text (TMP) dari dalam Btn_Senjata1
    public TextMeshProUGUI teksTombolRoket;  // Tarik objek Text (TMP) dari dalam Btn_Senjata2
    public TextMeshProUGUI teksTombolRambo;  // Tarik objek Text (TMP) dari dalam Btn_Senjata3

    [Header("KOMPONEN SENJATA (GRAPHIC PHOTOSHOP)")]
    public Image imgTombolPlasma; // Tarik Btn_Senjata1 ke sini
    public Image imgTombolRoket;  // Tarik Btn_Senjata2 ke sini
    public Image imgTombolRambo;  // Tarik Btn_Senjata3 ke sini

    private int TotalKoin;
    private int idTankDipilih = 0;

    private int idSenjataDipilihSementara = -1;
    private int hargaSenjataSementara = 0;

    // FUNGSI BAWAAN UNITY: Otomatis jalan setiap kali Panel Toko diaktifkan/dibuka kembali
    void OnEnable()
    {
        TotalKoin = PlayerPrefs.GetInt("TotalKoin", 0);
        idTankDipilih = PlayerPrefs.GetInt("SelectedTank", 0);

        UpdateCoinUI();
        UpdateTankShopDisplay();
        UpdateWeaponShopVisual(); // Paksa visual senjata nge-refresh pas toko dibuka kembali
    }

    void Start()
    {
        if (panelNotifTank != null) panelNotifTank.SetActive(false);
        if (panelNotifPeluru != null) panelNotifPeluru.SetActive(false);

        TotalKoin = PlayerPrefs.GetInt("TotalKoin", 0);
        idTankDipilih = PlayerPrefs.GetInt("SelectedTank", 0);

        UpdateCoinUI();
        UpdateTankShopDisplay();
        UpdateWeaponShopVisual();
    }

    // ==========================================
    // LOGIKA TANK
    // ==========================================

    public void TombolTankNext()
    {
        idTankDipilih++;
        if (idTankDipilih >= tankSprites.Length) idTankDipilih = 0;
        UpdateTankShopDisplay();
        UpdateWeaponShopVisual(); // Biar peluru ga ke-reset visualnya pas mindah tank
    }

    public void TombolTankPrev()
    {
        idTankDipilih--;
        if (idTankDipilih < 0) idTankDipilih = tankSprites.Length - 1;
        UpdateTankShopDisplay();
        UpdateWeaponShopVisual(); // Biar peluru ga ke-reset visualnya pas mindah tank
    }

    public void TombolEksekusiTank()
    {
        string statusKey = "TankDimiliki_" + idTankDipilih;
        bool sudahPunya = (idTankDipilih == 0) || (PlayerPrefs.GetInt(statusKey, 0) == 1);

        if (sudahPunya) { AktivasiTank(); }
        else if (panelNotifTank != null) { panelNotifTank.SetActive(true); if (teksNotifTank != null) teksNotifTank.text = $"{hargaTank[idTankDipilih]}"; }
    }

    public void TombolTankYes()
    {
        int harga = hargaTank[idTankDipilih];
        if (TotalKoin >= harga)
        {
            TotalKoin -= harga;
            PlayerPrefs.SetInt("TotalKoin", TotalKoin);
            PlayerPrefs.SetInt("TankDimiliki_" + idTankDipilih, 1);
            AktivasiTank();
            UpdateCoinUI();
        }
        TombolTankNo();
    }

    public void TombolTankNo() { if (panelNotifTank != null) panelNotifTank.SetActive(false); }

    void AktivasiTank()
    {
        PlayerPrefs.SetInt("SelectedTank", idTankDipilih);
        PlayerPrefs.Save();
        if (WeaponManager.Instance != null) WeaponManager.Instance.LoadWeapon();
        UpdateTankShopDisplay();
        UpdateWeaponShopVisual(); // Refresh visual senjata setelah tank berhasil dipakai/dibeli
    }

    void UpdateTankShopDisplay()
    {
        if (tankDisplayImage != null && tankSprites.Length > idTankDipilih) tankDisplayImage.sprite = tankSprites[idTankDipilih];

        string statusKey = "TankDimiliki_" + idTankDipilih;
        bool sudahPunya = (idTankDipilih == 0) || (PlayerPrefs.GetInt(statusKey, 0) == 1);

        if (sudahPunya)
        {
            if (btnBuyTank != null) btnBuyTank.SetActive(false);
            if (btnUseTank != null) btnUseTank.SetActive(true);
            Image imgUse = btnUseTank.GetComponent<Image>();
            if (imgUse != null) imgUse.sprite = (PlayerPrefs.GetInt("SelectedTank", 0) == idTankDipilih && tombolUsedSprite != null) ? tombolUsedSprite : tombolUseSprite;
        }
        else
        {
            if (btnBuyTank != null) btnBuyTank.SetActive(true);
            if (btnUseTank != null) btnUseTank.SetActive(false);
            Image imgBuy = btnBuyTank.GetComponent<Image>();
            if (imgBuy != null && tombolHargaSprites != null && tombolHargaSprites.Length > idTankDipilih) imgBuy.sprite = tombolHargaSprites[idTankDipilih];
        }
    }

    // ==========================================
    // LOGIKA PELURU / SENJATA
    // ==========================================

    public void BeliPlasmaLaser() { ProsesKlikTombolSenjata(1, hargaPlasma, "Plasma Laser"); }
    public void BeliHomingRocket() { ProsesKlikTombolSenjata(2, hargaRoket, "Homing Rocket"); }
    public void BeliRamboBurst() { ProsesKlikTombolSenjata(3, hargaRambo, "Rambo Burst"); }

    void ProsesKlikTombolSenjata(int idSenjata, int harga, string namaSenjata)
    {
        string statusKey = "SenjataDimiliki_" + idSenjata;
        bool sudahPunya = PlayerPrefs.GetInt(statusKey, 0) == 1;

        if (sudahPunya)
        {
            PlayerPrefs.SetInt("ActiveWeapon", idSenjata);
            PlayerPrefs.Save();
            if (WeaponManager.Instance != null) WeaponManager.Instance.ChangeWeaponInstantly(idSenjata);
            UpdateWeaponShopVisual();
        }
        else
        {
            idSenjataDipilihSementara = idSenjata;
            hargaSenjataSementara = harga;
            if (panelNotifPeluru != null)
            {
                panelNotifPeluru.SetActive(true);
                if (teksNotifPeluru != null) teksNotifPeluru.text = $"{harga}";
            }
        }
    }

    public void TombolPeluruYes()
    {
        if (idSenjataDipilihSementara != -1 && TotalKoin >= hargaSenjataSementara)
        {
            TotalKoin -= hargaSenjataSementara;
            PlayerPrefs.SetInt("TotalKoin", TotalKoin);
            PlayerPrefs.SetInt("SenjataDimiliki_" + idSenjataDipilihSementara, 1);
            PlayerPrefs.SetInt("ActiveWeapon", idSenjataDipilihSementara);
            PlayerPrefs.Save();

            if (WeaponManager.Instance != null) WeaponManager.Instance.ChangeWeaponInstantly(idSenjataDipilihSementara);

            UpdateCoinUI();
            UpdateWeaponShopVisual();
        }
        TombolPeluruNo();
    }

    public void TombolPeluruNo()
    {
        idSenjataDipilihSementara = -1;
        hargaSenjataSementara = 0;
        if (panelNotifPeluru != null) panelNotifPeluru.SetActive(false);
    }

    void UpdateWeaponShopVisual()
    {
        int senjataAktif = PlayerPrefs.GetInt("ActiveWeapon", 0);

        // --- 1. SETTING VISUAL PLASMA (ID: 1) ---
        bool punyaPlasma = PlayerPrefs.GetInt("SenjataDimiliki_1", 0) == 1;
        if (punyaPlasma)
        {
            if (imgTombolPlasma != null)
            {
                Button btn = imgTombolPlasma.GetComponent<Button>();
                if (senjataAktif == 1)
                {
                    imgTombolPlasma.sprite = tombolUsedSprite;
                    if (btn != null) btn.transition = Selectable.Transition.None;
                }
                else
                {
                    imgTombolPlasma.sprite = tombolUseSprite;
                    if (btn != null) btn.transition = Selectable.Transition.SpriteSwap;
                }
            }
            if (teksTombolPlasma != null) teksTombolPlasma.text = "";
        }
        else
        {
            if (imgTombolPlasma != null && normalSpritePlasma != null)
            {
                imgTombolPlasma.sprite = normalSpritePlasma;
                Button btn = imgTombolPlasma.GetComponent<Button>();
                if (btn != null) btn.transition = Selectable.Transition.SpriteSwap;
            }
            if (teksTombolPlasma != null) teksTombolPlasma.text = hargaPlasma.ToString();
        }

        // --- 2. SETTING VISUAL ROKET (ID: 2) ---
        bool punyaRoket = PlayerPrefs.GetInt("SenjataDimiliki_2", 0) == 1;
        if (punyaRoket)
        {
            if (imgTombolRoket != null)
            {
                Button btn = imgTombolRoket.GetComponent<Button>();
                if (senjataAktif == 2)
                {
                    imgTombolRoket.sprite = tombolUsedSprite;
                    if (btn != null) btn.transition = Selectable.Transition.None;
                }
                else
                {
                    imgTombolRoket.sprite = tombolUseSprite;
                    if (btn != null) btn.transition = Selectable.Transition.SpriteSwap;
                }
            }
            if (teksTombolRoket != null) teksTombolRoket.text = "";
        }
        else
        {
            if (imgTombolRoket != null && normalSpriteRoket != null)
            {
                imgTombolRoket.sprite = normalSpriteRoket;
                Button btn = imgTombolRoket.GetComponent<Button>();
                if (btn != null) btn.transition = Selectable.Transition.SpriteSwap;
            }
            if (teksTombolRoket != null) teksTombolRoket.text = hargaRoket.ToString();
        }

        // --- 3. SETTING VISUAL RAMBO (ID: 3) ---
        bool punyaRambo = PlayerPrefs.GetInt("SenjataDimiliki_3", 0) == 1;
        if (punyaRambo)
        {
            if (imgTombolRambo != null)
            {
                Button btn = imgTombolRambo.GetComponent<Button>();
                if (senjataAktif == 3)
                {
                    imgTombolRambo.sprite = tombolUsedSprite;
                    if (btn != null) btn.transition = Selectable.Transition.None;
                }
                else
                {
                    imgTombolRambo.sprite = tombolUseSprite;
                    if (btn != null) btn.transition = Selectable.Transition.SpriteSwap;
                }
            }
            if (teksTombolRambo != null) teksTombolRambo.text = "";
        }
        else
        {
            if (imgTombolRambo != null && normalSpriteRambo != null)
            {
                imgTombolRambo.sprite = normalSpriteRambo;
                Button btn = imgTombolRambo.GetComponent<Button>();
                if (btn != null) btn.transition = Selectable.Transition.SpriteSwap;
            }
            if (teksTombolRambo != null) teksTombolRambo.text = hargaRambo.ToString();
        }
    }

    void UpdateCoinUI()
    {
        TotalKoin = PlayerPrefs.GetInt("TotalKoin", 0);
        if (coinText != null) coinText.text = TotalKoin.ToString();
    }
}