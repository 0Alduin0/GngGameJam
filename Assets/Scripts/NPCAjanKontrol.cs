using UnityEngine;
using UnityEngine.AI;

public class NPCAjanKontrol : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private Transform oyuncu;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;

    [Header("Mesafe Ayarlar�")]
    [SerializeField] private float tetiklemeAlaniYaricap = 10f;
    [SerializeField] private float saldiriMesafesi = 2f;

    [Header("Ba�lang�� Pozisyonu")]
    private Vector3 baslangicPozisyonu;

    private bool oyuncuAlaniIcinde = false;
    private bool saldiriyor = false;

    void Start()
    {
        // NavMeshAgent'i 2D i�in yap�land�r
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

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
        if (oyuncu == null) return;

        float oyuncuyaMesafe = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(oyuncu.position.x, oyuncu.position.y)
        );

        // Oyuncu tetikleme alan� i�inde mi kontrol et
        if (oyuncuyaMesafe <= tetiklemeAlaniYaricap)
        {
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
                    animator.SetBool("Kos", false);
                    animator.SetBool("Saldir", true);
                }
            }
            else
            {
                // Ko�ma modunda
                if (saldiriyor)
                {
                    saldiriyor = false;
                    agent.isStopped = false;
                    animator.SetBool("Saldir", false);
                }

                animator.SetBool("Kos", true);
                agent.SetDestination(oyuncu.position);
            }
        }
        else
        {
            // Oyuncu alandan ��kt�
            if (oyuncuAlaniIcinde)
            {
                oyuncuAlaniIcinde = false;
                saldiriyor = false;
                agent.isStopped = false;
                animator.SetBool("Kos", false);
                animator.SetBool("Saldir", false);

                // Ba�lang�� pozisyonuna d�n (iste�e ba�l�)
                // agent.SetDestination(baslangicPozisyonu);
            }
        }

        // 2D i�in sprite'� �evir
        if (agent.velocity.x != 0)
        {
            if (agent.velocity.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
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
    }
}