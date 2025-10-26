using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Men� Paneli")]
    public GameObject pauseMenuPanel;

    [Header("Ses Ayarlar�")]
    public Image soundButtonImage;
    public AudioSource[] audioSources; // D��ar�dan atanacak AudioSource'lar
    private bool isMuted = false;

    void Start()
    {
        // Kaydedilmi� ses durumunu y�kle
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        ApplyMuteToAudioSources();
        UpdateSoundIcon();
    }

    // Resume butonu i�in
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Pause butonu i�in
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Ses a�/kapa butonu i�in
    public void ToggleSound()
    {
        isMuted = !isMuted;
        ApplyMuteToAudioSources();
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        UpdateSoundIcon();
    }

    // AudioSource'lar�n mute durumunu g�ncelle
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

    // Ana men�ye d�n
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mehmet");
    }

    // Oyundan ��k
    public void Quit()
    {
        Debug.Log("Oyun kapat�l�yor...");
        Application.Quit();
    }

    // Ses ikonu g�ncelleme
    void UpdateSoundIcon()
    {
        if (soundButtonImage != null)
        {
            Color iconColor = soundButtonImage.color;
            iconColor.a = isMuted ? 0.5f : 1f; // Kapal�ysa yar� saydam, a��ksa tam opak
            soundButtonImage.color = iconColor;
        }
    }
}