using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCAjanKontrol : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Transform oyuncu;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Mesafe Ayarlarý")]
    [SerializeField] private float tetiklemeAlaniYaricap = 10f;
    [SerializeField] private float saldiriMesafesi = 2f;

    [Header("Devriye Noktasý")]
    [SerializeField] private Transform devriyeNoktasi;
    [SerializeField] private float devriyeNoktasiTolerans = 0.5f;

    [Header("Hareket Ayarlarý")]
    [SerializeField] private float hareketHizi = 3.5f;
    [SerializeField] private float hareketEsikDegeri = 0.1f;

    [Header("Saðlýk ve Hasar")]
    [SerializeField] private float maxCan = 100f;
    [SerializeField] private float baltaHasari = 20f;
    [SerializeField] private float stunSuresi = 3f;
    [SerializeField] private float saldiriHasari = 10f;
    [SerializeField] private float saldiriBeklemeSuresi = 1.5f;

    // Özel deðiþkenler
    private float mevcutCan;
    private Vector3 baslangicPozisyonu;
    private bool oyuncuAlaniIcinde = false;
    private bool saldiriyor = false;
    private bool devriyeNoktasinaGidiyor = false;
    private bool ilkBaslangic = true;
    private bool sersemlemis = false;
    private bool oldu = false;
    private bool saldiriYapabiliyor = true;
    private Health Health;

    // Cache'lenmiþ deðiþkenler
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private static readonly int Kos = Animator.StringToHash("Kos");
    private static readonly int Saldir = Animator.StringToHash("Saldir");
    private static readonly int Olmeanim = Animator.StringToHash("Ol");

    void Start()
    {
        InitializeComponents();
        CacheReferences();
        SetupNavMeshAgent();
    }

    void Update()
    {
        if (oldu || sersemlemis) return;

        if (!ValidateEssentialComponents()) return;

        UpdateAIState();
        UpdateAnimation();
        UpdateSpriteDirection();
    }

    private void InitializeComponents()
    {
        mevcutCan = maxCan;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (oyuncu == null)
        {
            GameObject oyuncuObj = GameObject.FindGameObjectWithTag("Player");
            if (oyuncuObj != null)
                oyuncu = oyuncuObj.transform;
        }

        baslangicPozisyonu = transform.position;
    }

    private void CacheReferences()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Oyuncu saðlýk scriptini cache'le
        if (oyuncu != null)
        {
            Health = oyuncu.GetComponent<Health>();
            if (Health == null)
            {
                Debug.LogWarning("Oyuncu üzerinde Health scripti bulunamadý!");
            }
        }
    }

    private void SetupNavMeshAgent()
    {
        if (agent == null) return;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = hareketHizi;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.enabled = true;
        }
        else
        {
            Debug.LogError(gameObject.name + " NavMesh üzerine yerleþtirilemedi!");
        }
    }

    private bool ValidateEssentialComponents()
    {
        if (agent == null || !agent.isOnNavMesh || oyuncu == null)
        {
            Debug.LogWarning("Temel componentler eksik!");
            return false;
        }
        return true;
    }

    private void UpdateAIState()
    {
        float oyuncuyaMesafe = GetDistanceToPlayer();

        // Ýlk baþlangýç kontrolü
        HandleInitialPatrol();

        // Oyuncu takip ve saldýrý durumu
        if (oyuncuyaMesafe <= tetiklemeAlaniYaricap)
        {
            HandlePlayerInRange(oyuncuyaMesafe);
        }
        else
        {
            HandlePlayerOutOfRange();
        }

        HandlePatrolBehavior();
    }

    private float GetDistanceToPlayer()
    {
        return Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(oyuncu.position.x, oyuncu.position.y)
        );
    }

    private void HandleInitialPatrol()
    {
        if (ilkBaslangic && devriyeNoktasi != null)
        {
            ilkBaslangic = false;
            devriyeNoktasinaGidiyor = true;
            agent.isStopped = false;
            agent.SetDestination(devriyeNoktasi.position);
        }
    }

    private void HandlePlayerInRange(float distanceToPlayer)
    {
        devriyeNoktasinaGidiyor = false;
        oyuncuAlaniIcinde = true;

        if (distanceToPlayer <= saldiriMesafesi)
        {
            EnterAttackMode();
        }
        else
        {
            EnterChaseMode();
        }
    }

    private void HandlePlayerOutOfRange()
    {
        if (oyuncuAlaniIcinde)
        {
            oyuncuAlaniIcinde = false;
            saldiriyor = false;

            if (devriyeNoktasi != null)
            {
                ReturnToPatrol();
            }
        }

        if (!devriyeNoktasinaGidiyor)
        {
            agent.isStopped = true;
        }
    }

    private void HandlePatrolBehavior()
    {
        if (devriyeNoktasinaGidiyor && devriyeNoktasi != null)
        {
            float devriyeMesafe = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(devriyeNoktasi.position.x, devriyeNoktasi.position.y)
            );

            if (devriyeMesafe <= devriyeNoktasiTolerans)
            {
                devriyeNoktasinaGidiyor = false;
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }

    private void EnterAttackMode()
    {
        if (!saldiriyor)
        {
            saldiriyor = true;
            agent.isStopped = true;
        }

        // Saldýrý yap
        if (saldiriYapabiliyor)
        {
            StartCoroutine(SaldiriYap());
        }
    }

    private void EnterChaseMode()
    {
        saldiriyor = false;
        agent.isStopped = false;
        agent.SetDestination(oyuncu.position);
    }

    private void ReturnToPatrol()
    {
        devriyeNoktasinaGidiyor = true;
        agent.isStopped = false;
        agent.SetDestination(devriyeNoktasi.position);
    }

    private System.Collections.IEnumerator SaldiriYap()
    {
        saldiriYapabiliyor = false;

        // Oyuncuya hasar ver
        if (Health != null)
        {
            Health.TakeDamage();
            Debug.Log($"Oyuncuya {saldiriHasari} hasar verildi!");
        }

        // Bekleme süresi
        yield return new WaitForSeconds(saldiriBeklemeSuresi);
        saldiriYapabiliyor = true;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        float currentSpeed = agent.velocity.magnitude;

        if (saldiriyor)
        {
            animator.SetBool(Kos, false);
            animator.SetBool(Saldir, true);
        }
        else if (currentSpeed > hareketEsikDegeri)
        {
            animator.SetBool(Saldir, false);
            animator.SetBool(Kos, true);
        }
        else
        {
            animator.SetBool(Kos, false);
            animator.SetBool(Saldir, false);
        }
    }

    private void UpdateSpriteDirection()
    {
        if (agent.velocity.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (agent.velocity.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleWeaponCollision(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleWeaponCollision(other.gameObject);
    }

    private void HandleWeaponCollision(GameObject collisionObject)
    {
        if (collisionObject.CompareTag("Balta") || collisionObject.name.Contains("Balta"))
        {
            TakeDamage(baltaHasari);
        }
    }

    public void TakeDamage(float hasar)
    {
        if (oldu) return;

        mevcutCan -= hasar;
        Debug.Log($"{gameObject.name} hasar aldý! Kalan can: {mevcutCan}");

        if (mevcutCan <= 0)
        {
            mevcutCan = 0;
            Ol();
        }
        else
        {
            StartCoroutine(SersemletmeEfekti());
        }
    }

    private IEnumerator SersemletmeEfekti()
    {
        sersemlemis = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (animator != null)
        {
            animator.SetBool(Kos, false);
            animator.SetBool(Saldir, false);
        }

        Color originalColor = Color.white;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.gray;
        }

        yield return new WaitForSeconds(stunSuresi);

        sersemlemis = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    private void Ol()
    {
        oldu = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool(Kos, false);
            animator.SetBool(Saldir, false);
            animator.SetTrigger(Olmeanim);
        }

        if (col != null)
        {
            col.enabled = false;
        }

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tetiklemeAlaniYaricap);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, saldiriMesafesi);

        if (devriyeNoktasi != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(devriyeNoktasi.position, devriyeNoktasiTolerans);
            Gizmos.DrawLine(transform.position, devriyeNoktasi.position);
        }
    }
}