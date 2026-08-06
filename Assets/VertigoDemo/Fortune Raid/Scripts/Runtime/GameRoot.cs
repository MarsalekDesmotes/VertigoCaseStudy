using UnityEngine;

namespace VertigoDemo
{
    public sealed class GameRoot : MonoBehaviour
    {
        [SerializeField] private WheelCatalogModel wheelCatalog;
        [SerializeField] private GameScreenView gameScreenView;
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private Sprite collectedChestIcon;
        [Header("Revive")]
        [SerializeField] private RewardDefinitionModel reviveCurrencyDefinition;
        [SerializeField, Min(1)] private int reviveCost = 25;
        [SerializeField, Min(0)] private int startingReviveCurrency = 100;

        private GamePresenter presenter;

        private void Awake()
        {
            ILocalization localization = StringTableLocalization.CreateDefaultEnglish();
            IZoneRules zoneRules = new ZoneRules();
            IRewardRules rewardRules = new RewardRules();

            gameScreenView.Configure(localization, zoneRules, rewardRules);

            RunStateModel runState = new RunStateModel(
                reviveCurrencyDefinition,
                startingReviveCurrency);
            presenter = new GamePresenter(
                gameScreenView,
                gameScreenView,
                wheelCatalog,
                runState,
                zoneRules,
                rewardRules,
                reviveCurrencyDefinition,
                bombIcon,
                collectedChestIcon,
                reviveCost);
            presenter.Start();
        }

        private void OnDestroy()
        {
            presenter.Dispose();
        }
    }
}
