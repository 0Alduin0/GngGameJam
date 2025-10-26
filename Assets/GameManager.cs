using UnityEngine;
public class GameManager : MonoBehaviour
{
    public GameObject koylu;
    public GameObject koylu1;
    public GameObject koylu2;
    public GameObject koylu3;
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
        koylu2.GetComponent<NpcDiyalog>().enabled = false;
        koylu3.GetComponent<NpcDiyalog>().enabled = false;

        koylu1.GetComponent<SpriteRenderer>().enabled = false;
        koylu2.GetComponent<SpriteRenderer>().enabled = false;
        koylu3.GetComponent<SpriteRenderer>().enabled = false;
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
            case 1:
                koylu1.GetComponent<NpcDiyalog>().enabled = false;
                koylu1.GetComponent<SpriteRenderer>().enabled = false;
                koylu2.GetComponent<SpriteRenderer>().enabled = true;
                koylu2.GetComponent<NpcDiyalog>().enabled = true;
                break;
            case 2:
                koylu2.GetComponent<NpcDiyalog>().enabled = false;
                koylu2.GetComponent<SpriteRenderer>().enabled = false;
                koylu3.GetComponent<SpriteRenderer>().enabled = true;
                koylu3.GetComponent<NpcDiyalog>().enabled = true;
                break;
        }
    }
}