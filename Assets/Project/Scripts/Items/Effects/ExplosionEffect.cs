using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionEffect", menuName = "Inventory/Effects/Explosion")]
public class ExplosionEffect : ItemEffect
{
    public override bool Apply(GameObject goTarget)
    {
        if (goTarget == null)
        {
            return false;
        }

        CharacterMovement characterMovement = goTarget.GetComponent<CharacterMovement>();

        if (characterMovement == null)
        {
            return false;
        }

        characterMovement.ThrowBomb();

        return true;
    }
}
