using UnityEngine;
using UnityEngine.SceneManagement;

public class dead : MonoBehaviour
{
   public void Die()
    {
        // Oyuncu �ld���nde ana men� sahnesine y�nlendir
        SceneManager.LoadScene("Harita"); // Buraya kendi ana men� sahnenin ad�n� yaz
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
