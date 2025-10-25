using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private bool facingRight = true;

    public Animator animator;

    public float nextAttackTime = 0.7f;

    public int axeCount = 5;
    public GameObject axePrefab;
    public GameObject axeSpawnPoint;
    public float nextThrowAxe = 2f;



    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement();

        Attack();

        ThrowAxe();

    }
    private void Movement()
    {
        // Hareket girdisini al
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        // Karakterin y�n�n� �evir
        if (facingRight == false && moveInputX > 0)
            Flip();
        else if (facingRight == true && moveInputX < 0)
            Flip();
        if (moveInputX != 0 || moveInputY != 0)
            animator.SetFloat("Run", 1);
        else
            animator.SetFloat("Run", 0);
        rb.linearVelocity = new Vector3(moveInputX * moveSpeed, moveInputY * moveSpeed, 0);
    }

    private void Attack()
    {
        nextAttackTime -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && nextAttackTime <= 0)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = 0.7f;
        }
    }

    private void ThrowAxe()
    {
        nextThrowAxe -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1) && axeCount > 0)
        {
            GameObject throwedAxe = Instantiate(axePrefab, axeSpawnPoint.transform.position, Quaternion.identity);
            Rigidbody2D axeRb = throwedAxe.GetComponent<Rigidbody2D>();
            axeRb.AddForce(new Vector2(facingRight ? 1 : -1, 0) * 500f);
            axeCount--;
            nextThrowAxe = 2f;
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
