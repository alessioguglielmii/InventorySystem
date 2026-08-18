using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionEffect", menuName = "Inventory/Effects/Explosion")]
public class ExplosionEffect : ItemEffect
{
    [SerializeField] private float m_fRadius = 3.0f;
    [SerializeField] private float m_fForce = 500.0f;

    public override bool Apply(GameObject goTarget)
    {
        if (goTarget == null)
        {
            return false;
        }

        Vector3 v3ExplosionPosition = goTarget.transform.position;

        Collider[] arrColliders = Physics.OverlapSphere(v3ExplosionPosition, m_fRadius);

        foreach (Collider compCollider in arrColliders)
        {
            Rigidbody compRigidbody = compCollider.attachedRigidbody;

            if (compRigidbody == null)
            {
                continue;
            }

            Vector3 v3Direction = compRigidbody.position - v3ExplosionPosition;

            compRigidbody.AddExplosionForce(m_fForce, v3ExplosionPosition, m_fRadius);
        }

        return true;
    }
}
