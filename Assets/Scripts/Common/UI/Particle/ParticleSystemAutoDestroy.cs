using UnityEngine;

namespace Common.UI.Particle
{
    public class ParticleSystemAutoDestroy : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem curParticleSystem;

        public void FixedUpdate()
        {
            if (curParticleSystem && !curParticleSystem.IsAlive()) Destroy(gameObject);
        }

        private void OnValidate()
        {
            if (curParticleSystem == null) curParticleSystem = GetComponent<ParticleSystem>();
        }
    }
}