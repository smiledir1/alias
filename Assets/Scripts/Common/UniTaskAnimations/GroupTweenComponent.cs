using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    public class GroupTweenComponent : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField]
        private bool _startOnAwake;

        [SerializeField]
        [SerializeReference]
        private List<SimpleTween> _tweens = new();

        public List<SimpleTween> Tweens => _tweens;

        public void AddTween(SimpleTween tween)
        {
            _tweens.Add(tween);
        }

        public async UniTask PlayAnimations()
        {
            var tasks = new List<UniTask>();
            foreach (var tween in _tweens)
            {
                tasks.Add(tween.StartAnimation());
            }

            await UniTask.WhenAll(tasks);
        }

        public void StopAnimations()
        {
            foreach (var tween in _tweens)
            {
                tween.StopAnimation();
            }
        }

        public void ResetAnimations()
        {
            foreach (var tween in _tweens)
            {
                tween.ResetValues();
            }
        }

        #region Unity Life Cycle

        protected void Start()
        {
            if (_startOnAwake)
            {
                foreach (var tween in _tweens)
                {
                    tween.StartAnimation().Forget();
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = _tweens.Count - 1; i >= 0; i--)
            {
                var tween = _tweens[i];
                if (tween == null)
                {
                    _tweens.RemoveAt(i);
                }
            }
        }
#endif

        #endregion /Unity Life Cycle
    }
}