using System;
using System.Collections;
using System.Collections.Generic;
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

        public bool IsSpinning { get; private set; }

        public void Bind(WheelDefinition definition, int zone)
        {
            ui_image_wheel_base_value.sprite = definition.WheelBase;
            ui_image_wheel_indicator_value.sprite = definition.Indicator;
            for (int i = 0; i < ui_wheel_slices.Count; i++)
            {
                WheelSliceDefinition slice = i < definition.Slices.Count ? definition.Slices[i] : null;
                ui_wheel_slices[i].Bind(slice, zone);
            }
        }

        public void Spin(int selectedIndex, Action onComplete)
        {
            if (!IsSpinning)
            {
                StartCoroutine(SpinRoutine(selectedIndex, onComplete));
            }
        }

        private IEnumerator SpinRoutine(int selectedIndex, Action onComplete)
        {
            IsSpinning = true;
            float sliceAngle = 360f / Mathf.Max(1, ui_wheel_slices.Count);
            float current = Normalize(ui_transform_wheel_animator.localEulerAngles.z);
            float target = current - (360f * 5f + selectedIndex * sliceAngle);
            float elapsed = 0f;

            while (elapsed < spinDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / spinDuration);
                float eased = 1f - Mathf.Pow(1f - progress, 4f);
                ui_transform_wheel_animator.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(current, target, eased));
                yield return null;
            }

            ui_transform_wheel_animator.localRotation = Quaternion.Euler(0f, 0f, Normalize(target));
            IsSpinning = false;
            if (onComplete != null) onComplete.Invoke();
        }

        private static float Normalize(float angle)
        {
            return (angle % 360f + 360f) % 360f;
        }

        private void OnValidate()
        {
            if (ui_transform_wheel_animator == null)
            {
                Transform value = transform.Find("ui_transform_wheel_animator");
                if (value != null) ui_transform_wheel_animator = value as RectTransform;
            }

            if (ui_image_wheel_base_value == null && ui_transform_wheel_animator != null)
            {
                Transform value = ui_transform_wheel_animator.Find("ui_image_wheel_base_value");
                if (value != null) ui_image_wheel_base_value = value.GetComponent<Image>();
            }

            if (ui_image_wheel_indicator_value == null)
            {
                Transform value = transform.Find("ui_image_wheel_indicator_value");
                if (value != null) ui_image_wheel_indicator_value = value.GetComponent<Image>();
            }

            if (ui_wheel_slices.Count == 0 && ui_transform_wheel_animator != null)
            {
                ui_wheel_slices.AddRange(ui_transform_wheel_animator.GetComponentsInChildren<WheelSliceView>(true));
            }
        }
    }
}
