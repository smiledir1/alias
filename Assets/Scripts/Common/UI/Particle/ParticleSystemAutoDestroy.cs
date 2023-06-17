using System;
using UnityEngine;

namespace Common.UI.Particle
{
    public class ParticleSystemAutoDestroy : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _particleSystem;

        public void FixedUpdate()
        {
            if (_particleSystem && !_particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }

        private void OnValidate()
        {
            if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>();
        }
    }
}