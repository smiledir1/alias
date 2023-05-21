using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.UniTaskAnimations
{
    [Serializable]
    public abstract class SimpleTween
    {
        #region View

        [Header("Main Tween")]
        [SerializeField]
        protected GameObject _tweenObject;

        [SerializeField]
        protected float _startDelay;

        [SerializeField]
        protected float _tweenTime;

        [SerializeField]
        protected LoopType _loop;

        [SerializeField]
        protected AnimationCurve _animationCurve;

        #endregion /View

        #region Cache

        protected CancellationTokenSource CancellationTokenSource;
#if UNITY_EDITOR
        protected double PrevTime;
#endif
        protected AnimationCurve ReverseCurve;

        #endregion /Cache

        #region Abstract

        protected abstract UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default);

        public abstract void ResetValues();

        #endregion /Abstract

        #region Constructor

        protected SimpleTween()
        {
            _tweenObject = null;
            _startDelay = 0f;
            _tweenTime = 1f;
            _loop = LoopType.Once;
            _animationCurve = new AnimationCurve
            {
                keys = new[] {new Keyframe(0, 0), new Keyframe(1, 1)}
            };
        }

        protected SimpleTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve)
        {
            _tweenObject = tweenObject;
            _startDelay = startDelay;
            _tweenTime = tweenTime;
            _loop = loop;
            _animationCurve = animationCurve;
        }

        #endregion /Constructor

        #region Animation

        public bool ActiveAnimation => CancellationTokenSource != null;

        public GameObject TweenObject => _tweenObject;

        public float StartDelay => _startDelay;

        public float TweenTime => _tweenTime;

        public LoopType Loop => _loop;

        public AnimationCurve AnimationCurve => _animationCurve;

        public async UniTask StartAnimation(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            StopAnimation();
            CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var lastKeyIndex = AnimationCurve.keys.Length - 1;
            var lastKey = AnimationCurve.keys[lastKeyIndex];
            if (lastKeyIndex == -1 ||
                Math.Abs(lastKey.time - 1) > 0.01 ||
                Math.Abs(lastKey.value - 1) > 0.01)
            {
                Debug.LogError("Wrong Curve");
            }

            if (reverse && ReverseCurve == null)
            {
                ReverseCurve = new AnimationCurve();
                foreach (var k in AnimationCurve.keys)
                {
                    ReverseCurve.AddKey(1 - k.time, 1 - k.value);
                }
            }

            await DelayAnimation(CancellationTokenSource.Token);
#if UNITY_EDITOR
            PrevTime = UnityEditor.EditorApplication.timeSinceStartup;
#endif
            await Tween(reverse, startFromCurrentValue, CancellationTokenSource.Token);
            CancellationTokenSource = null;
        }

        public void StopAnimation()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource = null;
        }

        #endregion /Animation

        #region Protected

        protected async UniTask DelayAnimation(CancellationToken cancellationToken)
        {
            if (StartDelay > 0)
                await UniTask.Delay(TimeSpan.FromSeconds(StartDelay), cancellationToken: cancellationToken);
        }

        protected float GetDeltaTime()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var timeSinceStartup = UnityEditor.EditorApplication.timeSinceStartup;
                var deltaTime = timeSinceStartup - PrevTime;
                PrevTime = timeSinceStartup;
                return (float) deltaTime;
            }
#endif
            return Time.deltaTime;
        }

        #endregion /Protected

        #region Static

        public static SimpleTween Clone(SimpleTween tween, GameObject targetObject = null)
        {
            SimpleTween newTween = null;
            switch (tween)
            {
                case PositionTween positionTween:
                    newTween = new PositionTween(
                        targetObject,
                        positionTween.StartDelay,
                        positionTween.TweenTime,
                        positionTween.Loop,
                        positionTween.AnimationCurve,
                        positionTween.FromPosition,
                        positionTween.ToPosition);
                    break;

                case RotationTween rotationTween:
                    newTween = new RotationTween(
                        targetObject,
                        rotationTween.StartDelay,
                        rotationTween.TweenTime,
                        rotationTween.Loop,
                        rotationTween.AnimationCurve,
                        rotationTween.FromRotation,
                        rotationTween.ToRotation);
                    break;

                case ScaleTween scaleTween:
                    newTween = new ScaleTween(
                        targetObject,
                        scaleTween.StartDelay,
                        scaleTween.TweenTime,
                        scaleTween.Loop,
                        scaleTween.AnimationCurve,
                        scaleTween.FromScale,
                        scaleTween.ToScale);
                    break;

                case TransparencyTween transparencyTween:
                    CanvasGroup canvasGroup = null;
                    if (targetObject != null)
                    {
                        canvasGroup = targetObject.GetComponent<CanvasGroup>();
                        if (canvasGroup == null)
                        {
                            targetObject.AddComponent<CanvasGroup>();
                        }
                    }

                    newTween = new TransparencyTween(
                        targetObject,
                        transparencyTween.StartDelay,
                        transparencyTween.TweenTime,
                        transparencyTween.Loop,
                        transparencyTween.AnimationCurve,
                        canvasGroup,
                        transparencyTween.FromOpacity,
                        transparencyTween.ToOpacity);
                    break;
            }

            return newTween;
        }

        #endregion /Static
    }

    public enum LoopType
    {
        Once,
        Loop,
        PingPong
    }
}