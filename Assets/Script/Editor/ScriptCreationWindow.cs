using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// テンプレートを選択しスクリプトを作成するウィンドウを提供するエディター拡張
/// </summary>
public class ScriptCreationWindow : EditorWindow
{
    private string _scriptName = "NewScript"; // スクリプトの名前
    private string _savePath = "Assets"; // 保存パス
    private int _templateIndex = 0; // テンプレートのインデックス
    private string[] _templates = new string[0]; // テンプレートの配列
    private string _templateFolderPath = "Assets/Script/ScriptTemplates"; // スクリプトテンプレートが置かれているフォルダ
    

    [MenuItem("Tools/Script Creation Window")]
    public static void ShowWindow()
    {
        // ウィンドウを表示。エディタウィンドウのレイアウトに組み込めるようにする。
        ScriptCreationWindow window = EditorWindow.GetWindow<ScriptCreationWindow>("Script Creation");
        window.Show();
    }

    private void OnEnable()
    {
        LoadTemplates(); // スクリプトのテンプレートをロード
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Script", EditorStyles.boldLabel);

        // スクリプト名を入力するフィールド
        _scriptName = EditorGUILayout.TextField("Script Name", _scriptName);

        // テンプレートを選ぶドロップダウンメニュー
        if (_templates != null && _templates.Length > 0)
        {
            // テンプレートが存在したらポップアップに含めて表示する
            _templateIndex = EditorGUILayout.Popup("Template", _templateIndex, _templates);
        }
        else
        {
            // テンプレートが存在しなかったらメッセージを出す
            EditorGUILayout.HelpBox("No templates found in the 'ScriptTemplates' folder.", MessageType.Warning);
        }
        
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

    /// <summary>
    /// テンプレートの格納フォルダからテンプレートからリストを作成する
    /// </summary>
    private void LoadTemplates()
    {
        if (Directory.Exists(_templateFolderPath))
        {
            // フォルダ内のtxtファイルをすべて取得
            string[] templateFiles = Directory.GetFiles(_templateFolderPath, "*.txt");
            
            // テンプレート名をファイル名（拡張子なし）でリスト化
            _templates = templateFiles
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .ToArray();
        }
        else
        {
            _templates = new string[] { }; // フォルダがない場合は空のリスト
        }
    }
    
    /// <summary>
    /// 作成
    /// </summary>
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
        
        // スクリプトが既に存在するか確認
        if (File.Exists(path))
        {
            Debug.LogError($"Script {_scriptName} already exists!");
            return;
        }
        
        // 選ばれたテンプレートを読み込む
        string selectedTemplate = _templates[_templateIndex];
        string templatePath = $"{_templateFolderPath}/{selectedTemplate}.txt";

        if (!File.Exists(templatePath))
        {
            Debug.LogError($"Template file {selectedTemplate}.txt not found!");
            return;
        }

        // テンプレートファイルの内容を読み込む
        string templateContent = File.ReadAllText(templatePath);

        // スクリプト内容をファイルに書き込む
        string scriptContent = templateContent.Replace("{ClassName}", _scriptName); // テンプレート内の {ClassName} を置き換え

        // スクリプトファイルを作成
        File.WriteAllText(path, scriptContent);
        AssetDatabase.Refresh();
        Debug.Log($"Script {_scriptName} created at {path}");
    }
}