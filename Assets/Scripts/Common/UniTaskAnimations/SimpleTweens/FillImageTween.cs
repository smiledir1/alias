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
        private float _fromFill;

        [SerializeField]
        [Range(0, 1)]
        private float _toFill;

        [SerializeField]
        private Image _tweenImage;

        #endregion /View

        #region Properties

        public float FromFill => _fromFill;
        public float ToFill => _toFill;
        public Image TweenImage => _tweenImage;

        #endregion

        #region Constructor

        public FillImageTween()
        {
            _fromFill = 0;
            _toFill = 1;
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
            _fromFill = fromFill;
            _toFill = toFill;
            _tweenImage = tweenImage;
        }

        #endregion

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (_tweenImage == null)
            {
                _tweenImage = _tweenObject.GetComponent<Image>();
                if (_tweenImage == null) return;
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
                startFill = _toFill;
                toFill = _fromFill;
                animationCurve = ReverseCurve;
            }
            else
            {
                startFill = _fromFill;
                toFill = _toFill;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var currentValue = _tweenImage.fillAmount;
                var t = (currentValue - startFill) / (toFill - startFill);
                time = tweenTime * t;
            }

            while (loop)
            {
                _tweenImage.fillAmount = startFill;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Mathf.LerpUnclamped(startFill, toFill, lerpTime);

                    if (_tweenImage == null) return;
                    _tweenImage.fillAmount = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                _tweenImage.fillAmount = toFill;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        if (_tweenImage == null) return;
                        toFill = startFill;
                        startFill = _tweenImage.fillAmount;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (_tweenImage == null) _tweenImage = TweenObject.GetComponent<Image>();
            _tweenImage.fillAmount = _fromFill;
        }

        public void SetFill(float from, float to)
        {
            _fromFill = from;
            _toFill = to;
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