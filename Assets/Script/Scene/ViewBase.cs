using UnityEngine;

/// <summary>
/// シーン基盤
/// </summary>
public abstract class ViewBase : MonoBehaviour
{
    public abstract void OnAwake();
    public abstract void OnStart();
}