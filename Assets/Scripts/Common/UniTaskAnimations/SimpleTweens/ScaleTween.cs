using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class ScaleTween : SimpleTween
    {
        #region View

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
            if (TweenObject == null) return;
            
            Vector3 startScale;
            Vector3 toScale;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startScale = _toScale;
                toScale = _fromScale;
                animationCurve = ReverseCurve;
            }
            else
            {
                startScale = _fromScale;
                toScale = _toScale;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localScale = TweenObject.transform.localScale;
                var t = 1f;
                if (toScale.x - startScale.x != 0f)
                {
                    t = (localScale.x - startScale.x) / (toScale.x - startScale.x);
                }
                else if (toScale.y - startScale.y != 0f)
                {
                    t = (localScale.y - startScale.y) / (toScale.y - startScale.y);
                }
                else if (toScale.z - startScale.z != 0f)
                {
                    t = (localScale.z - startScale.z) / (toScale.z - startScale.z);
                }

                time = tweenTime * t;
            }

            while (loop)
            {
                TweenObject.transform.localScale = startScale;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
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
        
        #region Static

        public static ScaleTween Clone(
            ScaleTween tween,
            GameObject targetObject = null)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);
                    
            return new ScaleTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tween.FromScale,
                tween.ToScale);
        }

        #endregion
    }
}