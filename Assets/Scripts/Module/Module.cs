using UnityEngine;

public abstract class Module : ScriptableObject
{
    /// <summary>
    /// Called every frame in the object's Update method.
    /// </summary>
    public abstract void Execute(EnemyAgent agent);

    /// <summary>
    /// Called once in the object's Awake method.
    /// </summary>
    public abstract void Initialize(EnemyAgent agent);
}
