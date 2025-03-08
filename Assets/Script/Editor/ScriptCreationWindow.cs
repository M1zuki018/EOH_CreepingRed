using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// テンプレートを選択しスクリプトを作成するウィンドウを提供するエディター拡張
/// </summary>
public class ScriptCreationWindow : EditorWindow
{
    private string _scriptName = "NewScript"; // スクリプトの名前
    private string _savePath = "Assets"; // 保存パス
    private int _templateIndex = 0; // テンプレートのインデックス
    private string[] _templates = new string[0]; // テンプレートの配列
    private string _enumPath = "Assets";
    private readonly string _templateFolderPath = "Assets/Script/ScriptTemplates"; // スクリプトテンプレートが置かれているフォルダ
    

    [MenuItem("Tools/Script Creation Window")]
    public static void ShowWindow()
    {
        // ウィンドウを表示
        ScriptCreationWindow window = EditorWindow.GetWindow<ScriptCreationWindow>("Script Creation");
        window.Show();
    }

    private void OnEnable()
    {
        LoadTemplates(); // スクリプトのテンプレートをロード
    }

    private void OnGUI()
    {
        // スクリプト名を入力するフィールド
        _scriptName = EditorGUILayout.TextField("Script Name", _scriptName);
        
        // 保存フォルダ選択フィールド
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
        
        ShowScriptCreationGUI(); // スクリプト生成
        ShowEnumGenerationGUI(); // Enum生成
    }

    /// <summary>
    /// スクリプトのテンプレートから新しいスクリプトを作成するGUI
    /// </summary>
    private void ShowScriptCreationGUI()
    {
        GUILayout.Label("Create a New Script", EditorStyles.boldLabel);

        // テンプレートを選ぶドロップダウンメニュー
        if (_templates != null && _templates.Length > 0)
        {
            // テンプレートが存在したらポップアップに含めて表示する
            _templateIndex = EditorGUILayout.Popup("Template", _templateIndex, _templates);
        }
        else
        {
            // テンプレートが存在しなかったらメッセージを出す
            EditorGUILayout.HelpBox("'ScriptTemplates' フォルダーにテンプレートが見つかりません！", MessageType.Warning);
        }
        
        // スクリプト作成ボタン
        if (GUILayout.Button("Create Script"))
        {
            CreateScript();
        }
    }
    
    // Enumを生成するGUI
    private void ShowEnumGenerationGUI()
    {
        GUILayout.Label("Generate Enum from Folder", EditorStyles.boldLabel);

        // Enumを生成したいフォルダを選択するフィールド
        _enumPath = EditorGUILayout.TextField("Folder Path", _enumPath);
        
        // Enumを生成したいフォルダーのパスを選択するボタン
        if (GUILayout.Button("Select Create Enum Folder"))
        {
            string dataPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");

            if (!string.IsNullOrEmpty(dataPath))
            {
                if (dataPath.StartsWith(Application.dataPath))
                {
                    dataPath = "Assets" + dataPath.Substring(Application.dataPath.Length);
                }
                _enumPath = dataPath;
            }
        }

        // Enum生成ボタン
        if (GUILayout.Button("Generate Enum"))
        {
            GenerateEnum(_enumPath);
        }
    }

    /// <summary>
    /// テンプレートの格納フォルダからテンプレートをロードする
    /// </summary>
    private void LoadTemplates()
    {
        if (Directory.Exists(_templateFolderPath))
        {
            // フォルダ内のtxtファイルをすべて取得
            string[] templateFiles = Directory.GetFiles(_templateFolderPath, "*.txt");
            
            // テンプレート名をファイル名（拡張子なし）でリスト化
            _templates = templateFiles
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
        else
        {
            _templates = new string[] { }; // フォルダがない場合は空のリスト
        }
    }
    
    /// <summary>
    /// スクリプトを作成
    /// </summary>
    private void CreateScript()
    {
        // フォルダパスが空でないかチェック
        if (string.IsNullOrEmpty(_savePath))
        {
            Debug.LogError("フォルダパスが空です");
            return;
        }
        
        // スクリプト名を確保
        string path = Path.Combine(_savePath, $"{_scriptName}.cs");
        
        // スクリプトが既に存在するか確認
        if (File.Exists(path))
        {
            Debug.LogError($"Script {_scriptName} は既に存在します！");
            return;
        }
        
        // 選択されたテンプレートを読み込む
        string selectedTemplate = _templates[_templateIndex];
        string templatePath = $"{_templateFolderPath}/{selectedTemplate}.txt";

        if (!File.Exists(templatePath))
        {
            Debug.LogError($"テンプレートファイル {selectedTemplate}.txt が見つかりません");
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
    
    // フォルダ内のファイル名からEnumを生成する
    private void GenerateEnum(string folderPath)
    {
        if (string.IsNullOrEmpty(_scriptName) || string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("スクリプトの名前かEnumを生成したいファイルの名前が空です");
            return;
        }

        // フォルダ内の全てのファイルを取得
        string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        
        // Enum名を構築
        StringBuilder enumNames = new StringBuilder();
        enumNames.AppendLine($"public enum {_scriptName} {{");

        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file); // 拡張子を除いたファイル名を取得
            
            // 拡張子を除去したファイル名から「Mp3」などの不適切な部分を除去
            if (IsInvalidEnumName(fileName))
            {
                continue;
            }
            
            string enumName = ToEnumFormat(fileName);
            enumNames.AppendLine($"    {enumName},");
        }

        enumNames.AppendLine("}");
        
        // スクリプト名を確保
        string path = Path.Combine(_savePath, $"{_scriptName}.cs");

        File.WriteAllText(path, enumNames.ToString());
        AssetDatabase.Refresh();
        Debug.Log($"Enum を生成しました！　: {path}");
    }

    /// <summary>
    /// 拡張子が不適切かどうかを判定
    /// </summary>
    private bool IsInvalidEnumName(string fileName)
    {
        // 不適切なファイル名（拡張子を含むもの）をスキップ
        string[] invalidExtensions = new string[] { ".mp3", ".png", ".jpg", ".gif", ".bmp", ".tiff" };  // 拡張子リスト

        foreach (var ext in invalidExtensions)
        {
            if (fileName.ToLower().EndsWith(ext))
            {
                return true;  // 不適切な拡張子があればスキップ
            }
        }

        return false;
    }
    
    /// <summary>
    /// Enumの整形を行う
    /// </summary>
    private string ToEnumFormat(string fileName)
    {
        var formattedName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fileName.Replace("_", " ")).Replace(" ", string.Empty);
        return formattedName;
    }
}