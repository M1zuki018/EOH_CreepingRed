using System;
using UnityEngine;

/// <summary>
/// シーン基盤
/// </summary>
public abstract class ViewBase : MonoBehaviour
{
    public virtual void OnAwake(){}
    public virtual void OnUIInitialize(){}
    public virtual void OnStart(){}
}