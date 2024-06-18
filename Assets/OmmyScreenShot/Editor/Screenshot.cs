using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Ommy.Screenshot
{
    [ExecuteInEditMode]
    public class OmmyScreenshot : EditorWindow
    {
        public Camera myCamera;
        int scale = 1;
        string path = "";
        bool aplha = false;
        bool isPortrait = false;
        bool takeHiResShot = false;
        public string lastScreenshot = "";
        Texture2D logo;
        List<Vector2Int> customResolutions = new List<Vector2Int>();
        public Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Ommy/Ommy Screenshot/Open ScreenShoot Window")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow<OmmyScreenshot>();
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.titleContent = new GUIContent("Screenshot");
            editorWindow.Show();
        }

        void OnEnable()
        {
            logo = Resources.Load<Texture2D>("logo");
        }

        void OnGUI()
        {
            // Custom GUI styles
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.grey }
            };
            GUIStyle logoStyle = new GUIStyle(GUI.skin.label)
            {
                fixedHeight = 64,
                fixedWidth = 64,
                alignment = TextAnchor.UpperLeft,
            };
            GUIStyle subHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white, background = MakeTex(2, 2, new Color(0.1f, 0.5f, 0.8f, 1.0f)) },
                fixedHeight = 40
            };
            GUIStyle captureButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.green, background = MakeTex(2, 2, Color.blue) },
                fixedHeight = 40
            };
            GUIStyle browseButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white, background = MakeTex(2, 2, new Color(0.2f, 0.6f, 0.3f, 1.0f)) },
                fixedHeight = 30
            };

            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 14,
                normal = { textColor = Color.white, background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1.0f)) }
            };

            GUIStyle intFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white, background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 1.0f)) }
            };
            GUIStyle toggleFieldStyle = new GUIStyle(GUI.skin.toggle)
            {
                fontStyle = FontStyle.Bold
            };
            // Main layout
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);
            // Horizontal layout for header and logo
            GUILayout.BeginHorizontal();

            // Display logo
            //GUILayout.Label(logo, GUILayout.Height(64), GUILayout.Width(64));
            GUILayout.Label(logo, logoStyle);

            // Header text
            GUILayout.BeginVertical();
            GUILayout.Label("Ommy Screenshot Plugin", headerStyle);
            GUILayout.Space(10);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal(); // End horizontal layout for header and logo

            GUILayout.Space(10);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            // Custom resolutions section
            GUILayout.Label("Custom Resolutions", subHeaderStyle);
            GUILayout.Space(5);

            for (int i = 0; i < customResolutions.Count; i++)
            {
                GUILayout.BeginHorizontal();
                customResolutions[i] = new Vector2Int(EditorGUILayout.IntField(customResolutions[i].x, intFieldStyle, GUILayout.Width(80)),
                                                      EditorGUILayout.IntField(customResolutions[i].y, intFieldStyle, GUILayout.Width(80)));

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    customResolutions.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Resolution", GUILayout.Width(150)))
            {
                customResolutions.Add(new Vector2Int(1920, 1080));
            }

            GUILayout.Space(10);

            //scale = EditorGUILayout.IntSlider("Scale", scale, 1, 15);

            GUILayout.Space(10);
            GUILayout.Label("Presets", subHeaderStyle);
            isPortrait = EditorGUILayout.Toggle("Is Portrait", isPortrait);
            EditorGUILayout.HelpBox("Check portrait if your game in portrait mode.", MessageType.Info);
            if (GUILayout.Button("Icon Size", buttonStyle))
            {
                customResolutions.Clear();
                customResolutions.Add(new Vector2Int(1024, 1024));
                customResolutions.Add(new Vector2Int(512, 512));
                scale = 1;
            }
            if (GUILayout.Button("IOS Size", buttonStyle))
            {
                customResolutions.Clear();
                if (isPortrait)
                {
                    customResolutions.Add(new Vector2Int(1290, 2796));
                    customResolutions.Add(new Vector2Int(1284, 2778));
                    customResolutions.Add(new Vector2Int(1242, 2208));
                    customResolutions.Add(new Vector2Int(2048, 2732));
                }
                else
                {
                    customResolutions.Add(new Vector2Int(2796, 1290));
                    customResolutions.Add(new Vector2Int(2778, 1284));
                    customResolutions.Add(new Vector2Int(2208, 1242));
                    customResolutions.Add(new Vector2Int(2732, 2048));
                }
                scale = 1;
            }
            if (GUILayout.Button("Android Size", buttonStyle))
            {
                customResolutions.Clear();
                if (isPortrait) customResolutions.Add(new Vector2Int(1920, 1080));
                else customResolutions.Add(new Vector2Int(1080, 1920));
                scale = 1;
            }
            if (GUILayout.Button("Feature Image Size", buttonStyle))
            {
                customResolutions.Clear();
                if (isPortrait) customResolutions.Add(new Vector2Int(1024, 500));
                else customResolutions.Add(new Vector2Int(1024, 500));
                scale = 1;
            }
            if (GUILayout.Button("Set To Screen Size", buttonStyle))
            {
                Vector2 screenSize = Handles.GetMainGameViewSize();
                customResolutions.Clear();
                customResolutions.Add(new Vector2Int((int)screenSize.x, (int)screenSize.y));
            }
            // Save path section
            GUILayout.Label("Save Path", subHeaderStyle);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(path, textFieldStyle);
            if (GUILayout.Button("Browse", browseButtonStyle, GUILayout.Width(80)))
                path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("Choose the folder to save the screenshots.", MessageType.Info);
            GUILayout.Space(10);

            // Camera selection
            GUILayout.Label("Select Camera", subHeaderStyle);
            GUILayout.Space(5);
            myCamera = (Camera)EditorGUILayout.ObjectField(myCamera, typeof(Camera), true);

            if (myCamera == null)
                myCamera = Camera.main;

            aplha = EditorGUILayout.Toggle("Use Alpha Color", aplha, toggleFieldStyle);
            EditorGUILayout.HelpBox("Check Alpha if you want format ARGB32 instead of RGB24.", MessageType.Info);

            GUILayout.Space(10);



            // if (GUILayout.Button("Default Size", buttonStyle))
            // {
            //     customResolutions.Clear();
            //     customResolutions.Add(new Vector2Int(2560, 1440));
            //     scale = 1;
            // }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Take Screenshot", captureButtonStyle))
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                    Debug.Log("Path Set");
                }
                TakeHiResShot();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Take Screenshot With UI", buttonStyle))
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                    Debug.Log("Path Set");
                }
                CaptureScreenshotsWithUI();
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Last Screenshot", buttonStyle))
            {
                if (!string.IsNullOrEmpty(lastScreenshot))
                {
                    Application.OpenURL("file://" + lastScreenshot);
                    Debug.Log("Opening File " + lastScreenshot);
                }
            }

            if (GUILayout.Button("Open Folder", buttonStyle))
            {
                Application.OpenURL("file://" + path);
            }

            // if (GUILayout.Button("More Assets", buttonStyle))
            // {
            //     Application.OpenURL("https://assetstore.unity.com/publishers/71963");
            // }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (takeHiResShot)
            {
                CaptureScreenshots();
                takeHiResShot = false;
            }
            GUILayout.EndScrollView();
        }
        [ExecuteAlways]
        void CaptureScreenshotsWithUI()
        {
            foreach (var resolution in customResolutions)
            {
                EditorApplication.ExecuteMenuItem("Window/General/Game");

                int resWidthN = resolution.x * scale;
                int resHeightN = resolution.y * scale;
                string filename = ScreenShotName(resWidthN, resHeightN);

                ScreenCapture.CaptureScreenshot(filename, scale);

                Debug.Log($"Took screenshot with UI to: {filename}");
            }

            // Open the last screenshot taken
            if (customResolutions.Count > 0)
            {
                var lastResolution = customResolutions[customResolutions.Count - 1];
                Application.OpenURL("file://" + ScreenShotName(lastResolution.x * scale, lastResolution.y * scale));
            }
        }

        public string ScreenShotName(int width, int height)
        {
            string strPath = string.Format("{0}/screen_{1}x{2}_{3}.png", path, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            lastScreenshot = strPath;
            return strPath;
        }

        public void TakeHiResShot()
        {
            Debug.Log("Taking Screenshot");
            takeHiResShot = true;
        }

        void CaptureScreenshots()
        {
            foreach (var resolution in customResolutions)
            {
                int resWidthN = resolution.x * scale;
                int resHeightN = resolution.y * scale;
                string filename = ScreenShotName(resWidthN, resHeightN);

                RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
                myCamera.targetTexture = rt;

                TextureFormat tFormat = aplha ? TextureFormat.ARGB32 : TextureFormat.RGB24;
                Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
                myCamera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
                myCamera.targetTexture = null;
                RenderTexture.active = null; // added line to fix an issue with RenderTexture
                DestroyImmediate(rt); // added line to avoid memory leaks

                // Save the screenshot as a PNG file
                byte[] bytes = screenShot.EncodeToPNG();
                File.WriteAllBytes(filename, bytes);

                // Clean up memory
                DestroyImmediate(screenShot);

                Debug.Log($"Took screenshot to: {filename}");
            }

            // Open the last screenshot taken
            if (customResolutions.Count > 0)
            {
                var lastResolution = customResolutions[customResolutions.Count - 1];
                Application.OpenURL("file://" + ScreenShotName(lastResolution.x * scale, lastResolution.y * scale));
            }
        }

        // Helper method to create textures
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
