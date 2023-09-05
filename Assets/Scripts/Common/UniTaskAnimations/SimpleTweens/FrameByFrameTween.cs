using System;
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
        private Image _tweenImage;

        [SerializeField]
        private List<Sprite> _sprites;

        #endregion /View

        #region Properties

        public Image TweenImage => _tweenImage;
        public List<Sprite> Sprites => _sprites;

        #endregion

        #region Constructor

        public FrameByFrameTween()
        {
            _sprites = new List<Sprite>();
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
            _tweenImage = tweenImage;
            _sprites = sprites;
        }

        #endregion /Constructor

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

            if (_sprites.Count == 0) return;

            int startSprite;
            int toSprite;
            AnimationCurve animationCurve;
            var tweenTime = TweenTime;
            if (Loop == LoopType.PingPong) tweenTime /= 2;
            var time = 0f;
            var loop = true;

            if (reverse)
            {
                startSprite = _sprites.Count - 1;
                toSprite = 0;
                animationCurve = ReverseCurve;
            }
            else
            {
                startSprite = 0;
                toSprite = _sprites.Count - 1;
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
                _tweenImage.sprite = _sprites[startSprite];

                while (time < tweenTime)
                {
                    time += GetDeltaTime();

                    var normalizeTime = time / tweenTime;
                    var lerpTime = animationCurve?.Evaluate(normalizeTime) ?? normalizeTime;
                    var lerpValue = Mathf.LerpUnclamped(startSprite, toSprite, lerpTime);

                    if (_tweenImage == null) return;
                    var currentSpritePos = (int) (toSprite > startSprite
                        ? Mathf.Ceil(lerpValue)
                        : Mathf.Floor(lerpValue));
                    _tweenImage.sprite = _sprites[currentSpritePos];
                    await UniTask.Yield(cancellationToken);
                }

                _tweenImage.sprite = _sprites[toSprite];

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
                        toSprite = startSprite;
                        startSprite = GetImageSpritePosition();
                        break;
                }
            }
        }

        public override void ResetValues()
        {
            if (_sprites == null || _sprites.Count == 0) return;
            _tweenImage.sprite = _sprites[0];
        }

        public void SetSprites(List<Sprite> sprites)
        {
            _sprites = sprites;
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
            if (_tweenImage == null || _tweenImage.sprite == null) return 0;
            var imageSprite = _tweenImage.sprite;
            for (var i = 0; i < _sprites.Count; i++)
            {
                var sprite = _sprites[i];
                if (sprite != imageSprite) continue;
                return i;
            }

            return 0;
        }

        #endregion
    }
}