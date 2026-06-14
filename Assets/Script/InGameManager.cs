using UnityEngine;
using UnityEngine.SceneManagement; // Wajib dipanggil untuk urusan pindah scene

public class InGameManager : MonoBehaviour
{
    // Slot untuk memasukkan AudioSource suara klik tombol di Inspector
    public AudioSource sfxKlikInGameSource;

    // Fungsi utama yang bakal dipanggil saat tombol Kembali diklik
    public void KembaliKeMenu()
    {
        // 1. Putar suara klik dulu sebelum scene-nya pindah/berganti
        MainkanSuaraKlikInGame();

        // Ganti "Menu" sesuai dengan nama persis scene tokomu di folder Scenes
        SceneManager.LoadScene("Menu");
        
        Debug.Log("Kembali ke scene Menu Utama.");
    }

    // Fungsi pembantu untuk memutar sfx klik
    public void MainkanSuaraKlikInGame()
    {
        if (sfxKlikInGameSource != null)
        {
            sfxKlikInGameSource.PlayOneShot(sfxKlikInGameSource.clip);
        }
    }
}