using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class TransparencyCanvasGroupTween : SimpleTween
    {
        #region View

        [SerializeField]
        [Range(0, 1)]
        private float fromOpacity;

        [SerializeField]
        [Range(0, 1)]
        private float toOpacity;

        [SerializeField]
        private CanvasGroup tweenObjectRenderer;

        #endregion /View

        #region Properties

        public float FromOpacity => fromOpacity;
        public float ToOpacity => toOpacity;
        public CanvasGroup TweenObjectRenderer => tweenObjectRenderer;

        #endregion

        #region Constructor

        public TransparencyCanvasGroupTween()
        {
            fromOpacity = 0;
            toOpacity = 1;
        }

        public TransparencyCanvasGroupTween(
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
            this.fromOpacity = fromOpacity;
            this.toOpacity = toOpacity;
            this.tweenObjectRenderer = tweenObjectRenderer;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (tweenObjectRenderer == null)
            {
                tweenObjectRenderer = tweenObject.GetComponent<CanvasGroup>();
                if (tweenObjectRenderer == null) return;
            }

            float startOpacity;
            float toOpacity;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startOpacity = this.toOpacity;
                toOpacity = fromOpacity;
                animationCurve = ReverseCurve;
            }
            else
            {
                startOpacity = fromOpacity;
                toOpacity = this.toOpacity;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var currentValue = tweenObjectRenderer.alpha;
                var t = (currentValue - startOpacity) / (toOpacity - startOpacity);
                time = tweenTime * t;
            }

            while (loop)
            {
                tweenObjectRenderer.alpha = startOpacity;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Mathf.LerpUnclamped(startOpacity, toOpacity, lerpTime);

                    if (tweenObjectRenderer == null) return;
                    tweenObjectRenderer.alpha = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                tweenObjectRenderer.alpha = toOpacity;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        if (tweenObjectRenderer == null) return;
                        toOpacity = startOpacity;
                        startOpacity = tweenObjectRenderer.alpha;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (tweenObjectRenderer == null) tweenObjectRenderer = TweenObject.GetComponent<CanvasGroup>();
            tweenObjectRenderer.alpha = fromOpacity;
        }

        public void SetTransparency(float from, float to)
        {
            fromOpacity = from;
            toOpacity = to;
        }

        #endregion /Animation

        #region Static

        public static TransparencyCanvasGroupTween Clone(
            TransparencyCanvasGroupTween tween,
            GameObject targetObject = null)
        {
            CanvasGroup canvasGroup = null;
            if (targetObject != null)
            {
                canvasGroup = targetObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null) targetObject.AddComponent<CanvasGroup>();
            }

            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);

            return new TransparencyCanvasGroupTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                canvasGroup,
                tween.FromOpacity,
                tween.ToOpacity);
        }

        #endregion
    }
}