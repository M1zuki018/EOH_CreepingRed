using UnityEditor;
using UnityEngine;

/// <summary>
/// 変数名の代わりに任意のコメントを表示する
/// </summary>
[CustomPropertyDrawer(typeof(CommentAttribute))]
public class CommentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // CommentAttributeを取得
        CommentAttribute commentAttribute = (CommentAttribute)attribute;

        // 全体を2つの領域に分割
        float labelWidth = EditorGUIUtility.labelWidth; // デフォルトのラベル幅
        Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
        Rect fieldRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

        // ラベルとプロパティを描画
        EditorGUI.LabelField(labelRect, commentAttribute.Comment);
        EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}