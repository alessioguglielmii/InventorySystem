using UnityEngine;

[CreateAssetMenu(fileName = "UnlockEffect", menuName = "Inventory/Effects/Unlock")]
public class UnlockEffect : ItemEffect
{
    public override bool Apply(GameObject goTarget)
    {
        if (goTarget == null)
        {
            return false;
        }

        Unlockable compUnlockable = goTarget.GetComponent<Unlockable>();

        if (compUnlockable == null)
        {
            return false;
        }

        return compUnlockable.Unlock();
    }
}
