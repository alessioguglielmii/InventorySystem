using UnityEngine;

public class Torch : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light m_pointLight;
    [SerializeField] private float m_lightIntensity;
    [SerializeField] private float m_flickerSpeed = 4.0f;
    [SerializeField] private float m_flickerAmount = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioSource m_audioSource;

    private float _noiseOffset;
    private Vector3 _initialPosition;

    private void Start()
    {
        m_audioSource.time = Random.Range(0.0f, m_audioSource.clip.length);
        m_audioSource.Play();

        m_pointLight.intensity = m_lightIntensity;
        _noiseOffset = Random.Range(0.0f, 100.0f);
        _initialPosition = m_pointLight.transform.localPosition;
    }

    private void Update()
    {
        float time = Time.time * m_flickerSpeed;

        float noise = Mathf.PerlinNoise(_noiseOffset, Time.time * m_flickerSpeed);

        float flicker = Mathf.Lerp(1.0f - m_flickerAmount, 1.0f + m_flickerAmount, noise);

        m_pointLight.intensity = m_lightIntensity * flicker;

        float offsetX = (Mathf.PerlinNoise(_noiseOffset + 10.0f, time) - 0.5f) * 0.05f;
        float offsetY = (Mathf.PerlinNoise(_noiseOffset + 20.0f, time) - 0.5f) * 0.05f;

        m_pointLight.transform.localPosition = _initialPosition + new Vector3(offsetX, offsetY, 0.0f);
    }
}
