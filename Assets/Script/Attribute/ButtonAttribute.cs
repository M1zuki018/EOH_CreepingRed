using UnityEngine;
using System;

/// <summary>
/// インスペクターにボタンを表示し、メソッドを実行できるようにする属性
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public string Label { get; }

    public ButtonAttribute(string label = null)
    {
        Label = label;
    }
}