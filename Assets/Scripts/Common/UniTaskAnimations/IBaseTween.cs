﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    public interface IBaseTween
    {
        UniTask StartAnimation(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default);

        void StopAnimation();
        void ResetValues();
        public static IBaseTween Clone(IBaseTween tween, GameObject targetObject = null)
        {
            IBaseTween newTween = tween switch
            {
                GroupTween groupTween => GroupTween.Clone(groupTween, targetObject),
                SimpleTween simpleTween => SimpleTween.Clone(simpleTween, targetObject),
                MultiTween multiTween => MultiTween.Clone(multiTween, targetObject),
                _ => null
            };
            return newTween;
        }
    }
}