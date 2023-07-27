using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class BezierPositionTween : SimpleTween
    {
        #region View

        [SerializeField]
        private PositionType _positionType;

        [SerializeField]
        private Vector3 _fromPosition;

        [SerializeField]
        private Vector3 _toPosition;

        [SerializeField]
        private Vector3 _bezier1Offset;

        [SerializeField]
        private Vector3 _bezier2Offset;

        [SerializeField]
        [Range(0.001f, 0.5f)]
        private float _precision = 0.05f;

        #endregion /View

        #region Properties

        public PositionType PositionType => _positionType;
        public Vector3 FromPosition => _fromPosition;
        public Vector3 ToPosition => _toPosition;
        public Vector3 Bezier1Offset => _bezier1Offset;
        public Vector3 Bezier2Offset => _bezier2Offset;
        public float Precision => _precision;

        #endregion

        #region Cache

        private Vector3[] _bezierPoints;
        private float[] _bezierLens;
        private RectTransform _rectTransform;

        #endregion

        #region Constructor

        public BezierPositionTween()
        {
            _positionType = PositionType.Local;
            _fromPosition = Vector3.zero;
            _toPosition = Vector3.zero;
            _bezier1Offset = Vector3.zero;
            _bezier2Offset = Vector3.zero;
            _precision = 0.05f;
            CreatePoints();
        }

        public BezierPositionTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            PositionType positionType,
            Vector3 fromPosition,
            Vector3 toPosition,
            Vector3 bezier1Offset,
            Vector3 bezier2Offset,
            float precision = 0.05f) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            if (precision <= 0f)
            {
                throw new Exception("precision must be > 0");
            }

            _positionType = positionType;
            _fromPosition = fromPosition;
            _toPosition = toPosition;
            _bezier1Offset = bezier1Offset;
            _bezier2Offset = bezier2Offset;
            _precision = precision;

            CreatePoints();
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (TweenObject == null) return;

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
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localPosition = GetCurrentPosition();

                var t2 = 0f;
                for (var i = 1; i < _bezierPoints.Length; i++)
                {
                    var from = _bezierPoints[i - 1];
                    var to = _bezierPoints[i];
                    var qtx = to.x - from.x == 0f
                        ? 0f
                        : (localPosition.x - from.x) / (to.x - from.x);
                    var qty = to.y - from.y == 0f
                        ? 0f
                        : (localPosition.y - from.y) / (to.y - from.y);
                    var qtz = to.z - from.z == 0f
                        ? 0f
                        : (localPosition.z - from.z) / (to.z - from.z);
                    var qt = qtx > qty
                        ? qtx > qtz
                            ? qtx
                            : qtz
                        : qty > qtz
                            ? qty
                            : qtz;
                    if (qtx is < 0 or > 1 ||
                        qty is < 0 or > 1 ||
                        qtz is < 0 or > 1) continue;
                    var fromLen = _bezierLens[i - 1];
                    var toLen = _bezierLens[i];
                    var ft = fromLen + (toLen - fromLen) * qt;
                    t2 = ft;
                }

                time = tweenTime * t2;
            }

            while (loop)
            {
                GoToPosition(startPosition);
                var cur = reverse ? _bezierLens.Length - 2 : 1;
                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;

                    Vector3 startPoint;
                    Vector3 toPoint;
                    float startLen;
                    float endLen;

                    if (reverse)
                    {
                        lerpTime = 1f - lerpTime;
                        for (var i = cur; i >= 0; i--)
                        {
                            if (_bezierLens[i] > lerpTime) continue;
                            cur = i;
                            break;
                        }

                        startPoint = _bezierPoints[cur];
                        toPoint = _bezierPoints[cur + 1];

                        startLen = _bezierLens[cur];
                        endLen = _bezierLens[cur + 1];
                    }
                    else
                    {
                        for (var i = cur; i < _bezierLens.Length; i++)
                        {
                            if (_bezierLens[i] < lerpTime) continue;
                            cur = i;
                            break;
                        }

                        startPoint = _bezierPoints[cur - 1];
                        toPoint = _bezierPoints[cur];

                        startLen = _bezierLens[cur - 1];
                        endLen = _bezierLens[cur];
                    }

                    var valueTime = (lerpTime - startLen) / (endLen - startLen);

                    var lerpValue = Vector3.LerpUnclamped(startPoint, toPoint, valueTime);
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
                        reverse = !reverse;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            GoToPosition(_fromPosition);
        }

        public void SetPositions(
            PositionType positionType,
            Vector3 from,
            Vector3 to,
            Vector3 bezier1Offset,
            Vector3 bezier2Offset)
        {
            _positionType = positionType;
            _fromPosition = from;
            _toPosition = to;
            _bezier1Offset = bezier1Offset;
            _bezier2Offset = bezier2Offset;
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

        #region Private

        private void CreatePoints()
        {
            var b0 = _fromPosition;
            var b3 = _toPosition;

            var b1 = b0 + _bezier1Offset;
            var b2 = b3 + _bezier2Offset;

            var min = 0.0001f;
            var count = (int) ((1f - min) / _precision) + 2;

            _bezierPoints = new Vector3[count];
            _bezierLens = new float[count];

            _bezierPoints[0] = b0;
            _bezierLens[0] = 0f;

            var lastPos = count - 1;
            var fullSqrPath = 0f;
            float len;
            for (var i = 1; i < lastPos; i++)
            {
                var t = i * _precision;
                _bezierPoints[i] = CalculatePointPosition(b0, b1, b2, b3, t);

                len = (_bezierPoints[i] - _bezierPoints[i - 1]).magnitude;
                _bezierLens[i] = fullSqrPath + len;
                fullSqrPath += len;
            }

            _bezierPoints[lastPos] = b3;
            len = (_bezierPoints[lastPos] - _bezierPoints[lastPos - 1]).magnitude;
            _bezierLens[lastPos] = fullSqrPath + len;
            fullSqrPath += len;


            for (var i = 0; i < count; i++)
            {
                _bezierLens[i] /= fullSqrPath;
            }
        }

        private static Vector3 CalculatePointPosition(
            Vector3 b0,
            Vector3 b1,
            Vector3 b2,
            Vector3 b3,
            float t)
        {
            return Mathf.Pow(1 - t, 3) * b0 +
                   3 * Mathf.Pow(1 - t, 2) * t * b1 +
                   3 * (1 - t) * Mathf.Pow(t, 2) * b2 +
                   Mathf.Pow(t, 3) * b3;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        [UnityEditor.DrawGizmo(UnityEditor.GizmoType.Selected)]
        private static void OnDrawGizmo(TweenComponent component, UnityEditor.GizmoType gizmoType)
        {
            if (component.Tween is not BezierPositionTween bezierPositionTween) return;
            if (bezierPositionTween.Precision < 0.001f) return;

            Gizmos.color = Color.magenta;

            switch (bezierPositionTween.PositionType)
            {
                case PositionType.Local:
                    DrawLocalPosition(bezierPositionTween);
                    break;
                case PositionType.Global:
                    DrawGlobalPosition(bezierPositionTween);
                    break;
                case PositionType.Anchored:
                    DrawAnchoredPosition(bezierPositionTween);
                    break;
            }
        }

        private static void DrawLocalPosition(BezierPositionTween bezierPositionTween)
        {
            var parent = bezierPositionTween.TweenObject == null ||
                         bezierPositionTween.TweenObject.transform == null ||
                         bezierPositionTween.TweenObject.transform.parent == null
                ? null
                : bezierPositionTween.TweenObject.transform.parent;

            var parentPosition = parent == null ? Vector3.zero : parent.position;
            var parentScale = parent == null ? Vector3.one : parent.lossyScale;

            var b0 = parentPosition
                     + GetScaledPosition(parentScale, bezierPositionTween.FromPosition);
            var b3 = parentPosition
                     + GetScaledPosition(parentScale, bezierPositionTween.ToPosition);

            var b1 = b0
                     + GetScaledPosition(parentScale, bezierPositionTween.Bezier1Offset);
            var b2 = b3
                     + GetScaledPosition(parentScale, bezierPositionTween.Bezier2Offset);

            var count = bezierPositionTween._bezierPoints.Length;
            var lines = new Vector3[count];

            for (var i = 0; i < count; i++)
            {
                lines[i] = parentPosition
                           + GetScaledPosition(parentScale, bezierPositionTween._bezierPoints[i]);
            }

            Gizmos.DrawSphere(b0, 10f);
            Gizmos.DrawSphere(b1, 10f);
            Gizmos.DrawSphere(b2, 10f);
            Gizmos.DrawSphere(b3, 10f);
            Gizmos.DrawLineStrip(lines, false);
        }

        private static Vector3 GetScaledPosition(Vector3 scale, Vector3 position)
        {
            return new Vector3(
                position.x * scale.x,
                position.y * scale.y,
                position.z * scale.z);
        }

        private static void DrawGlobalPosition(BezierPositionTween bezierPositionTween)
        {
            var b0 = bezierPositionTween.FromPosition;
            var b3 = bezierPositionTween.ToPosition;

            var b1 = b0 + bezierPositionTween.Bezier1Offset;
            var b2 = b3 + bezierPositionTween.Bezier2Offset;

            Gizmos.DrawSphere(b0, 10f);
            Gizmos.DrawSphere(b1, 10f);
            Gizmos.DrawSphere(b2, 10f);
            Gizmos.DrawSphere(b3, 10f);
            Gizmos.DrawLineStrip(bezierPositionTween._bezierPoints, false);
        }

        private static void DrawAnchoredPosition(BezierPositionTween bezierPositionTween)
        {
            var parent = bezierPositionTween.TweenObject == null ||
                         bezierPositionTween.TweenObject.transform == null ||
                         bezierPositionTween.TweenObject.transform.parent == null
                ? null
                : bezierPositionTween.TweenObject.transform.parent;

            var parentPosition = parent == null ? Vector3.zero : parent.position;
            var parentScale = parent == null ? Vector3.one : parent.lossyScale;
            var rectTransform = bezierPositionTween._rectTransform;
            var difPosition = rectTransform.localPosition - rectTransform.anchoredPosition3D;
            var difScaled = GetScaledPosition(parentScale, difPosition);

            var b0 = parentPosition
                     + GetScaledPosition(parentScale, bezierPositionTween.FromPosition)
                     + difScaled;
            var b3 = parentPosition
                     + GetScaledPosition(parentScale, bezierPositionTween.ToPosition)
                     + difScaled;
            var b1 = b0
                     + GetScaledPosition(parentScale, bezierPositionTween.Bezier1Offset);
            var b2 = b3
                     + GetScaledPosition(parentScale, bezierPositionTween.Bezier2Offset);

            var count = bezierPositionTween._bezierPoints.Length;
            var lines = new Vector3[count];

            for (var i = 0; i < count; i++)
            {
                lines[i] = parentPosition
                           + GetScaledPosition(parentScale, bezierPositionTween._bezierPoints[i])
                           + difScaled;
            }

            Gizmos.DrawSphere(b0, 10f);
            Gizmos.DrawSphere(b1, 10f);
            Gizmos.DrawSphere(b2, 10f);
            Gizmos.DrawSphere(b3, 10f);
            Gizmos.DrawLineStrip(lines, false);
        }

        public override void OnGuiChange()
        {
            if (_tweenObject != null) _rectTransform = _tweenObject.transform as RectTransform;
            CreatePoints();
            base.OnGuiChange();
        }

#endif

        #endregion

        #region Static

        public static BezierPositionTween Clone(
            BezierPositionTween tween,
            GameObject targetObject = null)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);

            return new BezierPositionTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tween.PositionType,
                tween.FromPosition,
                tween.ToPosition,
                tween.Bezier1Offset,
                tween.Bezier2Offset,
                tween.Precision);
        }

        #endregion
    }
}