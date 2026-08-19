using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InvisibilityEffect", menuName = "Inventory/Effects/Invisibility")]
public class InvisibilityEffect : ItemEffect
{
    [SerializeField] private Material m_invisibilityMaterial;

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

        characterMovement.StartInvisibility(m_invisibilityMaterial);

        return true;
    }
}
