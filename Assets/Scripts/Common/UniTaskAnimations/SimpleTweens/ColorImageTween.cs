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
    
        [Header("Current Tween")]
        [SerializeField]
        private Color _fromColor;
    
        [SerializeField]
        private Color _toColor;
    
        [SerializeField]
        private Image _tweenImage;
        
        #endregion /View
    
        #region Properties
    
        public Color FromColor => _fromColor;
        public Color ToColor => _toColor;
        public Image TweenImage => _tweenImage;
    
        #endregion
    
        #region Constructor
    
        public ColorImageTween()
        {
            _fromColor = Color.white;
            _toColor = Color.black;
        }
    
        public ColorImageTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Image tweenImage,
            Color fromColor,
            Color toColor) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            _fromColor = fromColor;
            _toColor = toColor;
            _tweenImage = tweenImage;
        }
    
        #endregion
        
        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            Color startColor;
            Color toColor;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startColor = _toColor;
                toColor = _fromColor;
                animationCurve = ReverseCurve;
            }
            else
            {
                startColor = _fromColor;
                toColor = _toColor;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localColor = _tweenImage.color;
                var t = 1f;
                if (toColor.r - startColor.r != 0f)
                {
                    t = (localColor.r - startColor.r) / (toColor.r - startColor.r);
                }
                else if (toColor.g - startColor.g != 0f)
                {
                    t = (localColor.g - startColor.g) / (toColor.g - startColor.g);
                }
                else if (toColor.b - startColor.b != 0f)
                {
                    t = (localColor.b - startColor.b) / (toColor.b - startColor.b);
                }
                
                else if (toColor.a - startColor.a != 0f)
                {
                    t = (localColor.a - startColor.a) / (toColor.a - startColor.a);
                }

                time = tweenTime * t;
            }

            while (loop)
            {
                _tweenImage.color = startColor;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Color.LerpUnclamped(startColor, toColor, lerpTime);

                    _tweenImage.color = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                _tweenImage.color = toColor;
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
                        startColor = _tweenImage.color;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (_tweenImage == null) _tweenImage = TweenObject.GetComponent<Image>();
            _tweenImage.color = _fromColor;
        }

        public void SetColor(Color from, Color to)
        {
            _fromColor = from;
            _toColor = to;
        }

        #endregion /Animation
        
        #region Static

        public static ColorImageTween Clone(
            ColorImageTween tween,
            GameObject targetObject = null)
        {
            Image tweenImage = null;
            if (targetObject != null)
            {
                tweenImage = targetObject.GetComponent<Image>();
                if (tweenImage == null)
                {
                    targetObject.AddComponent<Image>();
                }
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