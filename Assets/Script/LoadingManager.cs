using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Grup UI (Canvas Group)")]
    public CanvasGroup grupJudul;   // Taruh Group_Titles ke sini
    public CanvasGroup grupLoading; // Taruh Group_Loading ke sini

    [Header("Elemen Loading")]
    public Slider sliderLoading;
    public TextMeshProUGUI textPersen;

    [Header("Pengaturan Waktu")]
    public float kecepatanFade = 0.8f;
    public float jedaTampilanJudul = 1.5f; // Berapa lama judul muncul sebelum bar jalan
    public string namaSceneTujuan = "Menu";

    void Start()
    {
        // Pastikan semua transparan di awal
        grupJudul.alpha = 0;
        grupLoading.alpha = 0;
        sliderLoading.value = 0;
        textPersen.text = "0%";

        // Mulai urutan sinematik
        StartCoroutine(UrutanSinematik());
    }

    IEnumerator UrutanSinematik()
    {
        // 1. FADE IN JUDUL & NAMA TIM
        while (grupJudul.alpha < 1)
        {
            grupJudul.alpha += Time.deltaTime * kecepatanFade;
            yield return null;
        }

        // 2. TUNGGU BENTAR BIAR PLAYER BACA JUDUL
        yield return new WaitForSeconds(jedaTampilanJudul);

        // 3. FADE IN LOADING BAR
        while (grupLoading.alpha < 1)
        {
            grupLoading.alpha += Time.deltaTime * kecepatanFade;
            yield return null;
        }

        // 4. MULAI LOADING ASINKRON (REAL)
        AsyncOperation operasi = SceneManager.LoadSceneAsync(namaSceneTujuan);
        operasi.allowSceneActivation = false;

        while (!operasi.isDone)
        {
            float progressTarget = Mathf.Clamp01(operasi.progress / 0.9f);

            // Gerakan slider & tank
            sliderLoading.value = Mathf.MoveTowards(sliderLoading.value, progressTarget, Time.deltaTime * 0.5f);
            textPersen.text = (sliderLoading.value * 100f).ToString("F0") + "%";

            if (sliderLoading.value >= 1f)
            {
                yield return new WaitForSeconds(0.8f); // Jeda dikit pas 100%
                operasi.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}