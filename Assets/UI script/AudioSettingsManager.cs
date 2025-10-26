using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioSettingsManager : MonoBehaviour
{
    public AudioSource ses;   // Hangi sesi kontrol edeceksin (Inspector’dan sürükle)
    public Slider sliderim;   // Volume slider (Inspector’dan sürükle)

    void Start()
    {
        // Baþlangýçta slider deðeri mevcut ses seviyesine eþit olsun
        sliderim.value = ses.volume;
    }

    void Update()
    {
        // Slider deðeri deðiþtikçe ses seviyesini ayarla
        ses.volume = sliderim.value;
    }
}

