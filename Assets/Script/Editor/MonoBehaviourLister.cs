using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.TypeCache;

/// <summary>
/// MonoBehaviour使用クラスのリストを表示するWindow
/// </summary>
public class MonoBehaviourLister : EditorWindow
{
    private Vector2 _scrollPosition;
    private List<Type> _monoBehaviourTypes;
    
    [MenuItem("Creeping Red/MonoBehaviour List")]
    public static void ShowWindow()
    {
        MonoBehaviourLister window = GetWindow<MonoBehaviourLister>("MonoBehaviour List");
        window.LoadTypes();
    }

    /// <summary>
    /// 自作クラスに絞り込んでリストを作成する
    /// </summary>
    private void LoadTypes()
    {
        _monoBehaviourTypes = GetTypesDerivedFrom<MonoBehaviour>()
            .Where(type => type.Assembly.GetName().Name == "Assembly-CSharp")
            .ToList();
        Repaint(); // 画面更新
    }
    
    private void OnGUI()
    {
        GUILayout.Label("MonoBehaviour Classes", EditorStyles.boldLabel);

        if (_monoBehaviourTypes == null || _monoBehaviourTypes.Count == 0)
        {
            GUILayout.Label("MonoBehaviorを継承したクラスが見つかりませんでした");
            if (GUILayout.Button("Refresh List"))
            {
                LoadTypes();
            }
            return;
        }
        
        if (GUILayout.Button("Refresh List"))
        {
            LoadTypes();
        }

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(400));
        foreach (var type in _monoBehaviourTypes)
        {
            GUILayout.Label(type.FullName, EditorStyles.label);
        }
        GUILayout.EndScrollView();
    }
}