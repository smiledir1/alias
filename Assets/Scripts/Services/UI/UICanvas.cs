using UnityEngine;

namespace Services.UI
{
    public abstract class UICanvas : MonoBehaviour
    {
        [SerializeField]
        protected GameObject _raycastBlock;

        public virtual void EnableRaycast()
        {
            _raycastBlock.gameObject.SetActive(true);
        }

        public virtual void DisableRaycast()
        {
            _raycastBlock.gameObject.SetActive(false);
        }
    }
}