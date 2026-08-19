using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float m_explosionRadius = 3.0f;
    [SerializeField] private GameObject m_explosionEffect;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_explosionClip;

    [HideInInspector] public bool isThrown = false;
    [HideInInspector] public Transform bombSocket;

    private void Update()
    {
        if (!isThrown)
        {
            transform.position = bombSocket.position;
            transform.rotation = bombSocket.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_explosionEffect != null)
        {
            Instantiate(m_explosionEffect, transform.position, transform.rotation);
        }

        if (m_audioSource != null && m_explosionClip != null)
        {
            GameObject audioObject = new GameObject("BombExplosionAudio");

            audioObject.transform.position = transform.position;

            AudioSource audioSource = audioObject.AddComponent<AudioSource>();

            audioSource.clip = m_explosionClip;
            audioSource.outputAudioMixerGroup = m_audioSource.outputAudioMixerGroup;
            audioSource.spatialBlend = m_audioSource.spatialBlend;
            audioSource.minDistance = m_audioSource.minDistance;
            audioSource.maxDistance = m_audioSource.maxDistance;
            audioSource.volume = m_audioSource.volume;
            audioSource.reverbZoneMix = m_audioSource.reverbZoneMix;
            audioSource.rolloffMode = m_audioSource.rolloffMode;

            audioSource.Play();

            Destroy(audioObject, m_explosionClip.length);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_explosionRadius);

        foreach (Collider collider in colliders)
        {
            Chest chest = collider.gameObject.GetComponent<Chest>();

            if (chest != null)
            {
                chest.DestroyChest();
            }
        }

        Destroy(gameObject);
    }
}
