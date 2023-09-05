using Common.UniTaskAnimations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.UI.PopupService
{
    public class PopupCanvas : UICanvas
    {
        [SerializeField]
        private TweenComponent _raycastTween;

        public override void EnableRaycast()
        {
            if (_raycastBlock.gameObject.activeSelf) return;
            _raycastBlock.gameObject.SetActive(true);
            if (_raycastTween != null &&
                _raycastTween.Tween != null)
                _raycastTween.Tween.StartAnimation().Forget();
        }

        public override void DisableRaycast()
        {
            if (_raycastTween != null &&
                _raycastTween.Tween != null)
                DisableAsync().Forget();
            else
                _raycastBlock.gameObject.SetActive(false);
        }

        private async UniTask DisableAsync()
        {
            await _raycastTween.Tween.StartAnimation(true);
            _raycastBlock.gameObject.SetActive(false);
        }
    }
}