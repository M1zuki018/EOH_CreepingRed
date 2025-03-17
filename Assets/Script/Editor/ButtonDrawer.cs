using UnityEditor;
using UnityEngine;
using System.Reflection;

/// <summary>
/// インスペクターにボタンを表示し、メソッドを実行できるようにします
/// </summary>
[CustomEditor(typeof(MonoBehaviour), true)]
public class ButtonDrawer : Editor
{
    //動作を制御するフラグ
    private static bool isEnabled = true;
    
    public override void OnInspectorGUI()
    {
        if (!isEnabled)
        {
            DrawDefaultInspector();
            return;
        }
        
        // デフォルトのプロパティを描画
        DrawDefaultInspector();

        // 現在のターゲットオブジェクトを取得
        MonoBehaviour targetObject = (MonoBehaviour)target;
        MethodInfo[] methods = targetObject.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // [Button] 属性が付いたメソッドを探す
        foreach (MethodInfo method in methods)
        {
            ButtonAttribute buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttribute != null)
            {
                string buttonText = string.IsNullOrEmpty(buttonAttribute.Label) ? method.Name : buttonAttribute.Label;

                // ボタンを描画
                if (GUILayout.Button("MethodTest: " + buttonText))
                {
                    // メソッドを呼び出す
                    method.Invoke(targetObject, null);
                }
            }
        }
    }
}