using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class WheelView : MonoBehaviour
    {
        [SerializeField] private RectTransform ui_transform_wheel_animator;
        [SerializeField] private Image ui_image_wheel_base_value;
        [SerializeField] private Image ui_image_wheel_indicator_value;
        [SerializeField] private List<WheelSliceView> ui_wheel_slices = new List<WheelSliceView>();
        [SerializeField, Min(1f)] private float spinDuration = 3.2f;

        private Tween spinTween;
        private Tween selectedSliceTween;
        private Tween bombImpactTween;
        private Sequence spinSequence;
        private Sequence zoneRevealTween;
        private Action pendingComplete;
        private int boundSliceCount;

        public bool IsSpinning { get; private set; }

        public void Bind(
            WheelDefinitionModel definition,
            int zone,
            IRewardRules rewardRules)
        {
            if (!definition.ValidateBombs())
            {
                Debug.LogError(
                    "WheelDefinitionModel bomb invariant failed: " + definition.name,
                    definition);
            }

            if (ui_wheel_slices.Count != definition.Slices.Count)
            {
                Debug.LogError(
                    "Wheel prefab slice count (" + ui_wheel_slices.Count +
                    ") must match WheelDefinitionModel.Slices (" +
                    definition.Slices.Count + ").",
                    this);
                boundSliceCount = 0;
                return;
            }

            boundSliceCount = definition.Slices.Count;
            ui_image_wheel_base_value.sprite = definition.WheelBase;
            ui_image_wheel_indicator_value.sprite = definition.Indicator;
            for (int i = 0; i < boundSliceCount; i++)
            {
                ui_wheel_slices[i].Bind(definition.Slices[i], zone, rewardRules);
            }

            KeepSliceContentsUpright();
        }

        public void PlayZoneReveal(bool golden)
        {
            RectTransform wheelRoot = (RectTransform)transform;

            zoneRevealTween.Kill(false);
            zoneRevealTween = null;
            wheelRoot.localScale = golden
                ? new Vector3(0.84f, 0.84f, 1f)
                : new Vector3(0.94f, 0.94f, 1f);
            wheelRoot.localRotation = golden
                ? Quaternion.Euler(0f, 0f, -2.2f)
                : Quaternion.identity;
            zoneRevealTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(wheelRoot)
                .Append(wheelRoot
                    .DOScale(
                        golden
                            ? new Vector3(1.055f, 1.055f, 1f)
                            : new Vector3(1.02f, 1.02f, 1f),
                        golden ? 0.34f : 0.20f)
                    .SetEase(Ease.OutBack))
                .Join(wheelRoot
                    .DOLocalRotate(Vector3.zero, golden ? 0.30f : 0.18f)
                    .SetEase(Ease.OutCubic))
                .Append(wheelRoot
                    .DOScale(Vector3.one, golden ? 0.16f : 0.10f)
                    .SetEase(Ease.OutQuad))
                .OnComplete(() => zoneRevealTween = null);
        }

        public void Spin(int selectedIndex, Action onComplete)
        {
            if (IsSpinning)
            {
                return;
            }

            if (boundSliceCount <= 0 ||
                selectedIndex < 0 ||
                selectedIndex >= boundSliceCount)
            {
                Debug.LogError(
                    "Spin aborted: invalid slice index " + selectedIndex +
                    " for bound count " + boundSliceCount + ".",
                    this);
                onComplete?.Invoke();
                return;
            }

            IsSpinning = true;
            pendingComplete = onComplete;
            ClearSliceFocus();
            float sliceAngle = 360f / boundSliceCount;
            float current = Normalize(ui_transform_wheel_animator.localEulerAngles.z);
            float selectedRotation = Normalize(selectedIndex * sliceAngle);
            float clockwiseAlignment = Mathf.Repeat(current - selectedRotation, 360f);
            float target = current - (360f * 5f + clockwiseAlignment);

            spinTween.Kill(false);
            spinSequence.Kill(false);
            Tween rotationTween = ui_transform_wheel_animator
                .DOLocalRotate(new Vector3(0f, 0f, target), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart)
                .SetUpdate(true)
                .SetTarget(ui_transform_wheel_animator)
                .OnUpdate(KeepSliceContentsUpright);
            spinTween = rotationTween;
            spinSequence = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(ui_transform_wheel_animator)
                .Append(ui_transform_wheel_animator
                    .DOLocalRotate(new Vector3(0f, 0f, current + 7f), 0.12f, RotateMode.Fast)
                    .SetEase(Ease.OutQuad)
                    .OnUpdate(KeepSliceContentsUpright))
                .Append(rotationTween)
                .OnComplete(() =>
                {
                    ui_transform_wheel_animator.localRotation =
                        Quaternion.Euler(0f, 0f, Normalize(target));
                    KeepSliceContentsUpright();
                    spinTween = null;
                    spinSequence = null;
                    PulseSelectedSlice(selectedIndex, CompleteSpin);
                });
        }

        private void KeepSliceContentsUpright()
        {
            float counterRotation = -ui_transform_wheel_animator.localEulerAngles.z;
            Quaternion uprightRotation = Quaternion.Euler(0f, 0f, counterRotation);
            for (int i = 0; i < boundSliceCount; i++)
            {
                ui_wheel_slices[i].Animator.localRotation = uprightRotation;
            }
        }

        private void PulseSelectedSlice(int selectedIndex, Action onComplete)
        {
            Debug.Assert(
                selectedIndex >= 0 && selectedIndex < boundSliceCount,
                "PulseSelectedSlice index out of bound slice range.");
            for (int i = 0; i < boundSliceCount; i++)
            {
                ui_wheel_slices[i].SetFocus(i == selectedIndex, true);
            }

            WheelSliceView selectedSlice = ui_wheel_slices[selectedIndex];
            selectedSlice.PlayWinGlow();
            RectTransform selected = selectedSlice.Animator;
            selectedSliceTween.Kill(false);
            selected.localScale = Vector3.one;
            selectedSliceTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(selected)
                .Append(selected.DOScale(new Vector3(1.18f, 1.18f, 1f), 0.12f).SetEase(Ease.OutBack))
                .AppendInterval(0.08f)
                .Append(selected.DOScale(Vector3.one, 0.16f).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    selectedSliceTween = null;
                    onComplete();
                });
        }

        private void ClearSliceFocus()
        {
            for (int i = 0; i < boundSliceCount; i++)
            {
                ui_wheel_slices[i].SetFocus(false, false);
            }
        }

        public void PlayBombImpact()
        {
            RectTransform wheelRoot = (RectTransform)transform;

            bombImpactTween.Kill(false);
            Vector2 restingPosition = wheelRoot.anchoredPosition;
            wheelRoot.anchoredPosition = restingPosition;
            bombImpactTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(wheelRoot)
                .Append(TweenAnchoredPosition(
                    wheelRoot, restingPosition + new Vector2(18f, 0f), 0.035f))
                .Append(TweenAnchoredPosition(
                    wheelRoot, restingPosition + new Vector2(-14f, 0f), 0.040f))
                .Append(TweenAnchoredPosition(
                    wheelRoot, restingPosition + new Vector2(10f, 0f), 0.040f))
                .Append(TweenAnchoredPosition(
                    wheelRoot, restingPosition + new Vector2(-6f, 0f), 0.040f))
                .Append(TweenAnchoredPosition(
                    wheelRoot, restingPosition + new Vector2(3f, 0f), 0.030f))
                .Append(TweenAnchoredPosition(wheelRoot, restingPosition, 0.035f))
                .OnComplete(() =>
                {
                    wheelRoot.anchoredPosition = restingPosition;
                    bombImpactTween = null;
                });
        }

        private static Tween TweenAnchoredPosition(
            RectTransform target,
            Vector2 endValue,
            float duration)
        {
            return DOTween.To(
                    () => target.anchoredPosition,
                    value => target.anchoredPosition = value,
                    endValue,
                    duration)
                .SetEase(Ease.InOutSine);
        }

        private static float Normalize(float angle)
        {
            return (angle % 360f + 360f) % 360f;
        }

        private void CompleteSpin()
        {
            IsSpinning = false;
            Action callback = pendingComplete;
            pendingComplete = null;
            callback();
        }

        private void OnDisable()
        {
            spinTween.Kill(false);
            spinSequence.Kill(false);
            selectedSliceTween.Kill(false);
            bombImpactTween.Kill(false);
            zoneRevealTween.Kill(false);
            spinTween = null;
            spinSequence = null;
            selectedSliceTween = null;
            bombImpactTween = null;
            zoneRevealTween = null;
            if (IsSpinning)
            {
                CompleteSpin();
            }
        }

    }
}
