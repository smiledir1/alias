using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Utils
{
    public class LayoutRebuild : MonoBehaviour
    {
        [SerializeField]
        private LayoutGroup _layoutGroup;

        private void Start()
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<LayoutGroup>();
            WaitRebuild().Forget();
        }

        public async UniTask WaitRebuild()
        {
            await UniTask.Yield();
            Rebuild();
        }

        public void Rebuild()
        {
            _layoutGroup.SetLayoutVertical();
            _layoutGroup.SetLayoutHorizontal();
        }

        private void OnValidate()
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<LayoutGroup>();
        }
    }
}