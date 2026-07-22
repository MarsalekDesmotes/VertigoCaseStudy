using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image ui_image_reward_value;
        [SerializeField] private TMP_Text ui_text_amount_value;

        public void Bind(WheelSliceDefinition definition, int zone)
        {
            if (definition == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            RewardDefinition reward = definition.Reward;
            ui_image_reward_value.sprite = reward == null ? null : reward.Icon;
            ui_image_reward_value.preserveAspect = true;
            RectTransform iconRect = ui_image_reward_value.rectTransform;
            iconRect.sizeDelta = definition.IsBomb ? new Vector2(118f, 108f) : new Vector2(62f, 56f);
            int amount = CalculateAmount(definition, zone);
            ui_text_amount_value.text = definition.IsBomb ? string.Empty : "x" + amount;
        }

        public static int CalculateAmount(WheelSliceDefinition definition, int zone)
        {
            if (definition == null || definition.IsBomb || definition.Reward == null)
            {
                return 0;
            }

            int progression = Mathf.Max(1, zone);
            return definition.Reward.BaseAmount * definition.AmountMultiplier * progression;
        }

        private void OnValidate()
        {
            if (ui_image_reward_value == null)
            {
                Transform value = transform.Find("ui_image_reward_value");
                if (value != null) ui_image_reward_value = value.GetComponent<Image>();
            }

            if (ui_text_amount_value == null)
            {
                Transform value = transform.Find("ui_text_amount_value");
                if (value != null) ui_text_amount_value = value.GetComponent<TMP_Text>();
            }
        }
    }
}
