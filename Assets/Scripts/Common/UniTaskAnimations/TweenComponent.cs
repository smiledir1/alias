using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    public class TweenComponent : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField]
        private bool _startOnAwake;

        [SerializeField]
        [SerializeReference]
        private IBaseTween _tween;

        public IBaseTween Tween => _tween;

        internal void SetTween(IBaseTween tween)
        {
            _tween = tween;
        }

        #region Unity Life Cycle

        protected void Start()
        {
            if (_startOnAwake) _tween.StartAnimation().Forget();
        }

        #endregion /Unity Life Cycle
    }
}