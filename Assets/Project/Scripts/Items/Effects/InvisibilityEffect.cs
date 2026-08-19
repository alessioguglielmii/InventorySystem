using UnityEngine;

[CreateAssetMenu(fileName = "InvisibilityEffect", menuName = "Inventory/Effects/Invisibility")]
public class InvisibilityEffect : ItemEffect
{
    public override bool Apply(GameObject goTarget)
    {
        if (goTarget == null)
        {
            return false;
        }

        return true;
    }
}
