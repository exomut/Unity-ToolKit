using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToolKit
{
    private static IEnumerator IRunAfterSeconds(Action task, float time)
    {
            yield return new WaitForSeconds(time);
            task();
    }

    public static void RunAfterSeconds(MonoBehaviour ctx, Action task, float time)
    {
        ctx.StartCoroutine(IRunAfterSeconds(task, time));
    }

    public static void AndroidBackButtonQuit()
    {
        // Android Back Button Support
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
    }
}
