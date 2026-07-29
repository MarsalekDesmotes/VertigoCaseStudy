using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class PopupSunrayView : MonoBehaviour
    {
        [SerializeField] private RectTransform ui_transform_sunray_animator;
        [SerializeField] private Image ui_image_sunray_value;
        [SerializeField, Min(0f)] private float rotationSpeedDegrees = 12f;
        [SerializeField, Min(0f)] private float pulseCyclesPerSecond = 0.55f;
        [SerializeField, Range(0f, 1f)] private float minimumAlpha = 0.18f;
        [SerializeField, Range(0f, 1f)] private float maximumAlpha = 0.34f;

        private Tween rotationTween;
        private Tween pulseTween;

        private void OnEnable()
        {
            ui_transform_sunray_animator.localRotation = Quaternion.Euler(0f, 0f, 8f);
            SetAlpha(minimumAlpha);

            float rotationDuration = 360f / Mathf.Max(0.01f, rotationSpeedDegrees);
            rotationTween = ui_transform_sunray_animator
                .DOLocalRotate(new Vector3(0f, 0f, 360f), rotationDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental)
                .SetUpdate(true)
                .SetTarget(ui_transform_sunray_animator);

            float halfPulseDuration = 0.5f / Mathf.Max(0.01f, pulseCyclesPerSecond);
            pulseTween = DOVirtual
                .Float(minimumAlpha, maximumAlpha, halfPulseDuration, SetAlpha)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true)
                .SetTarget(ui_image_sunray_value);
        }

        private void OnDisable()
        {
            rotationTween.Kill(false);
            pulseTween.Kill(false);
            rotationTween = null;
            pulseTween = null;
        }

        private void SetAlpha(float alpha)
        {
            Color color = ui_image_sunray_value.color;
            color.a = alpha;
            ui_image_sunray_value.color = color;
        }

    }
}
