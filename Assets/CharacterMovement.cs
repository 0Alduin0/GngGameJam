using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private bool facingRight = true;

    public Animator animator;

    // Combo değişkenleri
    private int comboStep = 0; // 0 = idle, 1 = attack1, 2 = attack2
    private float comboResetTime = 1f; // iki saldırı arasında izin verilen max süre
    private float comboTimer = 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement();
        Attack();

        // Combo sıfırlama
        if (comboStep > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboStep = 0;
            }
        }
    }

    private void Movement()
    {
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        if (!facingRight && moveInputX > 0)
            Flip();
        else if (facingRight && moveInputX < 0)
            Flip();

        animator.SetFloat("Run", (moveInputX != 0 || moveInputY != 0) ? 1 : 0);

        rb.linearVelocity = new Vector3(moveInputX * moveSpeed, moveInputY * moveSpeed, 0);
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (comboStep == 0)
            {
                animator.SetTrigger("Attack");
                comboStep = 1;
                comboTimer = comboResetTime;
            }
            else if (comboStep == 1)
            {
                animator.SetTrigger("Attack2");
                comboStep = 0; // combo bitti
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
