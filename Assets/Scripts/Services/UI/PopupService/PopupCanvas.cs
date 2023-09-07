using Common.UniTaskAnimations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.UI.PopupService
{
    public class PopupCanvas : UICanvas
    {
        [SerializeField]
        private TweenComponent raycastTween;

        public override void EnableRaycast()
        {
            if (raycastBlock.gameObject.activeSelf) return;
            raycastBlock.gameObject.SetActive(true);
            if (raycastTween != null &&
                raycastTween.Tween != null)
                raycastTween.Tween.StartAnimation().Forget();
        }

        public override void DisableRaycast()
        {
            if (raycastTween != null &&
                raycastTween.Tween != null)
                DisableAsync().Forget();
            else
                raycastBlock.gameObject.SetActive(false);
        }

        private async UniTask DisableAsync()
        {
            await raycastTween.Tween.StartAnimation(true);
            raycastBlock.gameObject.SetActive(false);
        }
    }
}