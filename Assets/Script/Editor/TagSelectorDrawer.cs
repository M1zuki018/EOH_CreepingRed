using UnityEditor;
using UnityEngine;

/// <summary>
/// タグをポップアップから選択できるようにする（string型のみ）
/// </summary>
[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label);
            string currentTag = property.stringValue; //現在の値を取得
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags; //タグのリストを取得
            
            //プルダウンメニューを作成
            int index = Mathf.Max(0, System.Array.IndexOf(tags, currentTag));
            index = EditorGUI.Popup(
                new Rect(position.x + EditorGUIUtility.labelWidth, position.y , position.width - EditorGUIUtility.labelWidth, position.height),
                index,
                tags
            );
            
            property.stringValue = tags[index]; //選択されたタグをフィールドに反映
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}