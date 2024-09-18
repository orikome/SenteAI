using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Agent
{
    public override void Initialize()
    {
        base.Initialize();
    }

    void OnEnable()
    {
        GameManager.Instance.activeEnemies.Add(this);
    }

    void OnDisable()
    {
        GameManager.Instance.activeEnemies.Remove(this);
    }
}
