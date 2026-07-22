using System.Collections;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class AutomatedDemoDriver : MonoBehaviour
    {
        private const string DemoArgument = "-autoDemo";

        [SerializeField] private GameController gameController;
        [SerializeField] private GameScreenView gameScreenView;

        public void Configure(GameController controller, GameScreenView screenView)
        {
            gameController = controller;
            gameScreenView = screenView;
        }

        private IEnumerator Start()
        {
            if (!HasCommandLineArgument(DemoArgument))
            {
                yield break;
            }

            yield return new WaitForSecondsRealtime(8f);
            for (int spin = 0; spin < 2; spin++)
            {
                gameController.TriggerDemoSpin();
                while (gameScreenView.Wheel.IsSpinning)
                {
                    yield return null;
                }

                yield return new WaitForSecondsRealtime(1.4f);
                gameScreenView.TriggerResultPrimaryForDemo();
                yield return new WaitForSecondsRealtime(0.8f);
            }

            yield return new WaitForSecondsRealtime(3f);
            Application.Quit(0);
        }

        private static bool HasCommandLineArgument(string value)
        {
            string[] arguments = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == value)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            if (gameController == null) gameController = GetComponent<GameController>();
            if (gameScreenView == null) gameScreenView = GetComponent<GameScreenView>();
        }
    }
}
