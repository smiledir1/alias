using Common.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Cheats
{
    public class CheatsLayout : MonoBehaviour
    {
        #region View

        [SerializeField]
        private Button _mainCheatsButton;
            
        [SerializeField]
        private CheatsButton _cheatsButtonTemplate;
        
        [SerializeField]
        private CheatsInput _cheatsInputTemplate;

        #endregion

        #region Cheats Layout
        
        private void Awake()
        {
            InitializeCheats();
            CreateCheats();
        }

        private void InitializeCheats()
        {
            gameObject.SetActive(false);
            _mainCheatsButton.SetClickListener(OnMainCheatsButton);
            _cheatsButtonTemplate.gameObject.SetActive(false);
            _cheatsInputTemplate.gameObject.SetActive(false);
        }

        private void OnMainCheatsButton()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void CreateCheatsButton(string labelText, UnityAction onClick)
        {
            var parent = _cheatsButtonTemplate.transform.parent;
            var newCheatsButton = Instantiate(_cheatsButtonTemplate, parent);
            newCheatsButton.Initialize(labelText, onClick);
            newCheatsButton.gameObject.SetActive(true);
        }
        
        private void CreateCheatsInput(
            string labelText, 
            string inputText,
            UnityAction<string> onClick)
        {
            var parent = _cheatsInputTemplate.transform.parent;
            var newCheatsButton = Instantiate(_cheatsInputTemplate, parent);
            newCheatsButton.Initialize(labelText, inputText, onClick);
            newCheatsButton.gameObject.SetActive(true);
        }
        
        #endregion

        #region Create Cheats

        // TODO: Придумать как вынести
        private void CreateCheats()
        {
            CreateCheatsButton("Test Button", () =>
            {
                Debug.Log("Test");
            });
            
            CreateCheatsInput("Test Button", "123Q", (text) =>
            {
                Debug.Log($"Test {text}");
            });
        }

        #endregion
    }
}
