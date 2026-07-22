using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class RewardRowView : MonoBehaviour
    {
        [SerializeField] private Image ui_image_collected_reward_value;
        [SerializeField] private TMP_Text ui_text_collected_amount_value;

        public void Bind(CollectedReward reward)
        {
            gameObject.SetActive(reward != null);
            if (reward == null) return;
            ui_image_collected_reward_value.sprite = reward.Definition.Icon;
            ui_image_collected_reward_value.preserveAspect = true;
            ui_text_collected_amount_value.text = "x" + reward.Amount;
        }

        private void OnValidate()
        {
            if (ui_image_collected_reward_value == null)
            {
                Transform value = transform.Find("ui_image_collected_reward_value");
                if (value != null) ui_image_collected_reward_value = value.GetComponent<Image>();
            }

            if (ui_text_collected_amount_value == null)
            {
                Transform value = transform.Find("ui_text_collected_amount_value");
                if (value != null) ui_text_collected_amount_value = value.GetComponent<TMP_Text>();
            }
        }
    }
}
