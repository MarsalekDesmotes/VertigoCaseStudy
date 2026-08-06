using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public static class UiTween
    {
        public static Tween Fade(CanvasGroup group, float end, float duration, Ease ease = Ease.OutQuad)
        {
            return DOTween.To(
                    () => group.alpha,
                    value => group.alpha = value,
                    end,
                    duration)
                .SetEase(ease)
                .SetUpdate(true)
                .SetTarget(group);
        }

        public static Tween FadeAlpha(Image image, float end, float duration, Ease ease = Ease.OutQuad)
        {
            return DOTween.To(
                    () => image.color.a,
                    value =>
                    {
                        Color color = image.color;
                        color.a = value;
                        image.color = color;
                    },
                    end,
                    duration)
                .SetEase(ease)
                .SetUpdate(true)
                .SetTarget(image);
        }

        public static Tween Scale(Transform target, Vector3 end, float duration, Ease ease = Ease.OutQuad)
        {
            return target
                .DOScale(end, duration)
                .SetEase(ease)
                .SetUpdate(true)
                .SetTarget(target);
        }

        public static Tween PunchScale(
            Transform target,
            Vector3 punch,
            float duration,
            int vibrato = 8,
            float elasticity = 0.55f)
        {
            return target
                .DOPunchScale(punch, duration, vibrato, elasticity)
                .SetUpdate(true)
                .SetTarget(target);
        }

        public static Tween LoopRotate(Transform target, float secondsPerTurn)
        {
            return target
                .DOLocalRotate(new Vector3(0f, 0f, 360f), secondsPerTurn, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true)
                .SetTarget(target);
        }

        public static Tween LoopAlphaPulse(Image image, float min, float max, float halfDuration)
        {
            return DOVirtual
                .Float(min, max, halfDuration, alpha =>
                {
                    Color color = image.color;
                    color.a = alpha;
                    image.color = color;
                })
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true)
                .SetTarget(image);
        }

        public static Tween FlashFadeOut(Image image, Color start, float duration, Action onComplete)
        {
            image.gameObject.SetActive(true);
            image.color = start;
            return FadeAlpha(image, 0f, duration)
                .OnComplete(() =>
                {
                    image.gameObject.SetActive(false);
                    onComplete();
                });
        }
    }
}
