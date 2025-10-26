using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public bool ilkBossOldumu = false;
    public bool ikinciBossOldumu = false;
    public bool ucuncuBossOldumu = false;
    public GameObject koyluSpawn1;
    public GameObject koyluSpawn2;
    public GameObject koyluSpawn3;

    public void KoyluBilgi1()
    {
        if (ilkBossOldumu)
        {
            Debug.Log("Ýlk boss öldü!");
            koyluSpawn1.SetActive(true);
        }
    }
    public void KoyluBilgi2()
    {
        if (ikinciBossOldumu)
        {
            Debug.Log("Ýkinci boss öldü!");
            koyluSpawn2.SetActive(true);
        }
    }
    public void KoyluBilgi3()
    {
        if (ikinciBossOldumu)
        {
            Debug.Log("Ýkinci boss öldü!");
            koyluSpawn3.SetActive(true);
        }
    }
}