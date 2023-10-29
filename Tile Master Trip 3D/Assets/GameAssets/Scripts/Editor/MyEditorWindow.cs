using UnityEngine;
using UnityEditor;

public class MyEditorWindow : EditorWindow
{
    private float width = 200;
    private float height = 100;

    [MenuItem("Window/My Editor Window")]
    static void ShowWindow()
    {
        GetWindow<MyEditorWindow>("My Window");
    }

    void OnGUI()
    {
        // Khai báo và sử dụng GUILayoutOption[]
        GUILayoutOption[] layoutOptions = new GUILayoutOption[]
        {
            GUILayout.Width(width),
            GUILayout.Height(height),
        };

        GUILayout.BeginArea(new Rect(10, 10, 300, 700));
        GUILayout.Box("This is a custom box", layoutOptions);
        GUILayout.Space(10);
        GUILayout.Button("Click Me", layoutOptions);
        if (GUILayout.Button("Resize"))
        {
            // Thay đổi kích thước phần tử giao diện
            width += 20;
            height += 20;
        }

        GUILayout.EndArea();
    }
}