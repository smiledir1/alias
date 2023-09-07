using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class ColorImageTween : SimpleTween
    {
        #region View

        [SerializeField]
        private Color fromColor;

        [SerializeField]
        private Color toColor;

        [SerializeField]
        private Graphic tweenGraphic;

        #endregion /View

        #region Properties

        public Color FromColor => fromColor;
        public Color ToColor => toColor;
        public Graphic TweenGraphic => tweenGraphic;

        #endregion

        #region Constructor

        public ColorImageTween()
        {
            fromColor = Color.white;
            toColor = Color.black;
        }

        public ColorImageTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Graphic tweenGraphic,
            Color fromColor,
            Color toColor) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            this.fromColor = fromColor;
            this.toColor = toColor;
            this.tweenGraphic = tweenGraphic;
        }

        #endregion

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (tweenGraphic == null)
            {
                tweenGraphic = tweenObject.GetComponent<Graphic>();
                if (tweenGraphic == null) return;
            }

            Color startColor;
            Color toColor;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startColor = this.toColor;
                toColor = fromColor;
                animationCurve = ReverseCurve;
            }
            else
            {
                startColor = fromColor;
                toColor = this.toColor;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localColor = tweenGraphic.color;
                var t = 1f;
                if (toColor.r - startColor.r != 0f)
                    t = (localColor.r - startColor.r) / (toColor.r - startColor.r);
                else if (toColor.g - startColor.g != 0f)
                    t = (localColor.g - startColor.g) / (toColor.g - startColor.g);
                else if (toColor.b - startColor.b != 0f)
                    t = (localColor.b - startColor.b) / (toColor.b - startColor.b);

                else if (toColor.a - startColor.a != 0f) t = (localColor.a - startColor.a) / (toColor.a - startColor.a);

                time = tweenTime * t;
            }

            while (loop)
            {
                tweenGraphic.color = startColor;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Color.LerpUnclamped(startColor, toColor, lerpTime);

                    tweenGraphic.color = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                tweenGraphic.color = toColor;
                time -= TweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        toColor = startColor;
                        startColor = tweenGraphic.color;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (tweenGraphic == null) tweenGraphic = TweenObject.GetComponent<Graphic>();
            tweenGraphic.color = fromColor;
        }

        public void SetColor(Color from, Color to)
        {
            fromColor = from;
            toColor = to;
        }

        #endregion /Animation

        #region Static

        public static ColorImageTween Clone(
            ColorImageTween tween,
            GameObject targetObject = null)
        {
            Graphic tweenImage = null;
            if (targetObject != null)
            {
                tweenImage = targetObject.GetComponent<Graphic>();
                if (tweenImage == null) targetObject.AddComponent<Image>();
            }

            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);

            return new ColorImageTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tweenImage,
                tween.FromColor,
                tween.ToColor);
        }

        #endregion
    }
}