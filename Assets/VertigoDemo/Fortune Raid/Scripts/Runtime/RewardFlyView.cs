using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoDemo
{
    public sealed class RewardFlyView : MonoBehaviour
    {
        private const float FlightDuration = 0.48f;
        private const float LandingDuration = 0.10f;

        [SerializeField] private Image ui_image_reward_fly_value;

        private Sequence flightTween;

        public void Play(
            Sprite sprite,
            Vector3 startPosition,
            RectTransform target,
            Action onComplete)
        {
            flightTween.Kill(false);
            ui_image_reward_fly_value.sprite = sprite;
            ui_image_reward_fly_value.preserveAspect = true;
            ui_image_reward_fly_value.color = Color.white;

            RectTransform fly = ui_image_reward_fly_value.rectTransform;
            fly.position = startPosition;
            fly.localScale = new Vector3(0.82f, 0.82f, 1f);
            fly.localRotation = Quaternion.identity;
            gameObject.SetActive(true);

            flightTween = DOTween.Sequence()
                .SetUpdate(true)
                .SetTarget(this)
                .Append(fly
                    .DOMove(target.position, FlightDuration)
                    .SetEase(Ease.InOutCubic))
                .Join(fly
                    .DOScale(new Vector3(1.12f, 1.12f, 1f), 0.12f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.OutQuad))
                .Join(fly
                    .DOLocalRotate(
                        new Vector3(0f, 0f, 18f),
                        FlightDuration)
                    .SetEase(Ease.InOutSine))
                .Append(fly
                    .DOScale(
                        new Vector3(0.42f, 0.42f, 1f),
                        LandingDuration)
                    .SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    flightTween = null;
                    onComplete?.Invoke();
                });
        }

        public void Hide()
        {
            flightTween.Kill(false);
            flightTween = null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            flightTween.Kill(false);
        }

    }
}
