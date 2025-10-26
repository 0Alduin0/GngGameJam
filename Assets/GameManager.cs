using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }


    public bool ilkBossOldumu = false;
    public GameObject koyluSpawn1;

    private void KoyluBilgi1()
    {
        if (ilkBossOldumu)
        {
            Debug.Log("Ýlk boss öldü!");
            koyluSpawn1.SetActive(true);
        }
    }
}