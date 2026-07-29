using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup ui_canvas_group_slice;
        [SerializeField] private RectTransform ui_transform_reward_animator;
        [SerializeField] private Image ui_image_reward_glow_value;
        [SerializeField] private Image ui_image_special_shine_value;
        [SerializeField] private Image ui_image_special_sparkle_value;
        [SerializeField] private Image ui_image_reward_value;
        [SerializeField] private TMP_Text ui_text_amount_value;

        private Sequence winGlowTween;
        private Tween focusTween;
        private Tween specialRotationTween;
        private Sequence specialPulseTween;

        public RectTransform Animator { get { return ui_transform_reward_animator; } }

        public void Bind(WheelSliceDefinition definition, int zone)
        {
            if (definition == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            SetFocus(false, false, true);
            ResetGlow();
            RewardDefinition reward = definition.Reward;
            ui_image_reward_value.sprite = reward.Icon;
            ui_image_reward_value.preserveAspect = true;
            SetSpecialEffect(reward.IsSpecial);
            RectTransform iconRect = ui_image_reward_value.rectTransform;
            iconRect.sizeDelta = Vector2.zero;
            iconRect.localScale = definition.IsBomb
                ? new Vector3(1.22f, 1.22f, 1f)
                : Vector3.one;
            int amount = CalculateAmount(definition, zone);
            ui_text_amount_value.text = definition.IsBomb ? string.Empty : "x" + amount;
        }

        public void PlayWinGlow()
        {
            RectTransform glowTransform = ui_image_reward_glow_value.rectTransform;
            winGlowTween.Kill(false);
            SetGlowAlpha(0f);
            glowTransform.localScale = new Vector3(0.72f, 0.72f, 1f);
            glowTransform.localRotation = Quaternion.identity;
            winGlowTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(ui_image_reward_glow_value)
                .Append(DOTween.To(
                    () => ui_image_reward_glow_value.color.a,
                    SetGlowAlpha,
                    0.72f,
                    0.12f))
                .Join(glowTransform
                    .DOScale(new Vector3(1.34f, 1.34f, 1f), 0.28f)
                    .SetEase(Ease.OutQuad))
                .Join(glowTransform
                    .DOLocalRotate(new Vector3(0f, 0f, 55f), 0.34f)
                    .SetEase(Ease.OutCubic))
                .Append(DOTween.To(
                    () => ui_image_reward_glow_value.color.a,
                    SetGlowAlpha,
                    0f,
                    0.30f));
        }

        public void SetFocus(bool selected, bool hasSelection, bool immediate = false)
        {
            float targetAlpha = !hasSelection || selected ? 1f : 0.38f;
            focusTween.Kill(false);
            focusTween = null;
            if (immediate)
            {
                ui_canvas_group_slice.alpha = targetAlpha;
                return;
            }

            focusTween = DOTween.To(
                    () => ui_canvas_group_slice.alpha,
                    value => ui_canvas_group_slice.alpha = value,
                    targetAlpha,
                    0.18f)
                .SetEase(Ease.OutQuad)
                .SetUpdate(true)
                .SetTarget(ui_canvas_group_slice);
        }

        private void SetSpecialEffect(bool active)
        {
            specialRotationTween.Kill(false);
            specialPulseTween.Kill(false);
            specialRotationTween = null;
            specialPulseTween = null;
            ui_image_special_shine_value.gameObject.SetActive(active);
            ui_image_special_sparkle_value.gameObject.SetActive(active);
            if (!active) return;

            RectTransform shine = ui_image_special_shine_value.rectTransform;
            RectTransform sparkle = ui_image_special_sparkle_value.rectTransform;
            shine.localRotation = Quaternion.identity;
            shine.localScale = Vector3.one;
            sparkle.localRotation = Quaternion.identity;
            sparkle.localScale = new Vector3(0.72f, 0.72f, 1f);
            SetSpecialShineAlpha(0.18f);
            SetSpecialSparkleAlpha(0.42f);

            specialRotationTween = shine
                .DOLocalRotate(new Vector3(0f, 0f, 360f), 5.2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true)
                .SetTarget(ui_image_special_shine_value);
            specialPulseTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(ui_image_special_sparkle_value)
                .Append(sparkle
                    .DOScale(new Vector3(1.12f, 1.12f, 1f), 0.72f)
                    .SetEase(Ease.InOutSine))
                .Join(DOTween.To(
                    () => ui_image_special_sparkle_value.color.a,
                    SetSpecialSparkleAlpha,
                    0.92f,
                    0.72f))
                .Join(DOTween.To(
                    () => ui_image_special_shine_value.color.a,
                    SetSpecialShineAlpha,
                    0.34f,
                    0.72f))
                .Append(sparkle
                    .DOScale(new Vector3(0.72f, 0.72f, 1f), 0.72f)
                    .SetEase(Ease.InOutSine))
                .Join(DOTween.To(
                    () => ui_image_special_sparkle_value.color.a,
                    SetSpecialSparkleAlpha,
                    0.42f,
                    0.72f))
                .Join(DOTween.To(
                    () => ui_image_special_shine_value.color.a,
                    SetSpecialShineAlpha,
                    0.18f,
                    0.72f))
                .SetLoops(-1, LoopType.Restart);
        }

        public static int CalculateAmount(WheelSliceDefinition definition, int zone)
        {
            if (definition == null || definition.IsBomb)
            {
                return 0;
            }

            if (!definition.Reward.IsStackable)
            {
                return definition.Reward.BaseAmount;
            }

            int progression = Mathf.Max(1, zone);
            return definition.Reward.BaseAmount * definition.AmountMultiplier * progression;
        }

        private void OnDisable()
        {
            winGlowTween.Kill(false);
            focusTween.Kill(false);
            specialRotationTween.Kill(false);
            specialPulseTween.Kill(false);
            winGlowTween = null;
            focusTween = null;
            specialRotationTween = null;
            specialPulseTween = null;
            ui_canvas_group_slice.alpha = 1f;
            ResetGlow();
        }

        private void ResetGlow()
        {
            RectTransform glowTransform = ui_image_reward_glow_value.rectTransform;
            SetGlowAlpha(0f);
            glowTransform.localScale = Vector3.one;
            glowTransform.localRotation = Quaternion.identity;
        }

        private void SetGlowAlpha(float alpha)
        {
            Color color = ui_image_reward_glow_value.color;
            color.a = alpha;
            ui_image_reward_glow_value.color = color;
        }

        private void SetSpecialShineAlpha(float alpha)
        {
            Color color = ui_image_special_shine_value.color;
            color.a = alpha;
            ui_image_special_shine_value.color = color;
        }

        private void SetSpecialSparkleAlpha(float alpha)
        {
            Color color = ui_image_special_sparkle_value.color;
            color.a = alpha;
            ui_image_special_sparkle_value.color = color;
        }

    }
}
