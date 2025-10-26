using UnityEngine;
public class Ana_menu : MonoBehaviour
{
    
        public GameObject mainMenuPanel;
        public GameObject settingsPanel;

        // Ana  menüyü açar
        public void OpenSettings()
        {
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        // Geri butonuna basýnca ana menüye döner
        public void BackToMainMenu()
        {
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        public void Quit()
        {
        Debug.Log("Oyun kapatýlýyor...");  // Editör'de test için log yazar
        Application.Quit();                // Gerçek build'de oyunu kapatýr
        }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
