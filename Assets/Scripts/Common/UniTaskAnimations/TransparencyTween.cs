using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public class TransparencyTween : SimpleTween
    {
        #region View

        [Header("Current Tween")]
        [SerializeField]
        [Range(0, 1)]
        private float _fromOpacity;

        [SerializeField]
        [Range(0, 1)]
        private float _toOpacity;

        [SerializeField]
        private CanvasGroup _tweenObjectRenderer;

        #endregion /View

        #region Properties

        public float FromOpacity => _fromOpacity;
        public float ToOpacity => _toOpacity;
        public CanvasGroup TweenObjectRenderer => _tweenObjectRenderer;

        #endregion

        #region Constructor

        public TransparencyTween()
        {
            _fromOpacity = 0;
            _toOpacity = 1;
        }

        public TransparencyTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            CanvasGroup tweenObjectRenderer,
            float fromOpacity,
            float toOpacity) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            _fromOpacity = fromOpacity;
            _toOpacity = toOpacity;
            _tweenObjectRenderer = tweenObjectRenderer;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (_tweenObjectRenderer == null) return;
            float startOpacity;
            float toOpacity;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startOpacity = _toOpacity;
                toOpacity = _fromOpacity;
            }
            else
            {
                startOpacity = _fromOpacity;
                toOpacity = _toOpacity;
            }

            if (startFromCurrentValue)
            {
                var currentValue = _tweenObjectRenderer.alpha;
                startOpacity = currentValue;
                var currentPartValue = Mathf.Abs(toOpacity - currentValue);
                var maxValue = Mathf.Abs(startOpacity - toOpacity);
                var normalizePart = currentPartValue / maxValue;
                tweenTime *= normalizePart;
            }

            while (loop)
            {
                _tweenObjectRenderer.alpha = startOpacity;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = AnimationCurve.Evaluate(normalizeTime);
                    var lerpValue = Mathf.LerpUnclamped(startOpacity, toOpacity, lerpTime);

                    if (_tweenObjectRenderer == null) return;
                    _tweenObjectRenderer.alpha = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                _tweenObjectRenderer.alpha = toOpacity;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        if (_tweenObjectRenderer == null) return;
                        toOpacity = startOpacity;
                        startOpacity = _tweenObjectRenderer.alpha;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (_tweenObjectRenderer == null) _tweenObjectRenderer = TweenObject.GetComponent<CanvasGroup>();
            _tweenObjectRenderer.alpha = _fromOpacity;
        }

        public void SetTransparency(float from, float to)
        {
            _fromOpacity = from;
            _toOpacity = to;
        }

        #endregion /Animation
    }
}