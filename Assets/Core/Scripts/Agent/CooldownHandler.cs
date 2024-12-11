using UnityEngine;

public class CooldownHandler
{
    private float _cooldownTime;
    private float _lastActionTime;

    public CooldownHandler(float cooldownTime)
    {
        _cooldownTime = cooldownTime;
        _lastActionTime = -cooldownTime;
    }

    public bool IsReady()
    {
        return Time.time >= _lastActionTime + _cooldownTime;
    }

    public void Reset()
    {
        _lastActionTime = Time.time;
    }
}
