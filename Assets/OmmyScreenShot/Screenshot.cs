//C# Example
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class Screenshot : EditorWindow
{

    int resWidth = Screen.width * 4;
    int resHeight = Screen.height * 4;

    public Camera myCamera;
    int scale = 1;

    string path = "";
    RenderTexture renderTexture;

    bool isTransparent = false;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Tools/Ommy ScreenShot/Open ScreenShoot Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(Screenshot));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
        editorWindow.titleContent.image = Resources.Load<Texture>("OmmyLogo");
    }

    float lastTime;


    bool isPathSelected = false;
    void OnGUI()
    {
        GUILayout.Box(Resources.Load<Texture>("OmmyLogo"), GUILayout.Width(128), GUILayout.Height(128));
        EditorGUILayout.LabelField("Ommy's ScreenShoot Plugin", EditorStyles.boldLabel);
        GUILayout.Label("Save Path", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(path, GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
            isPathSelected = true;
        }
        else if (!isPathSelected)
        {
            path = Application.dataPath;
        }
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.red;
        if (GUILayout.Button("Default Path",style,GUILayout.ExpandWidth(false)))
        {
            path = Application.dataPath;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Choose screenshots Save Path", MessageType.None);
        EditorGUILayout.Space();



        //isTransparent = EditorGUILayout.Toggle(isTransparent,"Transparent Background");



        GUILayout.Label("Pass Your Camera", EditorStyles.boldLabel);


        myCamera = EditorGUILayout.ObjectField(myCamera, typeof(Camera), true, null) as Camera;


        if (myCamera == null)
        {
            myCamera = Camera.main;
        }

        isTransparent = EditorGUILayout.Toggle("Transparent Background", isTransparent);


        EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
        resWidth = EditorGUILayout.IntField("Width", resWidth);
        resHeight = EditorGUILayout.IntField("Height", resHeight);

        EditorGUILayout.Space();

		EditorGUILayout.EndHorizontal();
        scale = EditorGUILayout.IntSlider ("Multiplayer", scale, 1, 30 ,GUILayout.ExpandWidth(true));

       // EditorGUILayout.HelpBox("The default mode of screenshot is crop - so choose a proper width and height. The scale is a factor " +
        //    "to multiply or enlarge the renders without loosing quality.", MessageType.None);


        EditorGUILayout.Space();

        EditorGUILayout.Space();
      //  EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Capture Setting", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set To Screen Size",GUILayout.Height(20),GUILayout.Height(40)))
        {
            resHeight = (int)Handles.GetMainGameViewSize().y;
            resWidth = (int)Handles.GetMainGameViewSize().x;

        }


        if (GUILayout.Button("Standerd Size",GUILayout.Height(20),GUILayout.Height(40)))
        {
            resHeight = 1440;
            resWidth = 2560;
            scale = 1;
        }
		EditorGUILayout.EndHorizontal();
       // EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Capture Size will be " + resWidth * scale + " x " + resHeight * scale + " px", EditorStyles.boldLabel);
        var TakeScreenstyle = new GUIStyle(GUI.skin.button);
        TakeScreenstyle.normal.textColor = Color.red;
        TakeScreenstyle.richText=true;
        TakeScreenstyle.normal.background=new Texture2D(20,20);
        if (GUILayout.Button("Take Screenshot",TakeScreenstyle ,GUILayout.MinHeight(30)))
        {
            if (path == "")
            {
                path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
                Debug.Log("Path Set");
                TakeHiResShot();
            }
            else
            {
                TakeHiResShot();
            }
        }

        EditorGUILayout.Space();
    //    EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Last Screenshot", GUILayout.MinHeight(25)))
        {
            if (lastScreenshot != "")
            {
                Application.OpenURL("file://" + lastScreenshot);
                Debug.Log("Opening File " + lastScreenshot);
            }
        }

        if (GUILayout.Button("Open Folder", GUILayout.MinHeight(25)))
        {

            Application.OpenURL("file://" + path);
        }
        if (GUILayout.Button("More Assets",GUILayout.MinHeight(25)))
        {
            Application.OpenURL("https://sites.google.com/view/umarhyatttools/home");
        }

       // EditorGUILayout.EndHorizontal();


        if (takeHiResShot)
        {
            int resWidthN = resWidth * scale;
            int resHeightN = resHeight * scale;
            RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
            myCamera.targetTexture = rt;

            TextureFormat tFormat;
            if (isTransparent)
                tFormat = TextureFormat.ARGB32;
            else
                tFormat = TextureFormat.RGB24;


            Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
            myCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            myCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidthN, resHeightN);

            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            Application.OpenURL(filename);
            takeHiResShot = false;
        }
    }



    private bool takeHiResShot = false;
    public string lastScreenshot = "";


    public string ScreenShotName(int width, int height)
    {

        string strPath = "";

        strPath = string.Format("{0}/screen_{1}x{2}_{3}.png",
                             path,
                             width, height,
                                       System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        lastScreenshot = strPath;

        return strPath;
    }



    public void TakeHiResShot()
    {
        Debug.Log("Taking Screenshot");
        takeHiResShot = true;
    }

}

