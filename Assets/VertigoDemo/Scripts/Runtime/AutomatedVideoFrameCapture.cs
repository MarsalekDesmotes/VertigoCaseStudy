using System.Collections;
using System.IO;
using UnityEngine;

namespace VertigoDemo
{
    public sealed class AutomatedVideoFrameCapture : MonoBehaviour
    {
        private const string FramesArgument = "-captureFrames";
        private const float FramesPerSecond = 15f;

        private IEnumerator Start()
        {
            string directory = ReadOutputDirectory();
            if (string.IsNullOrWhiteSpace(directory))
            {
                yield break;
            }

            Application.runInBackground = true;
            Directory.CreateDirectory(directory);
            WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
            float nextCapture = Time.realtimeSinceStartup;
            int frame = 0;

            while (true)
            {
                yield return endOfFrame;
                if (Time.realtimeSinceStartup < nextCapture)
                {
                    continue;
                }

                Texture2D image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                image.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
                image.Apply(false, false);
                byte[] bytes = image.EncodeToJPG(88);
                Destroy(image);

                string path = Path.Combine(directory, "frame_" + frame.ToString("D5") + ".jpg");
                File.WriteAllBytes(path, bytes);
                frame++;
                nextCapture += 1f / FramesPerSecond;
            }
        }

        private static string ReadOutputDirectory()
        {
            string[] arguments = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length - 1; i++)
            {
                if (arguments[i] == FramesArgument)
                {
                    return arguments[i + 1];
                }
            }

            return null;
        }
    }
}
