using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Utils
{
    public class LayoutRebuild : MonoBehaviour
    {
        [SerializeField]
        private LayoutGroup layoutGroup;

        [SerializeField]
        private int rebuildFrames = 1;

        private void Start()
        {
            if (layoutGroup == null) layoutGroup = GetComponent<LayoutGroup>();
            WaitRebuild().Forget();
        }

        public async UniTask WaitRebuild()
        {
            for (var i = 0; i < rebuildFrames; i++)
            {
                await UniTask.Yield();
            }

            Rebuild();
        }

        public void Rebuild()
        {
            layoutGroup.SetLayoutVertical();
            layoutGroup.SetLayoutHorizontal();
        }

        private void OnValidate()
        {
            if (layoutGroup == null) layoutGroup = GetComponent<LayoutGroup>();
        }
    }
}