using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject koylu;
    public GameObject koylu1;
    public int diyalogSeviyesi = 0;
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        koylu1.GetComponent<NpcDiyalog>().enabled = false;

        koylu1.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void koyluyoket(int kacýncýkoylu)
    {
        switch (kacýncýkoylu)
        {
            case 0:
                koylu.GetComponent<NpcDiyalog>().enabled = false;
                koylu.GetComponent<SpriteRenderer>().enabled = false;
                koylu1.GetComponent<SpriteRenderer>().enabled = true;
                koylu1.GetComponent<NpcDiyalog>().enabled = true;
                break;
        }
    }
}