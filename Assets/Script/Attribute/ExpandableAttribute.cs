using System;
using UnityEngine;

/// <summary>
/// ScriptableObject を Inspector 上で展開して編集できるようにする属性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ExpandableAttribute : PropertyAttribute { }