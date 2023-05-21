using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public class ScaleTween : SimpleTween
    {
        #region View

        [Header("Current Tween")]
        [SerializeField]
        private Vector3 _fromScale;

        [SerializeField]
        private Vector3 _toScale;

        #endregion /View

        #region Properties
        
        public Vector3 FromScale => _fromScale;
        public Vector3 ToScale => _toScale;

        #endregion

        #region Constructor

        public ScaleTween()
        {
            _fromScale = Vector3.zero;
            _toScale = Vector3.one;
        }

        public ScaleTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Vector3 fromScale,
            Vector3 toScale) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            _fromScale = fromScale;
            _toScale = toScale;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            Vector3 startScale;
            Vector3 toScale;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startScale = _toScale;
                toScale = _fromScale;
            }
            else
            {
                startScale = _fromScale;
                toScale = _toScale;
            }

            if (startFromCurrentValue)
            {
                var localScale = TweenObject.transform.localScale;
                startScale = localScale;
                var currentValue = localScale.x;
                var currentPartValue = Mathf.Abs(toScale.x - currentValue);
                var maxValue = Mathf.Abs(startScale.x - toScale.x);
                var normalizePart = currentPartValue / maxValue;
                tweenTime *= normalizePart;
            }

            while (loop)
            {
                TweenObject.transform.localScale = startScale;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = AnimationCurve.Evaluate(normalizeTime);
                    var lerpValue = Vector3.LerpUnclamped(startScale, toScale, lerpTime);

                    TweenObject.transform.localScale = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                TweenObject.transform.localScale = toScale;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        toScale = startScale;
                        startScale = TweenObject.transform.localScale;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            TweenObject.transform.localScale = _fromScale;
        }

        public void SetScale(Vector3 from, Vector3 to)
        {
            _fromScale = from;
            _toScale = to;
        }

        #endregion /Animation
    }
}