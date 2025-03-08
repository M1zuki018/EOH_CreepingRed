using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Enum生成用の静的補助クラス
/// </summary>
public static class EnumGenerator
{
    // フォルダ内のファイル名からEnumを生成する
    public static void GenerateEnum(string folderPath, string scriptName, string savePath)
    {
        if (string.IsNullOrEmpty(scriptName) || string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("スクリプトの名前かEnumを生成したいファイルの名前が空です");
            return;
        }

        // フォルダ内の全てのファイルを取得
        string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        
        // Enum名を構築
        StringBuilder enumNames = new StringBuilder();
        enumNames.AppendLine($"public enum {scriptName} {{");

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
        string path = Path.Combine(savePath, $"{scriptName}.cs");

        File.WriteAllText(path, enumNames.ToString());
        AssetDatabase.Refresh();
        Debug.Log($"Enum を生成しました！　: {path}");
    }

    /// <summary>
    /// 拡張子が不適切かどうかを判定
    /// </summary>
    private static bool IsInvalidEnumName(string fileName)
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
    private static string ToEnumFormat(string fileName)
    {
        var formattedName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fileName.Replace("_", " ")).Replace(" ", string.Empty);
        return formattedName;
    }
}
