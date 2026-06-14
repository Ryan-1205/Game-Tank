using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // ==========================================
    // 1. FUNGSI UTAMA & AUDIO
    // ==========================================
    
    // Slot untuk memasukkan AudioSource khusus SFX Klik Tombol
    public AudioSource sfxKlikSource; 

    public void PlayGame()
    {
        // Panggil suara klik dulu sebelum pindah scene
        MainkanSuaraKlik();
        SceneManager.LoadScene("InGame");
    }

    // FUNGSI BARU: Untuk dipanggil oleh tombol-tombol lain
    public void MainkanSuaraKlik()
    {
        if (sfxKlikSource != null)
        {
            sfxKlikSource.PlayOneShot(sfxKlikSource.clip);
        }
    }

    public void MuteAudio()
    {
        MainkanSuaraKlik(); // Biar pas pencet mute ada suara kliknya dulu
        AudioListener.volume = 0f;
        Debug.Log("Audio Bisu!");
    }

    public void UnmuteAudio()
    {
        MainkanSuaraKlik(); // Biar pas pencet unmute ada suara kliknya dulu
        AudioListener.volume = 1f;
        Debug.Log("Audio Nyala!");
    }

    // ==========================================
    // 2. FUNGSI LOGIKA SHOP (Beli Item)
    // ==========================================
    public void BeliSkin(int nomorSkin)
    {
        MainkanSuaraKlik(); // Suara klik pas beli skin
        Debug.Log("Tombol ditekan: Mencoba membeli SKIN nomor " + nomorSkin);
    }

    public void BeliSenjata(int nomorSenjata)
    {
        MainkanSuaraKlik(); // Suara klik pas beli senjata
        Debug.Log("Tombol ditekan: Mencoba membeli SENJATA nomor " + nomorSenjata);
    }
}