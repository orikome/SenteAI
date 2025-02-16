using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    public abstract class SenseModule : Module
    {
        public Vector3 LastKnownPosition { get; protected set; }
        public Vector3 LastKnownVelocity { get; protected set; }
        public float LastSeenTime { get; protected set; }
        public bool CanSenseTarget { get; protected set; }
    }
}
