using System.Collections;
using UnityEngine;

public sealed class Coroutines : MonoBehaviour
{
    public static Coroutines Instance { get => _instance != null ? _instance : CreateComponent(); }

    public static Coroutine Run(IEnumerator enumerator) => Instance.StartCoroutine(enumerator);
    public static void Stop(IEnumerator enumerator) => Instance.StopCoroutine(enumerator);
    public static void Stop(Coroutine coroutine) => Instance.StopCoroutine(coroutine);

    private static Coroutines _instance = null;
    private static Coroutines CreateComponent()
    {
        var globalObject = new GameObject("[Coroutines]");
        _instance = globalObject.AddComponent<Coroutines>();
        DontDestroyOnLoad(globalObject);
        return _instance;
    }
}