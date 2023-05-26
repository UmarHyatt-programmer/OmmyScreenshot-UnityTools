using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCaptureTest : MonoBehaviour
{
    public string fileName;
    private void Update() {
        if(Input.GetKeyDown("c"))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        print("screenshot");
        ScreenCapture.CaptureScreenshot("/Users/mac/Desktop/Github/OmmyScreenshot-UnityTools/Assets/"+fileName+".png",1);
    }
}
