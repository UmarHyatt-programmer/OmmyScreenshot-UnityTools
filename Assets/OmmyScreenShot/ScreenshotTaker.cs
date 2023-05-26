using UnityEngine;
using UnityEditor;
[ExecuteAlways]
public class ScreenshotTaker : MonoBehaviour
{
    [MenuItem("Tools/Take Screenshot")]
    private static void TakeScreenshot()
    {
        string fileName = "screenshot.png";
        string filePath = System.IO.Path.Combine(Application.dataPath, fileName);

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot taken and saved to " + filePath);
    }
}
