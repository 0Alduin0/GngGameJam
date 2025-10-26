using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioSettingsManager : MonoBehaviour
{
    public AudioSource ses;   // Hangi sesi kontrol edeceksin (Inspector�dan s�r�kle)
    public Slider sliderim;   // Volume slider (Inspector�dan s�r�kle)

    void Start()
    {
        // Ba�lang��ta slider de�eri mevcut ses seviyesine e�it olsun
        sliderim.value = ses.volume;
    }

    void Update()
    {
        // Slider de�eri de�i�tik�e ses seviyesini ayarla
        ses.volume = sliderim.value;
    }
}

