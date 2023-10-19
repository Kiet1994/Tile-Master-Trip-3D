using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public static class CUtils
{
    public static void SwapValue<T>(ref T value1, ref T value2)
    {
        T temp = value1;
        value1 = value2;
        value2 = temp;
    }

    public static float EaseOut(float t)
    {
        return 1.0f - (1.0f - t) * (1.0f - t) * (1.0f - t);
    }

    public static float EaseIn(float t)
    {
        return t * t * t;
    }

    public static Tweener DOTextInt(this TextMeshProUGUI text, int initialValue, int finalValue, float duration, Func<int, string> convertor)
    {
        return DOTween.To(
             () => initialValue,
             it => text.text = convertor(it),
             finalValue,
             duration
         );
    }

    public static Tweener DOTextInt(this TextMeshProUGUI text, int initialValue, int finalValue, float duration)
    {
        return DOTextInt(text, initialValue, finalValue, duration, it => it.ToString());
    }

    public static Tweener DOTextInt(this TextMeshPro text, int initialValue, int finalValue, float duration, Func<int, string> convertor)
    {
        return DOTween.To(
             () => initialValue,
             it => text.text = convertor(it),
             finalValue,
             duration
         );
    }

    public static Tweener DOTextInt(this TextMeshPro text, int initialValue, int finalValue, float duration)
    {
        return DOTextInt(text, initialValue, finalValue, duration, it => it.ToString());
    }

    public static string FormatNumber(int n)
    {
        if (n < 1000)
            return n.ToString();

        if (n < 10000)
            return String.Format("{0:#,.##}K", n - 5);

        if (n < 100000)
            return String.Format("{0:#,.#}K", n - 50);

        if (n < 1000000)
            return String.Format("{0:#,.}K", n - 500);

        if (n < 10000000)
            return String.Format("{0:#,,.##}M", n - 5000);

        if (n < 100000000)
            return String.Format("{0:#,,.#}M", n - 50000);

        if (n < 1000000000)
            return String.Format("{0:#,,.}M", n - 500000);

        return String.Format("{0:#,,,.##}B", n - 5000000);
    }

    public static float GetAnimLenght(Animator animator, string animName)
    {
        float duration = .5f;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(animName))
            {
                duration = clip.length;
                break;
            }
        }
        return duration;
    }

    public static IEnumerator CheckInternetConnection(Action<bool> callback)
    {
        Debug.Log("[INFO] Checking for internet connection");
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Debug.Log("[INFO] Internet is reachable");
            UnityWebRequest connection = new UnityWebRequest("https://veryplay.studio/privacy-policy/");
            yield return connection;
            Debug.Log("[INFO] Connection error " + !string.IsNullOrEmpty(connection.error));
            callback(connection.error == null);
        }
        else
        {
            Debug.Log("[INFO] Internet is not reachable");
            callback(false);
        }
    }
}