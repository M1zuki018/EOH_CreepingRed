using System;
using UnityEngine;

/// <summary>
/// Layer同様にタグもプルダウンから選択できるようにする属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class TagSelectorAttribute : PropertyAttribute { }