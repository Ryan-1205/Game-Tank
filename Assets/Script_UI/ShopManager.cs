using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Pengaturan Tank")]
    public Image tankDisplay;
    public Sprite[] tankSprites;

    [Header("Pengaturan Tombol Bawah")]
    public Image imgBuyUseBtn;
    public Sprite[] buySprites;
    public Sprite useSprite;
    public bool[] isTankBought;

    [Header("Pengaturan Pop-up Notif")]
    public GameObject panelNotif; // Wadah buat Panel_Notif

    private int currentIndex = 0;

    void Start()
    {
        if (isTankBought.Length != tankSprites.Length)
        {
            System.Array.Resize(ref isTankBought, tankSprites.Length);
        }

        // Mastiin pas mulai game, panel notif ketutup
        if (panelNotif != null) panelNotif.SetActive(false);

        UpdateTampilanTank();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) GeserKanan();
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) GeserKiri();
    }

    public void GeserKanan()
    {
        currentIndex++;
        if (currentIndex >= tankSprites.Length) currentIndex = 0;
        UpdateTampilanTank();
    }

    public void GeserKiri()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = tankSprites.Length - 1;
        UpdateTampilanTank();
    }

    private void UpdateTampilanTank()
    {
        tankDisplay.sprite = tankSprites[currentIndex];

        if (isTankBought[currentIndex] == true)
        {
            imgBuyUseBtn.sprite = useSprite;
        }
        else
        {
            imgBuyUseBtn.sprite = buySprites[currentIndex];
        }

        imgBuyUseBtn.SetNativeSize();
    }

    // --- FUNGSI BARU BUAT TOMBOL ---

    public void KlikTombolBuyUse()
    {
        if (isTankBought[currentIndex] == true)
        {
            Debug.Log("Tank udah kebeli! Sekarang lagi dipake.");
            // Nanti logika "Use" / milih tank buat di game ditaruh di sini
        }
        else
        {
            // Kalau belum beli, munculin pop-up konfirmasi
            if (panelNotif != null) panelNotif.SetActive(true);
        }
    }

    public void BatalBeli() // Ini buat tombol NO
    {
        if (panelNotif != null) panelNotif.SetActive(false);
    }

    public void KonfirmasiBeli() // Ini buat tombol YES
    {
        // 1. Ubah status tank jadi udah dibeli
        isTankBought[currentIndex] = true;

        // 2. Tutup panel pop-up
        if (panelNotif != null) panelNotif.SetActive(false);

        // 3. Update gambar di layar biar langsung berubah jadi "USE"
        UpdateTampilanTank();
    }
}