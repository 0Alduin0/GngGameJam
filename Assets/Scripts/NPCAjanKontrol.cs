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
    [SerializeField] private float mevcutCan;
    [SerializeField] private float baltaHasari = 20f;
    [SerializeField] private float stunSuresi = 3f;

    private Vector3 baslangicPozisyonu;
    private bool oyuncuAlaniIcinde = false;
    private bool saldiriyor = false;
    private bool devriyeNoktasinaGidiyor = false;
    private bool ilkBaslangic = true;
    private bool sersemlemis = false; // YENÝ: Stun durumu
    private bool oldu = false; // YENÝ: Ölü durumu

    void Start()
    {
        // Can baþlangýç deðeri
        mevcutCan = maxCan;

        // NavMeshAgent'i al ve kontrol et
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // NavMeshAgent'i 2D için yapýlandýr
            agent.updateRotation = false;
            agent.updateUpAxis = false;

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
        // Ölü veya sersemlemiþse hareketi engelle
        if (oldu || sersemlemis)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            if (animator != null && sersemlemis)
            {
                animator.SetBool("Kos", false);
                animator.SetBool("Saldir", false);
            }

            return;
        }

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
            agent.speed = hareketHizi;
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

        // HAREKET BAZLI ANÝMASYON
        float currentSpeed = agent.velocity.magnitude;

        if (animator != null)
        {
            if (saldiriyor)
            {
                animator.SetBool("Kos", false);
                animator.SetBool("Saldir", true);
            }
            else if (currentSpeed > hareketEsikDegeri)
            {
                animator.SetBool("Saldir", false);
                animator.SetBool("Kos", true);
            }
            else
            {
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

    // YENÝ: Balta çarpma kontrolü
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Balta objesi ile çarpýþma kontrolü
        if (collision.gameObject.CompareTag("Balta") || collision.gameObject.name.Contains("Balta"))
        {
            HasarAl(baltaHasari);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Alternatif: Trigger bazlý çarpýþma
        if (other.CompareTag("Balta") || other.name.Contains("Balta"))
        {
            HasarAl(baltaHasari);
        }
    }

    // YENÝ: Hasar alma methodu
    public void HasarAl(float hasar)
    {
        if (oldu) return; // Zaten ölüyse hasar alma

        mevcutCan -= hasar;
        Debug.Log($"{gameObject.name} hasar aldý! Kalan can: {mevcutCan}");

        // Can sýfýrlanýrsa öl
        if (mevcutCan <= 0)
        {
            mevcutCan = 0;
            Ol();
        }
        else
        {
            // Hala canlýysa sersemlet (stun)
            StartCoroutine(SersemletmeEfekti());
        }
    }

    // YENÝ: Sersemletme efekti (3 saniye hareket edememe)
    private IEnumerator SersemletmeEfekti()
    {
        sersemlemis = true;
        Debug.Log($"{gameObject.name} sersemletildi! {stunSuresi} saniye hareket edemeyecek.");

        // Agent'ý durdur
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        // Opsiyonel: Sersemletme animasyonu
        if (animator != null)
        {
            animator.SetBool("Kos", false);
            animator.SetBool("Saldir", false);
            // animator.SetTrigger("Sersem"); // Eðer sersemletme animasyonunuz varsa
        }

        // Opsiyonel: Görsel efekt (renk deðiþimi)
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (sprite != null)
        {
            originalColor = sprite.color;
            sprite.color = Color.gray; // Gri renk vererek sersemletmeyi göster
        }

        // Belirlenen süre kadar bekle
        yield return new WaitForSeconds(stunSuresi);

        // Sersemletmeyi kaldýr
        sersemlemis = false;
        Debug.Log($"{gameObject.name} sersemletmeden kurtuldu!");

        // Rengi eski haline getir
        if (sprite != null)
        {
            sprite.color = originalColor;
        }

        // Agent'ý yeniden aktif et
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    // YENÝ: Ölüm methodu
    private void Ol()
    {
        oldu = true;
        Debug.Log($"{gameObject.name} öldü!");

        // Agent'ý durdur
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Ölüm animasyonu
        if (animator != null)
        {
            animator.SetBool("Kos", false);
            animator.SetBool("Saldir", false);
            animator.SetTrigger("Ol"); // Eðer ölüm animasyonunuz varsa
        }

        // Collider'ý kapat
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 2 saniye sonra objeyi yok et (veya devre dýþý býrak)
        Destroy(gameObject, 2f);
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