using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip menuBGM;
    public AudioClip shopBGM;

    // Fungsi untuk memutar BGM Menu Utama
    public void PlayMenuBGM()
    {
        bgmSource.clip = menuBGM;
        bgmSource.Play();
    }

    // Fungsi untuk memutar BGM Shop
    public void PlayShopBGM()
    {
        bgmSource.clip = shopBGM;
        bgmSource.Play();
    }
}