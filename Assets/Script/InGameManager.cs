using UnityEngine;
using UnityEngine.SceneManagement; // Wajib dipanggil untuk urusan pindah scene

public class InGameManager : MonoBehaviour
{
    // Fungsi utama yang bakal dipanggil saat tombol Kembali diklik
    public void KembaliKeMenu()
    {
        // Ganti "Menu" sesuai dengan nama persis scene tokomu di folder Scenes
        SceneManager.LoadScene("Menu");
        
        Debug.Log("Kembali ke scene Menu Utama.");
    }
}