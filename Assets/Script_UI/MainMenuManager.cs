using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // ==========================================
    // 1. FUNGSI UTAMA & AUDIO
    // ==========================================
    public void PlayGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void MuteAudio()
    {
        AudioListener.volume = 0f;
        Debug.Log("Audio Bisu!");
    }

    public void UnmuteAudio()
    {
        AudioListener.volume = 1f;
        Debug.Log("Audio Nyala!");
    }

    // ==========================================
    // 2. FUNGSI LOGIKA SHOP (Beli Item)
    // ==========================================
    // Catatan untuk teman teman ku tolong dibaca ya note ini  nanti tinggal ditambahin pengecekan skor/uang di sini

    public void BeliSkin(int nomorSkin)
    {
        Debug.Log("Tombol ditekan: Mencoba membeli SKIN nomor " + nomorSkin);
        // Contoh logika masa depan: if (uang >= harga) { uang -= harga; unlockSkin; }
    }

    public void BeliSenjata(int nomorSenjata)
    {
        Debug.Log("Tombol ditekan: Mencoba membeli SENJATA nomor " + nomorSenjata);
        // Contoh logika masa depan: if (uang >= harga) { uang -= harga; unlockSenjata; }
    }
}