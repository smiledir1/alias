﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UniTaskAnimations.SimpleTweens
{
    [Serializable]
    public class FrameByFrameTween : SimpleTween
    {
        #region View

        [SerializeField]
        private Image tweenImage;

        [SerializeField]
        private List<Sprite> sprites;

        #endregion /View

        #region Properties

        public Image TweenImage => tweenImage;
        public List<Sprite> Sprites => sprites;

        #endregion

        #region Constructor

        public FrameByFrameTween()
        {
            sprites = new List<Sprite>();
        }

        public FrameByFrameTween(
            GameObject tweenObject,
            float startDelay,
            float tweenTime,
            LoopType loop,
            AnimationCurve animationCurve,
            Image tweenImage,
            List<Sprite> sprites) :
            base(tweenObject,
                startDelay,
                tweenTime,
                loop,
                animationCurve)
        {
            this.tweenImage = tweenImage;
            this.sprites = sprites;
        }

        #endregion /Constructor

        #region Animation

        protected override async UniTask Tween(
            bool reverse = false,
            bool startFromCurrentValue = false,
            CancellationToken cancellationToken = default)
        {
            if (tweenImage == null)
            {
                tweenImage = tweenObject.GetComponent<Image>();
                if (tweenImage == null) return;
            }

            if (sprites.Count == 0) return;

            int startSprite;
            int toSprite;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startSprite = sprites.Count - 1;
                toSprite = 0;
                animationCurve = ReverseCurve;
            }
            else
            {
                startSprite = 0;
                toSprite = sprites.Count - 1;
                animationCurve = AnimationCurve;
            }

            if (startFromCurrentValue)
            {
                var currentPosition = GetImageSpritePosition();
                var t = (currentPosition - startSprite) / (toSprite - startSprite);
                time = tweenTime * t;
            }

            while (loop)
            {
                tweenImage.sprite = sprites[startSprite];

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Mathf.LerpUnclamped(startSprite, toSprite, lerpTime);

                    if (tweenImage == null) return;
                    var currentSpritePos = (int) (toSprite > startSprite
                        ? Mathf.Ceil(lerpValue)
                        : Mathf.Floor(lerpValue));
                    tweenImage.sprite = sprites[currentSpritePos];
                    await UniTask.Yield(cancellationToken);
                }

                tweenImage.sprite = sprites[toSprite];

                time -= tweenTime;

                switch (Loop)
                {
                    case LoopType.Once:
                        loop = false;
                        break;

                    case LoopType.Loop:
                        break;

                    case LoopType.PingPong:
                        if (tweenImage == null) return;
                        toSprite = startSprite;
                        startSprite = GetImageSpritePosition();
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (sprites == null || sprites.Count == 0) return;
            tweenImage.sprite = sprites[0];
        }

        public void SetSprites(List<Sprite> sprites)
        {
            this.sprites = sprites;
        }

        #endregion /Animation

        #region Static

        public static FrameByFrameTween Clone(
            FrameByFrameTween tween,
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

            return new FrameByFrameTween(
                targetObject,
                tween.StartDelay,
                tween.TweenTime,
                tween.Loop,
                animationCurve,
                tweenImage,
                tween.Sprites);
        }

        #endregion

        #region Private

        private int GetImageSpritePosition()
        {
            if (tweenImage == null || tweenImage.sprite == null) return 0;
            var imageSprite = tweenImage.sprite;
            for (var i = 0; i < sprites.Count; i++)
            {
                var sprite = sprites[i];
                if (sprite != imageSprite) continue;
                return i;
            }

            return 0;
        }

        #endregion
    }
}