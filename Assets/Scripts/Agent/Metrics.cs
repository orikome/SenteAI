using UnityEngine;

public class Metrics : MonoBehaviour
{
    public Vector3 Velocity { get; protected set; }
    public Vector3 LastPosition { get; protected set; } = Vector3.zero;
    public float DamageTaken { get; private set; }
    public float DamageDone { get; private set; }

    // Behavior parameters
    public Behavior CurrentBehavior { get; protected set; }
    public float AggressiveThreshold { get; protected set; } = 0.7f;
    public float DefensiveThreshold { get; protected set; } = 0.3f;

    public virtual void UpdateVelocity()
    {
        Velocity = (transform.position - LastPosition) / Time.deltaTime;
        LastPosition = transform.position;
    }

    public void UpdateDamageDone(float dmgDone)
    {
        DamageDone += dmgDone;
    }

    public void UpdateDamageTaken(float dmgTaken)
    {
        DamageTaken += dmgTaken;
    }

    protected virtual Behavior ClassifyBehavior()
    {
        return Behavior.Balanced;
    }
}
