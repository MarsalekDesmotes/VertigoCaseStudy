using System.Collections;
using System.IO;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class AutomatedScreenshotCapture : MonoBehaviour
    {
        private const string CaptureArgument = "-captureScreenshot";

        private IEnumerator Start()
        {
            string path = ReadCapturePath();
            if (string.IsNullOrWhiteSpace(path))
            {
                yield break;
            }

            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            yield return null;
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(path);

            float timeout = Time.realtimeSinceStartup + 10f;
            while (!File.Exists(path) && Time.realtimeSinceStartup < timeout)
            {
                yield return null;
            }

            Application.Quit(File.Exists(path) ? 0 : 2);
        }

        private static string ReadCapturePath()
        {
            string[] arguments = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length - 1; i++)
            {
                if (arguments[i] == CaptureArgument)
                {
                    return arguments[i + 1];
                }
            }

            return null;
        }
    }
}
