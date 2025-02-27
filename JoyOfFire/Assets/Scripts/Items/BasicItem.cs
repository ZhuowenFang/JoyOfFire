using UnityEngine;

public class BasicItem : BaseItem
{
    private void Awake()
    {
        effectType = ItemEffectType.Passive;
    }

    public override void OnAcquire()
    {
        base.OnAcquire();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}