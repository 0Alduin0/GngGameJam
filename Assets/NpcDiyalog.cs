using UnityEngine;
using DialogueEditor;

public class NpcDiyalog : MonoBehaviour
{
    public NPCConversation conversation;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Tetikleme algılandı!");
            ConversationManager.Instance.StartConversation(conversation);
        }
    }
}