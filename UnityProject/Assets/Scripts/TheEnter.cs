using UnityEngine;

/// <summary>
/// 伟大的第一推动力泽恩特 :>
/// </summary>
public static class TheEnter
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Enter()
    {
        new Modules.Launcher().Init(() =>
        {
            var go = new GameObject("js");
            UnityEngine.GameObject.DontDestroyOnLoad(go);

            var js = go.AddComponent<Modules.JS.JSModule>();
            js.StartCoroutine(__JsInit());
        });
    }

    private static System.Collections.IEnumerator __JsInit()
    {
        var js = Modules.JS.JSModule.Instance;
        yield return new WaitUntil(() => js.State);
        js.Env.Eval("require('launcher')");
    }
}