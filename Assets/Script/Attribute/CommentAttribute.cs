using UnityEngine;
using System;

/// <summary>
/// 変数名を書き換えるカスタム属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class CommentAttribute : PropertyAttribute
{
    public string Comment { get; }

    public CommentAttribute(string comment)
    {
        Comment = comment;
    }
}