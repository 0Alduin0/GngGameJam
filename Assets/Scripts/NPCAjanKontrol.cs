using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float hareketHizi = 3.5f; // YENÝ: Hareket hýzý
    [SerializeField] private float hareketEsikDegeri = 0.1f;

    private Vector3 baslangicPozisyonu;
    private bool oyuncuAlaniIcinde = false;
    private bool saldiriyor = false;
    private bool devriyeNoktasinaGidiyor = false;
    private bool ilkBaslangic = true;

    void Start()
    {
        // NavMeshAgent'i al ve kontrol et
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // NavMeshAgent'i 2D için yapýlandýr
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            // YENÝ: Speed'i zorla ayarla
            agent.speed = hareketHizi;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;

            // Agent'ý NavMesh'e yerleþtir
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
        else
        {
            Debug.LogError(gameObject.name + " üzerinde NavMeshAgent componenti bulunamadý!");
        }

        // Animator yoksa bul
        if (animator == null)
            animator = GetComponent<Animator>();

        // Oyuncu yoksa "Player" tag'i ile bul
        if (oyuncu == null)
        {
            GameObject oyuncuObj = GameObject.FindGameObjectWithTag("Player");
            if (oyuncuObj != null)
                oyuncu = oyuncuObj.transform;
        }

        // Baþlangýç pozisyonunu kaydet
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        // Temel kontroller
        if (agent == null)
        {
            Debug.LogWarning("Agent bulunamadý!");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent NavMesh üzerinde deðil!");
            return;
        }

        if (oyuncu == null)
        {
            Debug.LogWarning("Oyuncu bulunamadý!");
            return;
        }

        // DEBUG: Agent durumunu göster
        Debug.Log($"Agent Speed: {agent.speed}, Velocity: {agent.velocity.magnitude}, isStopped: {agent.isStopped}");

        // Ýlk baþlangýçta devriye noktasýna git
        if (ilkBaslangic && devriyeNoktasi != null)
        {
            ilkBaslangic = false;
            devriyeNoktasinaGidiyor = true;
            agent.isStopped = false;
            agent.speed = hareketHizi; // Speed'i tekrar ayarla
            agent.SetDestination(devriyeNoktasi.position);

            Debug.Log("Ýlk baþlangýç: Devriye noktasýna gidiyor! Hedef: " + devriyeNoktasi.position);
        }

        float oyuncuyaMesafe = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(oyuncu.position.x, oyuncu.position.y)
        );

        // Oyuncu tetikleme alaný içinde mi kontrol et
        if (oyuncuyaMesafe <= tetiklemeAlaniYaricap)
        {
            devriyeNoktasinaGidiyor = false;

            if (!oyuncuAlaniIcinde)
            {
                oyuncuAlaniIcinde = true;
            }

            // Saldýrý mesafesinde mi kontrol et
            if (oyuncuyaMesafe <= saldiriMesafesi)
            {
                // Saldýrý moduna geç
                if (!saldiriyor)
                {
                    saldiriyor = true;
                    agent.isStopped = true;

                    if (animator != null)
                    {
                        animator.SetBool("Kos", false);
                        animator.SetBool("Saldir", true);
                    }
                }
            }
            else
            {
                // Koþma modunda
                if (saldiriyor)
                {
                    saldiriyor = false;
                }

                agent.isStopped = false;
                agent.speed = hareketHizi;

                // SetDestination sadece agent aktifse çaðrýlýr
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(oyuncu.position);
                }
            }
        }
        else
        {
            // Oyuncu alandan çýktý
            if (oyuncuAlaniIcinde)
            {
                oyuncuAlaniIcinde = false;
                saldiriyor = false;

                // Devriye noktasýna git
                if (devriyeNoktasi != null && agent.enabled && agent.isOnNavMesh)
                {
                    devriyeNoktasinaGidiyor = true;
                    agent.isStopped = false;
                    agent.speed = hareketHizi;
                    agent.SetDestination(devriyeNoktasi.position);

                    Debug.Log("Devriye noktasýna gidiyor! Hedef: " + devriyeNoktasi.position);
                }
            }

            // Devriye noktasýna gidiyorsa ve ulaþtýysa
            if (devriyeNoktasinaGidiyor && devriyeNoktasi != null)
            {
                float devriyeMesafe = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.y),
                    new Vector2(devriyeNoktasi.position.x, devriyeNoktasi.position.y)
                );

                // Devriye noktasýna ulaþtý mý?
                if (devriyeMesafe <= devriyeNoktasiTolerans)
                {
                    devriyeNoktasinaGidiyor = false;
                    agent.isStopped = true;

                    Debug.Log("Devriye noktasýna ulaþtý!");
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = hareketHizi;
                }
            }
            else if (!devriyeNoktasinaGidiyor)
            {
                agent.isStopped = true;
            }
        }

        // HAREKET BAZLI ANÝMASYON (velocity ile senkronize)
        float currentSpeed = agent.velocity.magnitude;

        if (animator != null)
        {
            if (saldiriyor)
            {
                // Saldýrý animasyonu
                animator.SetBool("Kos", false);
                animator.SetBool("Saldir", true);
            }
            else if (currentSpeed > hareketEsikDegeri)
            {
                // Hareket ediyor - Koþ animasyonu
                animator.SetBool("Saldir", false);
                animator.SetBool("Kos", true);
            }
            else
            {
                // Durmuþ - Idle animasyonu
                animator.SetBool("Kos", false);
                animator.SetBool("Saldir", false);
            }
        }

        // 2D için sprite'ý çevir
        if (agent.velocity.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (agent.velocity.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // Gizmos ile alanlarý görselleþtir
    void OnDrawGizmosSelected()
    {
        // Tetikleme alaný
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tetiklemeAlaniYaricap);

        // Saldýrý mesafesi
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, saldiriMesafesi);

        // Devriye noktasý
        if (devriyeNoktasi != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(devriyeNoktasi.position, devriyeNoktasiTolerans);
            Gizmos.DrawLine(transform.position, devriyeNoktasi.position);
        }
    }
}
