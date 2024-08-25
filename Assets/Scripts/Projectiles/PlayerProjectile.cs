using UnityEngine;

public class PlayerProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
        _collisionMask = LayerMask.GetMask("Enemy");
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
            return;

        base.OnCollisionEnter(collision);
    }
}
