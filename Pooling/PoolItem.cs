using System;
using UnityEngine;

public class PoolItem : MonoBehaviour 
{
    public event Action OnReturnRequested;

    public void ReturnToPool()
    {
        if(OnReturnRequested != null)
            OnReturnRequested.Invoke();
        else 
            Destroy(gameObject);
    }

    public void ResetEvent()
        => OnReturnRequested = null;
}
