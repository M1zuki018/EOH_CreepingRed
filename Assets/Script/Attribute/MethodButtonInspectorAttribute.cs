using UnityEngine;
using System;

/// <summary>
/// インスペクターにボタンを表示し、メソッドを実行できるようにする属性
/// （継承可能・同じメソッドに複数付けられない）
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class MethodButtonInspectorAttribute : PropertyAttribute
{
    public string Label { get; }

    public MethodButtonInspectorAttribute(string label = null)
    {
        Label = label;
    }
}