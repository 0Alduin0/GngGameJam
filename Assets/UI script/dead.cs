using UnityEngine;
using UnityEngine.SceneManagement;

public class dead : MonoBehaviour
{
   public void Die()
    {
        // Oyuncu öldüðünde ana menü sahnesine yönlendir
        SceneManager.LoadScene(1); // Buraya kendi ana menü sahnenin adýný yaz
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
