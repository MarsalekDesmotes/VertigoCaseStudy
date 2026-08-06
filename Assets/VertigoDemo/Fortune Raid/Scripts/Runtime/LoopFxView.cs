using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VertigoDemo
{
    // Reusable loop VFX: rotate + alpha pulse
    public sealed class LoopFxView : MonoBehaviour
    {
        [FormerlySerializedAs("ui_transform_sunray_animator")]
        [SerializeField] private RectTransform ui_transform_rotate_value;
        [FormerlySerializedAs("ui_image_sunray_value")]
        [SerializeField] private Image ui_image_pulse_value;
        [SerializeField, Min(0f)] private float rotationSpeedDegrees = 12f;
        [SerializeField, Min(0f)] private float pulseCyclesPerSecond = 0.55f;
        [SerializeField, Range(0f, 1f)] private float minimumAlpha = 0.18f;
        [SerializeField, Range(0f, 1f)] private float maximumAlpha = 0.34f;

        private Tween rotationTween;
        private Tween pulseTween;

        private void OnEnable()
        {
            Play();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            Stop();
            ui_transform_rotate_value.localRotation = Quaternion.Euler(0f, 0f, 8f);
            SetAlpha(minimumAlpha);

            float secondsPerTurn = 360f / Mathf.Max(0.01f, rotationSpeedDegrees);
            rotationTween = UiTween.LoopRotate(ui_transform_rotate_value, secondsPerTurn);

            float halfPulseDuration = 0.5f / Mathf.Max(0.01f, pulseCyclesPerSecond);
            pulseTween = UiTween.LoopAlphaPulse(
                ui_image_pulse_value,
                minimumAlpha,
                maximumAlpha,
                halfPulseDuration);
        }

        public void Stop()
        {
            rotationTween.Kill(false);
            pulseTween.Kill(false);
            rotationTween = null;
            pulseTween = null;
        }

        private void SetAlpha(float alpha)
        {
            Color color = ui_image_pulse_value.color;
            color.a = alpha;
            ui_image_pulse_value.color = color;
        }
    }
}
