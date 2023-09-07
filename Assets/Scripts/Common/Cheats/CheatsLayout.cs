using Common.Extensions;
using Services.Helper;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if YANDEX_PLATFORM
using Services.YandexGames;
#endif

namespace Common.Cheats
{
    public class CheatsLayout : MonoBehaviour
    {
        #region View

        [SerializeField]
        private Button mainCheatsButton;

        [Header("Cheats Elements")]
        [SerializeField]
        private CheatsButton cheatsButtonTemplate;

        [SerializeField]
        private CheatsInput cheatsInputTemplate;

        [SerializeField]
        private CheatsLabel cheatsLabelTemplate;

        #endregion

        #region Services

#if YANDEX_PLATFORM
        [Service]
        private static IYandexGamesService _yandexGamesService;
#endif

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
            mainCheatsButton.SetClickListener(OnMainCheatsButton);
            cheatsButtonTemplate.gameObject.SetActive(false);
            cheatsInputTemplate.gameObject.SetActive(false);
        }

        private void OnMainCheatsButton()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void CreateCheatsButton(string labelText, UnityAction onClick)
        {
            var parent = cheatsButtonTemplate.transform.parent;
            var newCheatsButton = Instantiate(cheatsButtonTemplate, parent);
            newCheatsButton.Initialize(labelText, onClick);
            newCheatsButton.gameObject.SetActive(true);
        }

        private void CreateCheatsInput(
            string labelText,
            string inputText,
            UnityAction<string> onClick)
        {
            var parent = cheatsInputTemplate.transform.parent;
            var newCheatsButton = Instantiate(cheatsInputTemplate, parent);
            newCheatsButton.Initialize(labelText, inputText, onClick);
            newCheatsButton.gameObject.SetActive(true);
        }

        private void CreateCheatsLabel(string labelText)
        {
            var parent = cheatsInputTemplate.transform.parent;
            var newCheatsButton = Instantiate(cheatsLabelTemplate, parent);
            newCheatsButton.Initialize(labelText);
            newCheatsButton.gameObject.SetActive(true);
        }

        #endregion

        #region Create Cheats

        // TODO: Придумать как вынести
        private void CreateCheats()
        {
            CreateCheatsLabel("Test Block");
            CreateCheatsButton("Test Button", () => { Debug.Log("Test"); });

            CreateCheatsInput("Test Button", "123Q", (text) => { Debug.Log($"Test {text}"); });

#if YANDEX_PLATFORM
            CreateCheatsLabel("Yandex Block");
            CreateCheatsButton("GetEnvironment", async () =>
            {
                var environment = await _yandexGamesService.GetEnvironment();
                Debug.Log(environment.ToString());
            });

            CreateCheatsButton("InitializePlayer", async () =>
            {
                var playerData = await _yandexGamesService.InitializePlayer(PlayerPhotoSize.Small);
                Debug.Log(playerData.ToString());
            });

            CreateCheatsButton("ShowReview", async () =>
            {
                var reviewReason = await _yandexGamesService.ShowReview();
                Debug.Log(reviewReason.ToString());
            });

            CreateCheatsButton("ShowFullscreenAd", async () =>
            {
                await _yandexGamesService.ShowFullscreenAd();
                Debug.Log("ShowFullscreenAd showed");
            });

            CreateCheatsButton("ShowRewardedVideoAd", async () =>
            {
                await _yandexGamesService.ShowRewardedVideoAd(OnReward);
                Debug.Log("ShowRewardedVideoAd showed");

                void OnReward()
                {
                    Debug.Log("Get Reward");
                }
            });
#endif

            CreateCheatsLabel("UserData");
        }

        #endregion
    }
}