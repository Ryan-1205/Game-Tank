using UnityEngine;
using UnityEngine.SceneManagement; // WAJIB ADA: Buat mindahin scene

public class ExitButton : MonoBehaviour
{
    // Fungsi ini bakal kita panggil pas tombolnya di-klik
    public void KembaliKeMenu()
    {
        // Menyimpan data (koin dll) sebelum keluar biar aman
        PlayerPrefs.Save();

        // Membuka scene bernama "Menu" (pastikan huruf M-nya besar sesuai nama file lu)
        SceneManager.LoadScene("Menu");

        // Memastikan waktu berjalan normal lagi (jaga-jaga kalau gamenya lagi di-pause pas exit)
        Time.timeScale = 1f;
    }
}