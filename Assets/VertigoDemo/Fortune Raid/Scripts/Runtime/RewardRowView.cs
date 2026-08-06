using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class RewardRowView : MonoBehaviour
    {
        [SerializeField] private Image ui_image_collected_row_background_value;
        [SerializeField] private Image ui_image_collected_reward_value;
        [SerializeField] private TMP_Text ui_text_collected_reward_name_value;
        [SerializeField] private TMP_Text ui_text_collected_amount_value;

        private Tween revealTween;
        private Tween amountTween;
        private RewardDefinitionModel boundDefinition;
        private int displayedAmount;

        public RewardDefinitionModel Definition { get { return boundDefinition; } }
        public RectTransform FlightTarget { get { return ui_image_collected_reward_value.rectTransform; } }

        public void Bind(CollectedRewardModel reward)
        {
            bool shouldReveal = reward != null && !gameObject.activeSelf;
            RewardDefinitionModel previousDefinition = boundDefinition;
            int previousAmount = displayedAmount;
            gameObject.SetActive(reward != null);
            if (reward == null)
            {
                boundDefinition = null;
                displayedAmount = 0;
                return;
            }

            boundDefinition = reward.Definition;
            ui_image_collected_reward_value.sprite = reward.Definition.Icon;
            ui_image_collected_reward_value.preserveAspect = true;
            ui_text_collected_reward_name_value.text = reward.Definition.DisplayName;
            ui_text_collected_reward_name_value.color = reward.Definition.IsSpecial
                ? new Color(1f, 0.80f, 0.22f)
                : new Color(0.84f, 0.88f, 0.94f);
            amountTween.Kill(false);
            int countFrom = previousDefinition == reward.Definition ? previousAmount : 0;
            displayedAmount = countFrom;
            ui_text_collected_amount_value.text = "x" + displayedAmount;
            if (displayedAmount != reward.Amount)
            {
                amountTween = DOTween.To(
                        () => displayedAmount,
                        value =>
                        {
                            displayedAmount = value;
                            ui_text_collected_amount_value.text = "x" + displayedAmount;
                        },
                        reward.Amount,
                        shouldReveal ? 0.34f : 0.26f)
                    .SetEase(Ease.OutCubic)
                    .SetUpdate(true)
                    .SetTarget(ui_text_collected_amount_value);
            }

            if (shouldReveal || previousAmount != reward.Amount)
            {
                revealTween.Kill(false);
                Color restingColor = new Color(0.13f, 0.17f, 0.24f, 0.95f);
                ui_image_collected_row_background_value.color =
                    new Color(0.48f, 0.31f, 0.08f, 1f);
                transform.localScale = shouldReveal
                    ? new Vector3(0.88f, 0.88f, 1f)
                    : new Vector3(1.025f, 1.025f, 1f);
                revealTween = DOTween.Sequence()
                    .SetUpdate(true)
                    .SetTarget(transform)
                    .Append(transform
                        .DOScale(Vector3.one, 0.22f)
                        .SetEase(Ease.OutBack))
                    .Join(DOTween.To(
                        () => ui_image_collected_row_background_value.color,
                        value => ui_image_collected_row_background_value.color = value,
                        restingColor,
                        0.36f)
                        .SetEase(Ease.OutQuad))
                    .OnComplete(() => revealTween = null);
            }
        }

        private void OnDisable()
        {
            revealTween.Kill(false);
            amountTween.Kill(false);
            revealTween = null;
            amountTween = null;
            boundDefinition = null;
            displayedAmount = 0;
            transform.localScale = Vector3.one;
        }

    }
}
