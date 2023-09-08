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
            Color endColor;
            AnimationCurve reverseCurve;
            var curTweenTime = TweenTime;
            if (Loop == LoopType.PingPong) curTweenTime /= 2;
            var time = 0f;
            var curLoop = true;

            if (reverse)
            {
                startColor = toColor;
                endColor = fromColor;
                reverseCurve = ReverseCurve;
            }
            else
            {
                startColor = fromColor;
                endColor = toColor;
                reverseCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localColor = tweenGraphic.color;
                var t = 1f;
                if (endColor.r - startColor.r != 0f)
                    t = (localColor.r - startColor.r) / (endColor.r - startColor.r);
                else if (endColor.g - startColor.g != 0f)
                    t = (localColor.g - startColor.g) / (endColor.g - startColor.g);
                else if (endColor.b - startColor.b != 0f)
                    t = (localColor.b - startColor.b) / (endColor.b - startColor.b);

                else if (endColor.a - startColor.a != 0f)
                    t = (localColor.a - startColor.a) / (endColor.a - startColor.a);

                time = curTweenTime * t;
            }

            while (curLoop)
            {
                tweenGraphic.color = startColor;

                while (time < curTweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / curTweenTime;
                    var lerpTime = reverseCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Color.LerpUnclamped(startColor, endColor, lerpTime);

                    tweenGraphic.color = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                tweenGraphic.color = endColor;
                time -= TweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        curLoop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        endColor = startColor;
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