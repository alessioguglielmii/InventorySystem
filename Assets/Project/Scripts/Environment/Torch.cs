using UnityEngine;
using UnityEngine.Audio;

public class Torch : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private Light m_pointLight;
    [SerializeField] private float m_lightIntensity;

    void Start()
    {
        m_audioSource.time = Random.Range(0.0f, m_audioSource.clip.length);
        m_audioSource.Play();

        m_pointLight.intensity = m_lightIntensity;
    }
}
