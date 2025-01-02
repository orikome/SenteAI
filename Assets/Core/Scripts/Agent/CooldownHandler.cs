using UnityEngine;

public class CooldownHandler
{
    private readonly float _cooldownTime;
    private float _lastActionTime;
    private float _pausedUntilTime;

    public CooldownHandler(float cooldownTime)
    {
        _cooldownTime = cooldownTime;
        _lastActionTime = -cooldownTime;
        _pausedUntilTime = 0f;
    }

    public bool IsReady()
    {
        if (Time.time < _pausedUntilTime)
            return false;
        return Time.time >= _lastActionTime + _cooldownTime;
    }

    public void Reset()
    {
        _lastActionTime = Time.time;
    }

    public void PauseFor(float duration)
    {
        _pausedUntilTime = Time.time + duration;
    }
}
