using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VertigoDemo
{
    public sealed class ButtonPressView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform ui_transform_button_animator;
        [SerializeField, Range(0.8f, 1f)] private float pressedScale = 0.96f;

        private Tween pressTween;

#if UNITY_EDITOR
        public void Configure(RectTransform animator)
        {
            ui_transform_button_animator = animator;
        }
#endif

        public void OnPointerDown(PointerEventData eventData)
        {
            AnimateTo(pressedScale, 0.08f, Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AnimateTo(1f, 0.12f, Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateTo(1f, 0.12f, Ease.OutBack);
        }

        private void AnimateTo(float scale, float duration, Ease ease)
        {
            pressTween.Kill(false);
            pressTween = UiTween.Scale(
                ui_transform_button_animator,
                new Vector3(scale, scale, 1f),
                duration,
                ease);
        }

        private void OnDisable()
        {
            pressTween.Kill(false);
            pressTween = null;
            ui_transform_button_animator.localScale = Vector3.one;
        }
    }
}
