using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GraphicDefaultValueWriter
{
#if UNITY_EDITOR
    private static HashSet<Graphic> sceneGraphics = new HashSet<Graphic>();
    private static string scenesHash = string.Empty;

    private static List<GameObject> tempRoots = new List<GameObject>();
    private static List<Graphic> tempGraphics = new List<Graphic>();

    private static HashAlgorithm hasher = new SHA512Managed();

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
        BuildInitialSceneGraphics(ComputeScenesHash());
    }

    private static void OnHierarchyChanged()
    {
        if (Application.isPlaying)
        {
            scenesHash = string.Empty;
            return;
        }

        var hash = ComputeScenesHash();
        if (hash != scenesHash)
        {
            // New scene, rebuild
            BuildInitialSceneGraphics(hash);
        }
        else
        {
            // Each new graphics, set default value
            foreach (var graphic in GetNewGraphics())
            {
                graphic.raycastTarget = graphic.GetComponent<IEventSystemHandler>() != null;
            }
        }
    }

    private static void BuildInitialSceneGraphics(string hash)
    {
        sceneGraphics.Clear();
        scenesHash = hash;

        foreach (var graphic in GetSceneGraphics())
        {
            sceneGraphics.Add(graphic);
        }
    }

    private static string ComputeScenesHash()
    {
        var sb = new StringBuilder();
        for (var ss = 0; ss < SceneManager.sceneCount; ss++)
        {
            var scene = SceneManager.GetSceneAt(ss);
            if (scene.isLoaded)
            {
                sb.Append(scene.path);
            }
        }

        var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        var hashSb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash)
        {
            hashSb.AppendFormat("{0:X2}", b);
        }

        return hashSb.ToString();
    }

    private static IEnumerable<Scene> GetLoadedScenes()
    {
        for (var ss = 0; ss < SceneManager.sceneCount; ss++)
        {
            var scene = SceneManager.GetSceneAt(ss);
            if (scene.isLoaded)
            {
                yield return scene;
            }
        }
    }

    private static IEnumerable<Graphic> GetSceneGraphics()
    {
        foreach (var scene in GetLoadedScenes())
        {
            tempRoots.Clear();
            scene.GetRootGameObjects(tempRoots);
            foreach (var root in tempRoots)
            {
                tempGraphics.Clear();
                root.GetComponentsInChildren<Graphic>(tempGraphics);
                foreach (var graphic in tempGraphics)
                {
                    yield return graphic;
                }
            }
        }
    }

    private static IEnumerable<Graphic> GetNewGraphics()
    {
        foreach (var graphic in GetSceneGraphics())
        {
            if (!sceneGraphics.Contains(graphic))
            {
                sceneGraphics.Add(graphic);
                yield return graphic;
            }
        }
    }
#endif
}