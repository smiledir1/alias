using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class FillImageTween : SimpleTween
    {
        #region View

        [SerializeField]
        [Range(0, 1)]
        private float fromFill;

        [SerializeField]
        [Range(0, 1)]
        private float toFill;

        [SerializeField]
        private Image tweenImage;

        #endregion /View

        #region Properties

        public float FromFill => fromFill;
        public float ToFill => toFill;
        public Image TweenImage => tweenImage;

        #endregion

        #region Constructor

        public FillImageTween()
        {
            fromFill = 0;
            toFill = 1;
        }

        public FillImageTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Image tweenImage,
            float fromFill,
            float toFill) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            this.fromFill = fromFill;
            this.toFill = toFill;
            this.tweenImage = tweenImage;
        }

        #endregion

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (tweenImage == null)
            {
                tweenImage = tweenObject.GetComponent<Image>();
                if (tweenImage == null) return;
            }

            float startFill;
            float toFill;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startFill = this.toFill;
                toFill = fromFill;
                animationCurve = ReverseCurve;
            }
            else
            {
                startFill = fromFill;
                toFill = this.toFill;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var currentValue = tweenImage.fillAmount;
                var t = (currentValue - startFill) / (toFill - startFill);
                time = tweenTime * t;
            }

            while (loop)
            {
                tweenImage.fillAmount = startFill;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Mathf.LerpUnclamped(startFill, toFill, lerpTime);

                    if (tweenImage == null) return;
                    tweenImage.fillAmount = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                tweenImage.fillAmount = toFill;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        if (tweenImage == null) return;
                        toFill = startFill;
                        startFill = tweenImage.fillAmount;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (tweenImage == null) tweenImage = TweenObject.GetComponent<Image>();
            tweenImage.fillAmount = fromFill;
        }

        public void SetFill(float from, float to)
        {
            fromFill = from;
            toFill = to;
        }

        #endregion /Animation

        #region Static

        public static FillImageTween Clone(
            FillImageTween tween,
            GameObject targetObject = null)
        {
            Image tweenImage = null;
            if (targetObject != null)
            {
                tweenImage = targetObject.GetComponent<Image>();
                if (tweenImage == null) targetObject.AddComponent<Image>();
            }

            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);

            return new FillImageTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tweenImage,
                tween.FromFill,
                tween.ToFill);
        }

        #endregion
    }
}