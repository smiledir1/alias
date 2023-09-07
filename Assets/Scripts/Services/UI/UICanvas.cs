using UnityEngine;

namespace Services.UI
{
    public abstract class UICanvas : MonoBehaviour
    {
        [SerializeField]
        protected GameObject raycastBlock;

        public virtual void EnableRaycast()
        {
            raycastBlock.gameObject.SetActive(true);
        }

        public virtual void DisableRaycast()
        {
            raycastBlock.gameObject.SetActive(false);
        }
    }
}