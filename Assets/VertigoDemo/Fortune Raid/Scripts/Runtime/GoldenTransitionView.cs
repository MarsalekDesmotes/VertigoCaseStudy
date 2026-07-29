using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class GoldenTransitionView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup ui_canvas_group_super_transition;
        [SerializeField] private TMP_Text ui_text_super_transition_title_value;
        [SerializeField] private TMP_Text ui_text_super_transition_punchline_value;

        private Sequence transitionTween;

        public bool IsPlaying { get; private set; }

        public void Play(int zone, Action revealWheel, Action onComplete)
        {
            transitionTween.Kill(false);
            transitionTween = null;
            IsPlaying = true;

            ui_text_super_transition_title_value.text = "GOLDEN ZONE " + zone;
            gameObject.SetActive(true);
            ui_canvas_group_super_transition.alpha = 0f;
            ui_text_super_transition_title_value.rectTransform.localScale =
                new Vector3(0.78f, 0.78f, 1f);
            ui_text_super_transition_punchline_value.rectTransform.localScale =
                new Vector3(0.88f, 0.88f, 1f);
            ui_text_super_transition_punchline_value.alpha = 0f;

            transitionTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(gameObject)
                .Append(DOTween.To(
                    () => ui_canvas_group_super_transition.alpha,
                    value => ui_canvas_group_super_transition.alpha = value,
                    1f,
                    0.10f))
                .Join(ui_text_super_transition_title_value.rectTransform
                    .DOScale(Vector3.one, 0.24f)
                    .SetEase(Ease.OutBack))
                .InsertCallback(0.14f, () => revealWheel?.Invoke())
                .Insert(0.13f, DOTween.To(
                    () => ui_text_super_transition_punchline_value.alpha,
                    value => ui_text_super_transition_punchline_value.alpha = value,
                    1f,
                    0.18f))
                .Insert(0.13f, ui_text_super_transition_punchline_value.rectTransform
                    .DOScale(Vector3.one, 0.20f)
                    .SetEase(Ease.OutBack))
                .AppendInterval(0.24f)
                .Append(DOTween.To(
                    () => ui_canvas_group_super_transition.alpha,
                    value => ui_canvas_group_super_transition.alpha = value,
                    0f,
                    0.20f))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    IsPlaying = false;
                    transitionTween = null;
                    onComplete?.Invoke();
                });
        }

        private void OnDestroy()
        {
            transitionTween.Kill(false);
        }

    }
}
