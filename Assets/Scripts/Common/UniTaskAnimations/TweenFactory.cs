using System.Collections.Generic;
using Common.UniTaskAnimations.SimpleTweens;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UniTaskAnimations
{
    public static class TweenFactory
    {
        #region Example Values

        private const float StartDelay = 0f;
        private const float TweenTime = 1f;
        private const LoopType Loop = LoopType.Once;

        private static readonly AnimationCurve AnimationCurve = new()
        {
            keys = new[] {new Keyframe(0, 0), new Keyframe(1, 1)}
        };
        
        private static readonly AnimationCurve LinearAnimationCurve = new()
        {
            keys = new[] {
                new Keyframe(0, 0, 1, 1), 
                new Keyframe(1, 1, 1, 1)}
        };

        #endregion /Example Values

        #region Main

        public static SimpleTween CreateSimpleTween(System.Type type, GameObject tweenObject = null)
        {
            return CreateSimpleTween(type.Name, tweenObject);
        }

        public static SimpleTween CreateSimpleTween(string typeName, GameObject tweenObject = null)
        {
            return typeName switch
            {
                nameof(PositionTween) => CreatePositionTween(tweenObject),
                nameof(RotationTween) => CreateRotationTween(tweenObject),
                nameof(ScaleTween) => CreateScaleTween(tweenObject),
                nameof(TransparencyCanvasGroupTween) => CreateTransparencyCanvasGroupTween(tweenObject),
                nameof(BezierPositionTween) => CreateBezierPositionTween(tweenObject),
                nameof(FillImageTween) => CreateFillImageTween(tweenObject),
                nameof(ColorImageTween) => CreateColorImageTween(tweenObject),
                nameof(FrameByFrameTween) => CreateFrameByFrameTween(tweenObject),
                nameof(OffsetPositionTween) => CreateOffsetPositionTween(tweenObject),
                _ => null
            };
        }

        #endregion

        #region Create Tweens

        public static SimpleTween CreatePositionTween(GameObject tweenObject = null)
        {
            return new PositionTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                PositionType.Local,
                Vector3.zero,
                Vector3.zero);
        }

        public static SimpleTween CreateRotationTween(GameObject tweenObject = null)
        {
            return new RotationTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                Vector3.zero,
                Vector3.zero);
        }

        public static SimpleTween CreateScaleTween(GameObject tweenObject = null)
        {
            return new ScaleTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                Vector3.zero,
                Vector3.one);
        }

        public static SimpleTween CreateTransparencyCanvasGroupTween(GameObject tweenObject = null)
        {
            var canvasGroup = tweenObject == null ? null : tweenObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null && tweenObject != null)
            {
                canvasGroup = tweenObject.gameObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 1f;
            }

            return new TransparencyCanvasGroupTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                canvasGroup,
                0,
                1);
        }

        public static SimpleTween CreateBezierPositionTween(GameObject tweenObject = null)
        {
            return new BezierPositionTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                PositionType.Local,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                Vector3.zero,
                0.05f);
        }

        public static SimpleTween CreateFillImageTween(GameObject tweenObject = null)
        {
            var image = tweenObject == null ? null : tweenObject.GetComponent<Image>();
            if (image == null && tweenObject != null)
            {
                image = tweenObject.gameObject.AddComponent<Image>();
                image.fillAmount = 1f;
            }

            return new FillImageTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                image,
                0,
                1);
        }

        public static SimpleTween CreateColorImageTween(GameObject tweenObject = null)
        {
            var image = tweenObject == null ? null : tweenObject.GetComponent<Graphic>();
            if (image == null && tweenObject != null)
            {
                image = tweenObject.gameObject.AddComponent<Image>();
                image.color = Color.white;
            }

            return new ColorImageTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                image,
                Color.white,
                Color.black);
        }

        public static SimpleTween CreateFrameByFrameTween(GameObject tweenObject = null)
        {
            var image = tweenObject == null ? null : tweenObject.GetComponent<Image>();
            if (image == null && tweenObject != null)
            {
                image = tweenObject.gameObject.AddComponent<Image>();
                image.color = Color.white;
            }

            var sprites = new List<Sprite>();
            return new FrameByFrameTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                LinearAnimationCurve,
                image,
                sprites);
        }
        
        public static SimpleTween CreateOffsetPositionTween(GameObject tweenObject = null)
        {
            return new OffsetPositionTween(
                tweenObject,
                StartDelay,
                TweenTime,
                Loop,
                AnimationCurve,
                Vector3.zero,
                Vector3.zero);
        }

        #endregion
    }
}