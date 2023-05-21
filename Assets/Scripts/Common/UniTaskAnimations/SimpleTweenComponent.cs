using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    public class SimpleTweenComponent : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField]
        private bool _startOnAwake;

        [SerializeField]
        [SerializeReference]
        private SimpleTween _tween;

        public SimpleTween Tween => _tween;

        internal void SetTween(SimpleTween tween)
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