using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Menü Paneli")]
    public GameObject pauseMenuPanel;

    [Header("Ses Ayarlarý")]
    public Image soundButtonImage;
    public AudioSource[] audioSources; // Dýþarýdan atanacak AudioSource'lar
    private bool isMuted = false;

    void Start()
    {
        // Kaydedilmiþ ses durumunu yükle
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        ApplyMuteToAudioSources();
        UpdateSoundIcon();
    }

    // Resume butonu için
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Pause butonu için
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Ses aç/kapa butonu için
    public void ToggleSound()
    {
        isMuted = !isMuted;
        ApplyMuteToAudioSources();
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        UpdateSoundIcon();
    }

    // AudioSource'larýn mute durumunu güncelle
    void ApplyMuteToAudioSources()
    {
        if (audioSources != null)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource != null)
                {
                    audioSource.mute = isMuted;
                }
            }
        }
    }

    // Ana menüye dön
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mehmet");
    }

    // Oyundan çýk
    public void Quit()
    {
        Debug.Log("Oyun kapatýlýyor...");
        Application.Quit();
    }

    // Ses ikonu güncelleme
    void UpdateSoundIcon()
    {
        if (soundButtonImage != null)
        {
            Color iconColor = soundButtonImage.color;
            iconColor.a = isMuted ? 0.5f : 1f; // Kapalýysa yarý saydam, açýksa tam opak
            soundButtonImage.color = iconColor;
        }
    }
}