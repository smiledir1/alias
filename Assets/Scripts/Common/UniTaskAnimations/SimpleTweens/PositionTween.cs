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
        private PositionType _positionType;

        [SerializeField]
        private Vector3 _fromPosition;

        [SerializeField]
        private Vector3 _toPosition;

        #endregion /View

        #region Properties

        public PositionType PositionType => _positionType;
        public Vector3 FromPosition => _fromPosition;
        public Vector3 ToPosition => _toPosition;

        #endregion

        #region Cache

        private RectTransform _rectTransform;

        #endregion

        #region Constructor

        public PositionTween()
        {
            _positionType = PositionType.Local;
            _fromPosition = Vector3.zero;
            _toPosition = Vector3.zero;
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
            _positionType = positionType;
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
            if (_tweenObject == null) return;

            _rectTransform = _tweenObject.transform as RectTransform;
            Vector3 startPosition;
            Vector3 toPosition;
            AnimationCurve animationCurve;
            var tweenTime = _tweenTime;
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
                animationCurve = _animationCurve;
            }

            if (startFromCurrentValue)
            {
                var currentPosition = GetCurrentPosition();
                var t = 1f;
                if (toPosition.x - startPosition.x != 0f)
                    t = (currentPosition.x - startPosition.x) / (toPosition.x - startPosition.x);
                else if (toPosition.y - startPosition.y != 0f)
                    t = (currentPosition.y - startPosition.y) / (toPosition.y - startPosition.y);
                else if (toPosition.z - startPosition.z != 0f)
                    t = (currentPosition.z - startPosition.z) / (toPosition.z - startPosition.z);

                time = tweenTime * t;
            }

            while (loop)
            {
                GoToPosition(startPosition);

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Vector3.LerpUnclamped(startPosition, toPosition, lerpTime);

                    GoToPosition(lerpValue);
                    await UniTask.Yield(cancellationToken);
                }

                GoToPosition(toPosition);
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
                        startPosition = GetCurrentPosition();
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            GoToPosition(_fromPosition);
        }

        public void SetPositions(Vector3 from, Vector3 to, PositionType positionType)
        {
            _positionType = positionType;
            _fromPosition = from;
            _toPosition = to;
        }

        internal Vector3 GetCurrentPosition()
        {
            return _positionType switch
            {
                PositionType.Local => _tweenObject.transform.localPosition,
                PositionType.Global => _tweenObject.transform.position,
                PositionType.Anchored => _rectTransform.anchoredPosition,
                _ => Vector3.zero
            };
        }

        internal void GoToPosition(Vector3 position)
        {
            switch (_positionType)
            {
                case PositionType.Local:
                    _tweenObject.transform.localPosition = position;
                    return;
                case PositionType.Global:
                    _tweenObject.transform.position = position;
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
            if (_tweenObject != null) _rectTransform = _tweenObject.transform as RectTransform;
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