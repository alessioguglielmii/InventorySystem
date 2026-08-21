using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float m_explosionRadius = 3.0f;
    [SerializeField] private GameObject m_explosionEffect;
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_explosionClip;

    [HideInInspector] public Transform bombSocket;

    private bool _isThrown = false;

    private void Update()
    {
        if (!_isThrown)
        {
            transform.position = bombSocket.position;
            transform.rotation = bombSocket.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 0.75f);
    }

    private void OnDestroy()
    {
        if (m_explosionEffect != null)
        {
            GameObject explosionEffect = Instantiate(m_explosionEffect, transform.position, transform.rotation);
            Destroy(explosionEffect, 2.5f);
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
            Bomb otherBomb = collider.gameObject.GetComponent<Bomb>();

            if (chest != null)
            {
                chest.DestroyChest();
            }

            if (otherBomb != null)
            {
                Destroy(otherBomb);
            }
        }
    }

    public void BombThrowing(float bombForce, float bombUpwardForce)
    {
        _isThrown = true;

        Rigidbody rigidbody = GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            Vector3 throwDirection = transform.forward;
            throwDirection.y = bombUpwardForce;
            throwDirection.Normalize();

            rigidbody.AddForce(throwDirection * bombForce, ForceMode.Impulse);
        }
    }
}
