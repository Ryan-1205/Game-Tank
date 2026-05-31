using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponShopManager : MonoBehaviour
{
    [System.Serializable]
    public struct AsetSenjata
    {
        public string namaSenjata;
        public Sprite spriteNormal;
        public Sprite spriteUdahBeli;
        public Sprite spriteSelected;
    }

    [Header("Pengaturan Pop-up Peluru")]
    public GameObject panelNotifPeluru;
    public TextMeshProUGUI txtHargaPeluru;

    [Header("Daftar Kartu/Tombol Peluru")]
    public Button[] tombolPeluru;
    public int[] hargaPeluru = { 67, 200, 500 };
    public bool[] isPeluruBought = new bool[3];

    [Header("9 Kumpulan Gambar dari Photoshop")]
    public AsetSenjata[] dataGambarSenjata;

    private int selectedWeaponIndex = -1;
    private int equippedWeaponIndex = -1; // Ini yang bakal nginget peluru aktif

    void Start()
    {
        if (panelNotifPeluru != null) panelNotifPeluru.SetActive(false);

        // Setup awal biar semua tombol matiin transition Unity-nya
        for (int i = 0; i < tombolPeluru.Length; i++)
        {
            tombolPeluru[i].transition = Selectable.Transition.None;
        }
    }

    public void KlikTombolPeluru(int index)
    {
        selectedWeaponIndex = index;

        if (isPeluruBought[index] == true)
        {
            EquipPeluru(index);
        }
        else
        {
            txtHargaPeluru.text = hargaPeluru[index].ToString();
            panelNotifPeluru.SetActive(true);
        }
    }

    public void BatalBeliPeluru()
    {
        panelNotifPeluru.SetActive(false);
    }

    public void KonfirmasiBeliPeluru()
    {
        if (selectedWeaponIndex != -1)
        {
            isPeluruBought[selectedWeaponIndex] = true;
            EquipPeluru(selectedWeaponIndex);
            panelNotifPeluru.SetActive(false);
        }
    }

    private void EquipPeluru(int index)
    {
        // 1. Kalo ada peluru yang lagi aktif sebelum ini, balikin gambarnya ke status "Udah Beli"
        if (equippedWeaponIndex != -1 && equippedWeaponIndex != index)
        {
            tombolPeluru[equippedWeaponIndex].GetComponent<Image>().sprite = dataGambarSenjata[equippedWeaponIndex].spriteUdahBeli;
        }

        // 2. Pasang index peluru yang baru
        equippedWeaponIndex = index;

        // 3. Ubah peluru baru ini jadi warna HIJAU (Selected)
        Image imgBaru = tombolPeluru[equippedWeaponIndex].GetComponent<Image>();
        imgBaru.sprite = dataGambarSenjata[equippedWeaponIndex].spriteSelected;
        //imgBaru.SetNativeSize();

        Debug.Log("Sekarang player pake peluru: " + dataGambarSenjata[equippedWeaponIndex].namaSenjata);
    }
}