using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    public class WarningIndicator : MonoBehaviour
    {
        [SerializeField]
        private float lifeTime = 2f;

        [SerializeField]
        private float fadeSpeed = 1f;
        private Material _material;
        private float _timer;
        private Agent _owner;
        private bool _IsInit;

        private void Awake()
        {
            _material = GetComponentInChildren<Renderer>().material;
            _timer = lifeTime;
        }

        private void Update()
        {
            if (!_IsInit)
                return;

            if (_owner == null)
            {
                Destroy(gameObject);
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer <= fadeSpeed)
            {
                Color color = _material.GetColor("_MainColor");
                color.a = _timer / fadeSpeed;
                _material.SetColor("_MainColor", color);
            }
            if (_timer <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(Agent agent)
        {
            _owner = agent;
            _IsInit = true;
        }
    }
}
