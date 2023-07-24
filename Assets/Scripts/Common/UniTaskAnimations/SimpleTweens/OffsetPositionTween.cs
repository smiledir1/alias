﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class OffsetPositionTween : SimpleTween
    {
        #region View

        [Header("Current Tween")]
        [SerializeField]
        private Vector3 _fromPosition;

        [SerializeField]
        private Vector3 _toPosition;

        #endregion /View

        #region Properties

        public Vector3 FromPosition => _fromPosition;
        public Vector3 ToPosition => _toPosition;

        #endregion

        #region Constructor

        public OffsetPositionTween()
        {
            _fromPosition = Vector3.zero;
            _toPosition = Vector3.zero;
        }

        public OffsetPositionTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Vector3 fromPosition,
            Vector3 toPosition) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            _fromPosition = fromPosition;
            _toPosition = toPosition;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (TweenObject == null) return;
            
            var targetPosition = TweenObject.transform.localPosition;

            Vector3 startPosition;
            Vector3 toPosition;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startPosition = targetPosition + _toPosition;
                toPosition = targetPosition + _fromPosition;
                animationCurve = ReverseCurve;
            }
            else
            {
                startPosition = targetPosition + _fromPosition;
                toPosition = targetPosition + _toPosition;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localPosition = TweenObject.transform.localPosition;
                var t = 1f;
                if (toPosition.x - startPosition.x != 0f)
                {
                    t = (localPosition.x - startPosition.x) / (toPosition.x - startPosition.x);
                }
                else if (toPosition.y - startPosition.y != 0f)
                {
                    t = (localPosition.y - startPosition.y) / (toPosition.y - startPosition.y);
                }
                else if (toPosition.z - startPosition.z != 0f)
                {
                    t = (localPosition.z - startPosition.z) / (toPosition.z - startPosition.z);
                }

                time = tweenTime * t;
            }

            while (loop)
            {
                TweenObject.transform.localPosition = startPosition;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Vector3.LerpUnclamped(startPosition, toPosition, lerpTime);

                    TweenObject.transform.localPosition = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                TweenObject.transform.localPosition = toPosition;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        toPosition = startPosition;
                        startPosition = TweenObject.transform.localPosition;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
        }

        public void SetPosition(Vector3 from, Vector3 to)
        {
            _fromPosition = from;
            _toPosition = to;
        }

        #endregion /Animation

        #region Editor

#if UNITY_EDITOR
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.Selected)]
        private static void OnDrawGizmo(TweenComponent component, UnityEditor.GizmoType gizmoType)
        {
            if (component.Tween is not OffsetPositionTween positionTween) return;
            Gizmos.color = Color.magenta;
            var targetPosition =
                positionTween.TweenObject == null ||
                positionTween.TweenObject.transform == null
                    ? Vector3.zero
                    : positionTween.TweenObject.transform.position;
            var fromPosition = targetPosition + positionTween.FromPosition;
            var toPosition = targetPosition + positionTween.ToPosition;
            Gizmos.DrawLine(fromPosition, toPosition);
        }
#endif

        #endregion

        #region Static

        public static OffsetPositionTween Clone(
            OffsetPositionTween tween,
            GameObject targetObject = null)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);

            return new OffsetPositionTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tween.FromPosition,
                tween.ToPosition);
        }

        #endregion
    }
}