using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    public static ProjectilePooler Instance { get; private set; }

    [SerializeField]
    private GameObject _projectilePrefab;
    public static readonly int POOL_SIZE = 400;
    private Stack<Projectile> _projectilePool = new();
    private Transform _projectileParent;

    private void Awake()
    {
        Instance = this;

        _projectileParent = new GameObject("ProjectilePool").transform;
        _projectileParent.SetParent(this.transform);

        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject projectileObject = Instantiate(_projectilePrefab, _projectileParent);
            Projectile projectileComponent = projectileObject.GetComponent<Projectile>();

            if (projectileComponent == null)
            {
                Debug.LogError("Projectile prefab does not have the Projectile script attached!");
                return;
            }

            projectileComponent.gameObject.SetActive(false);
            _projectilePool.Push(projectileComponent);
        }
    }

    public Projectile SpawnProjectile(
        Vector3 position,
        Quaternion rotation,
        Vector3 direction,
        float speed,
        int damage
    )
    {
        if (_projectilePool.Count > 0)
        {
            Projectile projectile = _projectilePool.Pop();
            projectile.gameObject.SetActive(true);

            projectile.transform.position = position;
            projectile.transform.rotation = rotation;
            projectile.SetParameters(direction, speed, damage);

            projectile.OnMissCallback = () => ReturnProjectile(projectile);

            return projectile;
        }
        else
        {
            Debug.LogWarning("No projectiles available in pool!");
            return null;
        }
    }

    public void ReturnProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _projectilePool.Push(projectile);
    }
}
