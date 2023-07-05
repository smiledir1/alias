using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public class MultiTween : IBaseTween
    {
        [SerializeField]
        protected Transform _parentObject;

        [SerializeField]
        protected float _perObjectSecondsDelay;

        [SerializeField]
        [SerializeReference]
        protected ITween _tween;
        
        public Transform ParentObject => _parentObject;
        public float PerObjectSecondsDelay =>_perObjectSecondsDelay;
        public ITween Tween => _tween;

        private List<ITween> _animations = new();

        public MultiTween(Transform parentObject, float perObjectSecondsDelay)
        {
            _parentObject = parentObject;
            _perObjectSecondsDelay = perObjectSecondsDelay;
        }
        
        public static MultiTween Clone(MultiTween tween, GameObject targetObject = null)
        {
            var newTween = new MultiTween(targetObject.transform, tween.PerObjectSecondsDelay)
            {
                _tween = ITween.Clone(tween.Tween, targetObject)
            };

            return newTween;
        }

        public async UniTask StartAnimation(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            CheckInitialize();
            foreach (var animation in _animations)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_perObjectSecondsDelay),
                    cancellationToken: cancellationToken);
                animation.StartAnimation(reverse, startFromCurrentValue, cancellationToken).Forget();
            }
        }

        public void StopAnimation()
        {
            CheckInitialize();
            foreach (var animation in _animations)
            {
                animation.StopAnimation();
            }
        }

        public void ResetValues()
        {
            CheckInitialize();
            foreach (var animation in _animations)
            {
                animation.ResetValues();
            }
        }

        public void InitializeChildren()
        {
            _animations.Clear();
            for (var i = 0; i < _parentObject.childCount; i++)
            {
                var child = _parentObject.GetChild(i);
                var animation = ITween.Clone(_tween, child.gameObject);
                _animations.Add(animation);
            }
        }

        private void CheckInitialize()
        {
            if (_animations.Count == 0) InitializeChildren();
        }
        
        
    }
}