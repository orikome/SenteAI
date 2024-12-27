using UnityEngine;

public class FramerateManager : MonoBehaviour
{
    public enum FramerateOptions
    {
        FPS_10 = 10,
        FPS_20 = 20,
        FPS_30 = 30,
        FPS_40 = 40,
        FPS_50 = 50,
        FPS_60 = 60,
        FPS_144 = 144,
        FPS_240 = 240,
        FPS_360 = 360,
        FPS_500 = 500,
    }

    public FramerateOptions targetFramerate = FramerateOptions.FPS_60;

    private void Start()
    {
        SetFramerate((int)targetFramerate);
    }

    public void SetFramerate(int framerate)
    {
        Application.targetFrameRate = framerate;
        AgentLogger.Log($"Framerate set to: {framerate} FPS");
    }
}
