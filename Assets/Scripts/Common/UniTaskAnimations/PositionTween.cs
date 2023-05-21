using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public class PositionTween : SimpleTween
    {
        #region View

        [Header("Current Tween")]
        [SerializeField]
        private Vector3 _fromPosition;

        [SerializeField]
        private Vector3 _toPosition;

        #endregion /View

        #region Properties

        public Vector3 FromPosition => _fromPosition;
        public Vector3 ToPosition => _toPosition;

        #endregion
        
        #region Constructor
        
        public PositionTween() 
        {
            _fromPosition = Vector3.zero;
            _toPosition = Vector3.zero;
        }
        
        public PositionTween(
            GameObject tweenObject, 
            float startDelay, 
            float tweenTime, 
            LoopType loop, 
            AnimationCurve animationCurve, 
            Vector3 fromPosition,
            Vector3 toPosition) :
            base(tweenObject, 
            startDelay, 
            tweenTime, 
            loop, 
            animationCurve)
        {
            _fromPosition = fromPosition;
            _toPosition = toPosition;
        }

        #endregion /Constructor
        
        #region Animation
        
        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            Vector3 startPosition;
            Vector3 toPosition;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;
            
            if (reverse)
            {
                startPosition = _toPosition;
                toPosition = _fromPosition;
                animationCurve = ReverseCurve;
            }
            else
            {
                startPosition = _fromPosition;
                toPosition = _toPosition;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localPosition = TweenObject.transform.localPosition;
                startPosition = localPosition;
                var currentValue = localPosition.x;
                var currentPartValue = Mathf.Abs(toPosition.x - currentValue);
                var maxValue = Mathf.Abs(startPosition.x - toPosition.x);
                var normalizePart = currentPartValue / maxValue;
                tweenTime *= normalizePart;
            }

            
            while (loop)
            {
                TweenObject.transform.localPosition = startPosition;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve.Evaluate(normalizeTime);
                    var lerpValue = Vector3.LerpUnclamped(startPosition, toPosition, lerpTime);

                    TweenObject.transform.localPosition = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                TweenObject.transform.localPosition = toPosition;
                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        toPosition = startPosition;
                        startPosition = TweenObject.transform.localPosition;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            TweenObject.transform.localPosition = _fromPosition;
        }

        public void SetPosition(Vector3 from, Vector3 to)
        {
            _fromPosition = from;
            _toPosition = to;
        }
        
        #endregion /Animation
    }
}
