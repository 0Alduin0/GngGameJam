using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss1 : MonoBehaviour
{
    public GameObject bossPromptUI;  // Canvas içindeki panel
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            bossPromptUI.SetActive(true);
            Time.timeScale = 0f; // Oyunu dondur
        }
    }

    public void OnYesPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("BossArena1");
    }

    public void OnNoPressed()
    {
        bossPromptUI.SetActive(false);
        Time.timeScale = 1f;
        playerInRange = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bossPromptUI.SetActive(false);
            playerInRange = false;
        }
    }
}
