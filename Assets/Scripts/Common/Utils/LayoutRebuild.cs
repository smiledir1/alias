using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Utils
{
    public class LayoutRebuild : MonoBehaviour
    {
        [SerializeField]
        private LayoutGroup _layoutGroup;

        [SerializeField]
        private int _rebuildFrames = 1;

        private void Start()
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<LayoutGroup>();
            WaitRebuild().Forget();
        }

        public async UniTask WaitRebuild()
        {
            for (var i = 0; i < _rebuildFrames; i++)
            {
                await UniTask.Yield();
            }
            
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