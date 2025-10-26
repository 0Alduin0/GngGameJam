using UnityEngine;
using TMPro;

public class NPCManager : MonoBehaviour
{
    public GameObject diyalogPanel;
    public TextMeshProUGUI diyalogText;
    public string[] diyalogLines;

    private int currentLine = 0;
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (diyalogPanel.activeInHierarchy)
            {
                SonrakiCumle();
            }
            else
            {
                DiyaloguBaslat();
            }
        }
    }

    void DiyaloguBaslat()
    {
        diyalogPanel.SetActive(true);
        currentLine = 0;
        diyalogText.text = diyalogLines[currentLine];
    }

    void SonrakiCumle()
    {
        currentLine++;

        if (currentLine < diyalogLines.Length)
        {
            diyalogText.text = diyalogLines[currentLine];
        }
        else
        {
            // Diyalog bitti, sonraki NPC'ye geç
            diyalogPanel.SetActive(false);
            GameManager.instance.SonrakiNPCyeGec();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            diyalogPanel.SetActive(false);
        }
    }
}