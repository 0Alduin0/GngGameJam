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

    [Header("Baþlangýç Pozisyonu")]
    private Vector3 baslangicPozisyonu;

    private bool oyuncuAlaniIcinde = false;
    private bool saldiriyor = false;

    void Start()
    {
        // NavMeshAgent'i 2D için yapýlandýr
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

        // Baþlangýç pozisyonunu kaydet
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        if (oyuncu == null) return;

        float oyuncuyaMesafe = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(oyuncu.position.x, oyuncu.position.y)
        );

        // Oyuncu tetikleme alaný içinde mi kontrol et
        if (oyuncuyaMesafe <= tetiklemeAlaniYaricap)
        {
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
                    animator.SetBool("Kos", false);
                    animator.SetBool("Saldir", true);
                }
            }
            else
            {
                // Koþma modunda
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
            // Oyuncu alandan çýktý
            if (oyuncuAlaniIcinde)
            {
                oyuncuAlaniIcinde = false;
                saldiriyor = false;
                agent.isStopped = false;
                animator.SetBool("Kos", false);
                animator.SetBool("Saldir", false);

                // Baþlangýç pozisyonuna dön (isteðe baðlý)
                // agent.SetDestination(baslangicPozisyonu);
            }
        }

        // 2D için sprite'ý çevir
        if (agent.velocity.x != 0)
        {
            if (agent.velocity.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
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
    }
}