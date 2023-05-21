using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public class RotationTween : SimpleTween
    {
        #region View

        [Header("Current Tween")]
        [SerializeField]
        private Vector3 _fromRotation;

        [SerializeField]
        private Vector3 _toRotation;

        #endregion /View

        #region Properties

        public Vector3 FromRotation => _fromRotation;
        public Vector3 ToRotation => _toRotation;

        #endregion

        #region Constructor

        public RotationTween()
        {
            _fromRotation = Vector3.zero;
            _toRotation = Vector3.zero;
        }

        public RotationTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Vector3 fromRotation,
            Vector3 toRotation) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            _fromRotation = fromRotation;
            _toRotation = toRotation;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            Vector3 startRotation;
            Vector3 toRotation;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startRotation = _toRotation;
                toRotation = _fromRotation;
            }
            else
            {
                startRotation = _fromRotation;
                toRotation = _toRotation;
            }

            if (startFromCurrentValue)
            {
                var localScale = TweenObject.transform.localScale;
                startRotation = localScale;
                var currentValue = localScale.x;
                var currentPartValue = Mathf.Abs(toRotation.x - currentValue);
                var maxValue = Mathf.Abs(startRotation.x - toRotation.x);
                var normalizePart = currentPartValue / maxValue;
                tweenTime *= normalizePart;
            }

            while (loop)
            {
                TweenObject.transform.eulerAngles = startRotation;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = AnimationCurve.Evaluate(normalizeTime);
                    var lerpValue = Vector3.LerpUnclamped(startRotation, toRotation, lerpTime);

                    TweenObject.transform.eulerAngles = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                TweenObject.transform.eulerAngles = toRotation;
                time -= TweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        toRotation = startRotation;
                        startRotation = TweenObject.transform.eulerAngles;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            TweenObject.transform.eulerAngles = _fromRotation;
        }

        public void SetRotation(Vector3 from, Vector3 to)
        {
            _fromRotation = from;
            _toRotation = to;
        }

        #endregion /Animation
    }
}