using UnityEngine;

public class sahneyonet : MonoBehaviour
{

    public GameObject panerl;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panerl.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
