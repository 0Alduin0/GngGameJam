using UnityEngine;
public class Ana_menu : MonoBehaviour
{
    
        public GameObject mainMenuPanel;
        public GameObject settingsPanel;

        // Ana  men�y� a�ar
        public void OpenSettings()
        {
            mainMenuPanel.SetActive(false);
            settingsPanel.SetActive(true);
        }

        // Geri butonuna bas�nca ana men�ye d�ner
        public void BackToMainMenu()
        {
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        public void Quit()
        {
        Debug.Log("Oyun kapat�l�yor...");  // Edit�r'de test i�in log yazar
        Application.Quit();                // Ger�ek build'de oyunu kapat�r
        }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
