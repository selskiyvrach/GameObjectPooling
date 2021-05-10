using System;
using UnityEngine;

public class PoolItem : MonoBehaviour 
{
    public event Action OnReturnRequested;

    public void ReturnToPool()
    {
        try { OnReturnRequested.Invoke(); }
        catch { Destroy(gameObject); }
    }

    public void ResetEvent()
        => OnReturnRequested = null;
}
