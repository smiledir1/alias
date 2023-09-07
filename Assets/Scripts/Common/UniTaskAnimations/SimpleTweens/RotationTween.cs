using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class RotationTween : SimpleTween
    {
        #region View

        [SerializeField]
        private Vector3 fromRotation;

        [SerializeField]
        private Vector3 toRotation;

        #endregion /View

        #region Properties

        public Vector3 FromRotation => fromRotation;
        public Vector3 ToRotation => toRotation;

        #endregion

        #region Constructor

        public RotationTween()
        {
            fromRotation = Vector3.zero;
            toRotation = Vector3.zero;
        }

        public RotationTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Vector3 fromRotation,
            Vector3 toRotation) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            this.fromRotation = fromRotation;
            this.toRotation = toRotation;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (TweenObject == null) return;

            Vector3 startRotation;
            Vector3 toRotation;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startRotation = this.toRotation;
                toRotation = fromRotation;
                animationCurve = ReverseCurve;
            }
            else
            {
                startRotation = fromRotation;
                toRotation = this.toRotation;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var localRotation = TweenObject.transform.eulerAngles;
                var t = 1f;
                if (toRotation.x - startRotation.x != 0f)
                    t = (localRotation.x - startRotation.x) / (toRotation.x - startRotation.x);
                else if (toRotation.y - startRotation.y != 0f)
                    t = (localRotation.y - startRotation.y) / (toRotation.y - startRotation.y);
                else if (toRotation.z - startRotation.z != 0f)
                    t = (localRotation.z - startRotation.z) / (toRotation.z - startRotation.z);

                time = tweenTime * t;
            }

            while (loop)
            {
                TweenObject.transform.eulerAngles = startRotation;

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Vector3.LerpUnclamped(startRotation, toRotation, lerpTime);

                    TweenObject.transform.eulerAngles = lerpValue;
                    await UniTask.Yield(cancellationToken);
                }

                TweenObject.transform.eulerAngles = toRotation;
                time -= TweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        toRotation = startRotation;
                        startRotation = TweenObject.transform.eulerAngles;
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            TweenObject.transform.eulerAngles = fromRotation;
        }

        public void SetRotation(Vector3 from, Vector3 to)
        {
            fromRotation = from;
            toRotation = to;
        }

        #endregion /Animation

        #region Static

        public static RotationTween Clone(
            RotationTween tween,
            GameObject targetObject = null)
        {
            var animationCurve = new AnimationCurve();
            animationCurve.CopyFrom(tween.AnimationCurve);

            return new RotationTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tween.FromRotation,
                tween.ToRotation);
        }

        #endregion
    }
}