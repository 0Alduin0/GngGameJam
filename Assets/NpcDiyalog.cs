using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NpcDiyalog : MonoBehaviour
{
    public GameObject diyalogPanel;
    public TextMeshProUGUI diyalogText;
    public string[] diyalogLines;
    public string[] diyalogSeti1;
    private int index = 0;
    public float wordSpeed;
    private bool playerInRange;
    public GameObject continueButton;
    public GameManager GameManager;
    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Start()
    {
        diyalogLines = diyalogSeti1;
    }
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            switch (GameManager.instance.diyalogSeviyesi)
            {
                case 0:
                    diyalogLines = diyalogSeti1;
                    break;
            }
            if (diyalogPanel.activeInHierarchy)
            {
                ZeroText();
            }
            else
            {
                diyalogPanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }
        if (diyalogText.text == diyalogLines[index])
        {
            continueButton.SetActive(true);
        }
    }
    public void ZeroText()
    {
        diyalogText.text = "";
        index = 0;
        diyalogPanel.SetActive(false);
    }
    IEnumerator Typing()
    {
        foreach (char letter in diyalogLines[index].ToCharArray())
        {
            diyalogText.text += letter; yield return new WaitForSeconds(wordSpeed);
        }
    }
    public void NextLine()
    {
        continueButton.SetActive(false);
        if (index < diyalogLines.Length - 1)
        {
            index++;
            diyalogText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            ZeroText();
            GameManager.koyluyoket(GameManager.instance.diyalogSeviyesi);
            GameManager.instance.diyalogSeviyesi++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("içine girdi");
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            ZeroText();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("dışına çıktı");
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            ZeroText();
        }
    }
}