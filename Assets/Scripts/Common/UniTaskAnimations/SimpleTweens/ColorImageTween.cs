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
        private Color _fromColor;
    
        [SerializeField]
        private Color _toColor;
    
        [SerializeField]
        private Graphic _tweenGraphic;
        
        #endregion /View
    
        #region Properties
    
        public Color FromColor => _fromColor;
        public Color ToColor => _toColor;
        public Graphic TweenGraphic => _tweenGraphic;
    
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
            Graphic tweenGraphic,
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
            _tweenGraphic = tweenGraphic;
        }
    
        #endregion
        
        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (_tweenGraphic == null)
            {
                _tweenGraphic = _tweenObject.GetComponent<Graphic>();
                if (_tweenGraphic == null) return;
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
                var localColor = _tweenGraphic.color;
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
                _tweenGraphic.color = startColor;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Color.LerpUnclamped(startColor, toColor, lerpTime);

                    _tweenGraphic.color = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                _tweenGraphic.color = toColor;
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
                        startColor = _tweenGraphic.color;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (_tweenGraphic == null) _tweenGraphic = TweenObject.GetComponent<Graphic>();
            _tweenGraphic.color = _fromColor;
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
            Graphic tweenImage = null;
            if (targetObject != null)
            {
                tweenImage = targetObject.GetComponent<Graphic>();
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