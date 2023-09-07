using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public class GroupTween : ITween
    {
        [SerializeField]
        private bool synchronously;

        [SerializeReference]
        private List<ITween> _tweens = new();

        public bool Synchronously => synchronously;
        public List<ITween> Tweens => _tweens;

        private CancellationTokenSource _currentToken;

        public GroupTween(bool synchronously)
        {
            this.synchronously = synchronously;
        }

        public async UniTask StartAnimation(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            _currentToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (synchronously)
            {
                foreach (var tween in _tweens)
                {
                    if (_currentToken.IsCancellationRequested) return;
                    if (tween == null) continue;
                    await tween.StartAnimation(
                        reverse,
                        startFromCurrentValue,
                        _currentToken.Token);
                }
            }
            else
            {
                var tasks = new List<UniTask>();
                foreach (var tween in _tweens)
                {
                    if (tween == null) continue;
                    tasks.Add(
                        tween.StartAnimation(
                            reverse,
                            startFromCurrentValue,
                            _currentToken.Token));
                }

                await UniTask.WhenAll(tasks);
            }
        }

        public void StopAnimation()
        {
            _currentToken.Cancel();
            foreach (var tween in _tweens)
            {
                tween?.StopAnimation();
            }
        }

        public void ResetValues()
        {
            foreach (var tween in _tweens)
            {
                tween?.ResetValues();
            }
        }

        public void AddTween(ITween tween)
        {
            _tweens.Add(tween);
        }

        public static GroupTween Clone(GroupTween tween, GameObject targetObject = null)
        {
            var newTween = new GroupTween(tween.Synchronously);
            foreach (var inTween in tween.Tweens)
            {
                var newInTween = ITween.Clone(inTween, targetObject);
                newTween.AddTween(newInTween);
            }

            return newTween;
        }
    }
}