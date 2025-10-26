using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCAjanKontrol : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Transform oyuncu;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Mesafe Ayarlar�")]
    [SerializeField] private float tetiklemeAlaniYaricap = 10f;
    [SerializeField] private float saldiriMesafesi = 2f;

    [Header("Devriye Noktas�")]
    [SerializeField] private Transform devriyeNoktasi;
    [SerializeField] private float devriyeNoktasiTolerans = 0.5f;

    [Header("Hareket Ayarlar�")]
    [SerializeField] private float hareketHizi = 3.5f;
    [SerializeField] private float hareketEsikDegeri = 0.1f;

    [Header("Sa�l�k ve Hasar")]
    [SerializeField] private float maxCan = 100f;
    [SerializeField] private float mevcutCan;
    [SerializeField] private float baltaHasari = 20f;
    [SerializeField] private float stunSuresi = 3f;

    private Vector3 baslangicPozisyonu;
    private bool oyuncuAlaniIcinde = false;
    private bool saldiriyor = false;
    private bool devriyeNoktasinaGidiyor = false;
    private bool ilkBaslangic = true;
    private bool sersemlemis = false; // YEN�: Stun durumu
    private bool oldu = false; // YEN�: �l� durumu

    void Start()
    {
        // Can ba�lang�� de�eri
        mevcutCan = maxCan;

        // NavMeshAgent'i al ve kontrol et
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // NavMeshAgent'i 2D i�in yap�land�r
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            agent.speed = hareketHizi;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;

            // Agent'� NavMesh'e yerle�tir
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.enabled = true;
            }
            else
            {
                Debug.LogError(gameObject.name + " NavMesh �zerine yerle�tirilemedi!");
            }
        }
        else
        {
            Debug.LogError(gameObject.name + " �zerinde NavMeshAgent componenti bulunamad�!");
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

        // Ba�lang�� pozisyonunu kaydet
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        // �l� veya sersemlemi�se hareketi engelle
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
            Debug.LogWarning("Agent bulunamad�!");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent NavMesh �zerinde de�il!");
            return;
        }

        if (oyuncu == null)
        {
            Debug.LogWarning("Oyuncu bulunamad�!");
            return;
        }

        // DEBUG: Agent durumunu g�ster
        Debug.Log($"Agent Speed: {agent.speed}, Velocity: {agent.velocity.magnitude}, isStopped: {agent.isStopped}");

        // �lk ba�lang��ta devriye noktas�na git
        if (ilkBaslangic && devriyeNoktasi != null)
        {
            ilkBaslangic = false;
            devriyeNoktasinaGidiyor = true;
            agent.isStopped = false;
            agent.speed = hareketHizi;
            agent.SetDestination(devriyeNoktasi.position);

            Debug.Log("�lk ba�lang��: Devriye noktas�na gidiyor! Hedef: " + devriyeNoktasi.position);
        }

        float oyuncuyaMesafe = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(oyuncu.position.x, oyuncu.position.y)
        );

        // Oyuncu tetikleme alan� i�inde mi kontrol et
        if (oyuncuyaMesafe <= tetiklemeAlaniYaricap)
        {
            devriyeNoktasinaGidiyor = false;

            if (!oyuncuAlaniIcinde)
            {
                oyuncuAlaniIcinde = true;
            }

            // Sald�r� mesafesinde mi kontrol et
            if (oyuncuyaMesafe <= saldiriMesafesi)
            {
                // Sald�r� moduna ge�
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
                // Ko�ma modunda
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
            // Oyuncu alandan ��kt�
            if (oyuncuAlaniIcinde)
            {
                oyuncuAlaniIcinde = false;
                saldiriyor = false;

                // Devriye noktas�na git
                if (devriyeNoktasi != null && agent.enabled && agent.isOnNavMesh)
                {
                    devriyeNoktasinaGidiyor = true;
                    agent.isStopped = false;
                    agent.speed = hareketHizi;
                    agent.SetDestination(devriyeNoktasi.position);

                    Debug.Log("Devriye noktas�na gidiyor! Hedef: " + devriyeNoktasi.position);
                }
            }

            // Devriye noktas�na gidiyorsa ve ula�t�ysa
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

                    Debug.Log("Devriye noktas�na ula�t�!");
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

        // HAREKET BAZLI AN�MASYON
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

        // 2D i�in sprite'� �evir
        if (agent.velocity.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (agent.velocity.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // YEN�: Balta �arpma kontrol�
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Balta objesi ile �arp��ma kontrol�
        if (collision.gameObject.CompareTag("Balta") || collision.gameObject.name.Contains("Balta"))
        {
            HasarAl(baltaHasari);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Alternatif: Trigger bazl� �arp��ma
        if (other.CompareTag("Balta") || other.name.Contains("Balta"))
        {
            HasarAl(baltaHasari);
        }
    }

    // YEN�: Hasar alma methodu
    public void HasarAl(float hasar)
    {
        if (oldu) return; // Zaten �l�yse hasar alma

        mevcutCan -= hasar;
        Debug.Log($"{gameObject.name} hasar ald�! Kalan can: {mevcutCan}");

        // Can s�f�rlan�rsa �l
        if (mevcutCan <= 0)
        {
            mevcutCan = 0;
            Ol();
        }
        else
        {
            // Hala canl�ysa sersemlet (stun)
            StartCoroutine(SersemletmeEfekti());
        }
    }

    // YEN�: Sersemletme efekti (3 saniye hareket edememe)
    private IEnumerator SersemletmeEfekti()
    {
        sersemlemis = true;
        Debug.Log($"{gameObject.name} sersemletildi! {stunSuresi} saniye hareket edemeyecek.");

        // Agent'� durdur
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
            // animator.SetTrigger("Sersem"); // E�er sersemletme animasyonunuz varsa
        }

        // Opsiyonel: G�rsel efekt (renk de�i�imi)
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = Color.white;
        if (sprite != null)
        {
            originalColor = sprite.color;
            sprite.color = Color.gray; // Gri renk vererek sersemletmeyi g�ster
        }

        // Belirlenen s�re kadar bekle
        yield return new WaitForSeconds(stunSuresi);

        // Sersemletmeyi kald�r
        sersemlemis = false;
        Debug.Log($"{gameObject.name} sersemletmeden kurtuldu!");

        // Rengi eski haline getir
        if (sprite != null)
        {
            sprite.color = originalColor;
        }

        // Agent'� yeniden aktif et
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    // YEN�: �l�m methodu
    private void Ol()
    {
        oldu = true;
        Debug.Log($"{gameObject.name} �ld�!");

        // Agent'� durdur
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // �l�m animasyonu
        if (animator != null)
        {
            animator.SetBool("Kos", false);
            animator.SetBool("Saldir", false);
            animator.SetTrigger("Ol"); // E�er �l�m animasyonunuz varsa
        }

        // Collider'� kapat
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 2 saniye sonra objeyi yok et (veya devre d��� b�rak)
        Destroy(gameObject, 2f);
    }

    // Gizmos ile alanlar� g�rselle�tir
    void OnDrawGizmosSelected()
    {
        // Tetikleme alan�
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tetiklemeAlaniYaricap);

        // Sald�r� mesafesi
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, saldiriMesafesi);

        // Devriye noktas�
        if (devriyeNoktasi != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(devriyeNoktasi.position, devriyeNoktasiTolerans);
            Gizmos.DrawLine(transform.position, devriyeNoktasi.position);
        }
    }
}