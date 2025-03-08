using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// テンプレートを選択しスクリプトを作成するウィンドウを提供するエディター拡張
/// </summary>
public class ScriptCreationWindow : EditorWindow
{
    private string _scriptName = "NewScript"; // スクリプトの名前
    private string _savePath = "Assets"; // 保存パス
    private string _template = "Default";

    [MenuItem("Tools/Script Creation Window")]
    public static void ShowWindow()
    {
        // ウィンドウを表示。エディタウィンドウのレイアウトに組み込めるようにする。
        ScriptCreationWindow window = EditorWindow.GetWindow<ScriptCreationWindow>("Script Creation");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Script", EditorStyles.boldLabel);

        // スクリプト名を入力するフィールド
        _scriptName = EditorGUILayout.TextField("Script Name", _scriptName);

        // テンプレートを選ぶドロップダウンメニュー
        _template = EditorGUILayout.Popup("Template", _template == "Default" ? 0 : 1, new string[] { "Default", "MonoBehaviour" }) == 0 ? "Default" : "MonoBehaviour";

        // フォルダパスを選択するフィールド
        GUILayout.Label("Select Save Folder", EditorStyles.boldLabel);
        _savePath = EditorGUILayout.TextField("Folder Path", _savePath);
        
        // パス選択ボタン
        if (GUILayout.Button("Select Folder"))
        {
            string folderPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");

            if (!string.IsNullOrEmpty(folderPath))
            {
                if (folderPath.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
                }
                _savePath = folderPath;
            }
        }
        
        // ボタンを押してスクリプトを作成
        if (GUILayout.Button("Create Script"))
        {
            CreateScript();
        }
    }

    private void CreateScript()
    {
        // フォルダパスが空でないかチェック
        if (string.IsNullOrEmpty(_savePath))
        {
            Debug.LogError("Please specify a folder path.");
            return;
        }
        
        // スクリプト名を確保
        string path = Path.Combine(_savePath, $"{_scriptName}.cs");

        // テンプレートによって内容を決める
        string scriptContent = "";
        if (_template == "MonoBehaviour")
        {
            scriptContent = "using UnityEngine;\n\npublic class " + _scriptName + " : MonoBehaviour\n{\n    void Start()\n    {\n        \n    }\n\n    void Update()\n    {\n        \n    }\n}";
        }
        else
        {
            scriptContent = "public class " + _scriptName + "\n{\n    \n}";
        }

        // スクリプトファイルを作成
        File.WriteAllText(path, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"Script {_scriptName} created at {path}");
    }
}