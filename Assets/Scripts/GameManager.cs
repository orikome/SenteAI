using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Agent> activeAgents = new List<Agent>();

    void Awake()
    {
        Instance = this;
    }
}
