using UnityEngine;
using UnityEngine.SceneManagement;
public class Ana_menu : MonoBehaviour
{

    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }
    public void Quit()
    {
        Debug.Log("Oyun kapatılıyor...");
        Application.Quit();
    }

    void Start()
    {

    }


    void Update()
    {

    }
}
