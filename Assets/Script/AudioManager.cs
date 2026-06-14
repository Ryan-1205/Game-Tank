using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip menuBGM;
    public AudioClip shopBGM;

    // Fungsi untuk memutar BGM Menu Utama
    public void PlayMenuBGM()
    {
        if (bgmSource.clip == menuBGM) return; // Biar lagu gak ngulang dari awal kalau udah muter
        bgmSource.clip = menuBGM;
        bgmSource.Play();
    }

    // Fungsi untuk memutar BGM Shop
    public void PlayShopBGM()
    {
        if (bgmSource.clip == shopBGM) return; // Biar gak ngulang dari awal kalau udah di shop
        bgmSource.clip = shopBGM;
        bgmSource.Play();
    }
}