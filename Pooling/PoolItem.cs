using System;
using UnityEngine;

public class PoolItem : MonoBehaviour 
{
    public event Action OnReturnRequested;

    public void ReturnToPool()
    {
        bool failed = false;
        if(OnReturnRequested == null)
            failed = true;
        
        if(!failed)
        {
            try { OnReturnRequested.Invoke(); }
            catch (Exception) { failed = true; }

        }
        if(failed)
            Destroy(gameObject);
    }

    public void ResetEvent()
        => OnReturnRequested = null;
}
