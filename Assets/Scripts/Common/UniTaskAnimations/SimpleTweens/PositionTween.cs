using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class PositionTween : SimpleTween
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

        public PositionTween()
        {
            _fromPosition = Vector3.zero;
            _toPosition = Vector3.zero;
        }

        public PositionTween(
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
            Vector3 startPosition;
            Vector3 toPosition;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startPosition = _toPosition;
                toPosition = _fromPosition;
                animationCurve = ReverseCurve;
            }
            else
            {
                startPosition = _fromPosition;
                toPosition = _toPosition;
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
            TweenObject.transform.localPosition = _fromPosition;
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
            if (component.Tween is not PositionTween positionTween) return;
            Gizmos.color = Color.magenta;
            var parentPosition =
                positionTween.TweenObject == null ||
                positionTween.TweenObject.transform == null
                    ? Vector3.zero
                    : positionTween.TweenObject.transform.parent.position;
            var fromPosition = parentPosition + positionTween.FromPosition;
            var toPosition = parentPosition + positionTween.ToPosition;
            Gizmos.DrawLine(fromPosition, toPosition);
        }
#endif

        #endregion
        
        #region Static

        public static PositionTween Clone(
            PositionTween tween,
            GameObject targetObject = null)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);
                    
            return new PositionTween(
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