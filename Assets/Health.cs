using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int health = 100;

    public static Health instance;
    public Image healthBar;

    public void TakeDamage()
    {
        health -= 20;
        healthBar.fillAmount = health / 100f;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
