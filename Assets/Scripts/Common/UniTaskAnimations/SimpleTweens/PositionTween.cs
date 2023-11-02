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

        [SerializeField]
        private PositionType positionType;

        [SerializeField]
        private Vector3 fromPosition;

        [SerializeField]
        private Vector3 toPosition;

        #endregion /View

        #region Properties

        public PositionType PositionType => positionType;
        public Vector3 FromPosition => fromPosition;
        public Vector3 ToPosition => toPosition;

        #endregion

        #region Cache

        private RectTransform _rectTransform;

        #endregion

        #region Constructor

        public PositionTween()
        {
            positionType = PositionType.Local;
            fromPosition = Vector3.zero;
            toPosition = Vector3.zero;
        }

        public PositionTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            PositionType positionType,
            Vector3 fromPosition,
            Vector3 toPosition) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            this.positionType = positionType;
            this.fromPosition = fromPosition;
            this.toPosition = toPosition;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (tweenObject == null) return;

            _rectTransform = tweenObject.transform as RectTransform;
            Vector3 startPosition;
            Vector3 endPosition;
            AnimationCurve curve;
            var curTweenTime = tweenTime;
            if (Loop == LoopType.PingPong) curTweenTime /= 2;
            var time = 0f;
            var curLoop = true;

            if (reverse)
            {
                startPosition = toPosition;
                endPosition = fromPosition;
                curve = ReverseCurve;
            }
            else
            {
                startPosition = fromPosition;
                endPosition = toPosition;
                curve = animationCurve;
            }

            if (startFromCurrentValue)
            {
                var currentPosition = GetCurrentPosition();
                var t = 1f;
                if (endPosition.x - startPosition.x != 0f)
                    t = (currentPosition.x - startPosition.x) / (endPosition.x - startPosition.x);
                else if (endPosition.y - startPosition.y != 0f)
                    t = (currentPosition.y - startPosition.y) / (endPosition.y - startPosition.y);
                else if (endPosition.z - startPosition.z != 0f)
                    t = (currentPosition.z - startPosition.z) / (endPosition.z - startPosition.z);

                time = curTweenTime * t;
            }

            while (curLoop)
            {
                GoToPosition(startPosition);

                while (time < curTweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / curTweenTime;
                    var lerpTime = curve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Vector3.LerpUnclamped(startPosition, endPosition, lerpTime);

                    GoToPosition(lerpValue);
                    await UniTask.Yield(cancellationToken);
                }

                GoToPosition(endPosition);
                time -= curTweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        curLoop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        endPosition = startPosition;
                        startPosition = GetCurrentPosition();
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            GoToPosition(fromPosition);
        }

        public void SetPositions(Vector3 from, Vector3 to, PositionType curPositionType)
        {
            positionType = curPositionType;
            fromPosition = from;
            toPosition = to;
        }

        internal Vector3 GetCurrentPosition()
        {
            return positionType switch
            {
                PositionType.Local => tweenObject.transform.localPosition,
                PositionType.Global => tweenObject.transform.position,
                PositionType.Anchored => _rectTransform.anchoredPosition,
                _ => Vector3.zero
            };
        }

        internal void GoToPosition(Vector3 position)
        {
            if (tweenObject == null || tweenObject.transform == null) return;
            switch (positionType)
            {
                case PositionType.Local:
                    tweenObject.transform.localPosition = position;
                    return;
                case PositionType.Global:
                    tweenObject.transform.position = position;
                    return;
                case PositionType.Anchored:
                    _rectTransform.anchoredPosition = position;
                    return;
            }
        }

        #endregion /Animation

        #region Editor

#if UNITY_EDITOR
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.Selected)]
        private static void OnDrawGizmo(TweenComponent component, UnityEditor.GizmoType gizmoType)
        {
            if (component.Tween is not PositionTween positionTween) return;
            Gizmos.color = Color.magenta;

            switch (positionTween.PositionType)
            {
                case PositionType.Local:
                    DrawLocalPosition(positionTween);
                    break;
                case PositionType.Global:
                    DrawGlobalPosition(positionTween);
                    break;
                case PositionType.Anchored:
                    DrawAnchoredPosition(positionTween);
                    break;
            }
        }

        private static void DrawLocalPosition(PositionTween positionTween)
        {
            var parent = positionTween.TweenObject == null ||
                         positionTween.TweenObject.transform == null ||
                         positionTween.TweenObject.transform.parent == null
                ? null
                : positionTween.TweenObject.transform.parent;

            var parentPosition = parent == null ? Vector3.zero : parent.position;
            var parentScale = parent == null ? Vector3.one : parent.lossyScale;

            var fromPosition = parentPosition
                               + GetScaledPosition(parentScale, positionTween.FromPosition);
            var toPosition = parentPosition
                             + GetScaledPosition(parentScale, positionTween.ToPosition);
            Gizmos.DrawLine(fromPosition, toPosition);

            Gizmos.DrawSphere(fromPosition, 10f);
            Gizmos.DrawSphere(toPosition, 10f);
        }

        private static Vector3 GetScaledPosition(Vector3 scale, Vector3 position) =>
            new(position.x * scale.x,
                position.y * scale.y,
                position.z * scale.z);

        private static void DrawGlobalPosition(PositionTween positionTween)
        {
            Gizmos.DrawLine(positionTween.FromPosition, positionTween.ToPosition);

            Gizmos.DrawSphere(positionTween.FromPosition, 10f);
            Gizmos.DrawSphere(positionTween.ToPosition, 10f);
        }

        private static void DrawAnchoredPosition(PositionTween positionTween)
        {
            var parent = positionTween.TweenObject == null ||
                         positionTween.TweenObject.transform == null ||
                         positionTween.TweenObject.transform.parent == null
                ? null
                : positionTween.TweenObject.transform.parent;

            var parentPosition = parent == null ? Vector3.zero : parent.position;
            var parentScale = parent == null ? Vector3.one : parent.lossyScale;
            var rectTransform = positionTween._rectTransform;
            var difPosition = rectTransform.localPosition - rectTransform.anchoredPosition3D;
            var difScaled = GetScaledPosition(parentScale, difPosition);

            var fromPosition = parentPosition
                               + GetScaledPosition(parentScale, positionTween.FromPosition)
                               + difScaled;

            var toPosition = parentPosition +
                             GetScaledPosition(parentScale, positionTween.ToPosition)
                             + difScaled;

            Gizmos.DrawLine(fromPosition, toPosition);
            Gizmos.DrawSphere(fromPosition, 10f);
            Gizmos.DrawSphere(toPosition, 10f);
        }

        public override void OnGuiChange()
        {
            if (tweenObject != null) _rectTransform = tweenObject.transform as RectTransform;
            base.OnGuiChange();
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
                tween.PositionType,
                tween.FromPosition,
                tween.ToPosition);
        }

        #endregion
    }
}