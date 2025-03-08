using UnityEngine;
using System;

/// <summary>
/// inspectorからフォルダーを開き、パスを直接指定できるようにするカスタム属性
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class FolderPathAttribute : PropertyAttribute { }