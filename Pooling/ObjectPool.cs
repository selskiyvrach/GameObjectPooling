using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T: Component
{
    private Stack<T> _pool = new Stack<T>();
    private T _sample;
    private GameObject _poolParent;
    
    // TRACKERS
    private static int _poolsCreated;

// CONSTRUCTOR

    public ObjectPool(T sample, int quantity)
    {
        // VALIDATION
        if(sample == null) { Debug.LogError("Cannot create pool of null objects"); return; }
        bool failed = false;
        try 
        { 
            if (((Component)sample).gameObject == null) 
                failed = true;
        }
        catch (Exception) 
        { 
            failed = true;  
        }
        if (failed) 
        {
            Debug.LogError("You shouldn't create MonobeHaviour subtypes via contructor! Use gameObject.AddComponent instead");
            return;
        }
        quantity = Mathf.Max(quantity, 0); // ENDVAL

        _poolParent = new GameObject($"Object Pool ({_poolsCreated++}) of {sample.ToString()}'s");
        _poolParent.transform.SetParent(PoolingMasterObject.PoolingParent.transform);
        _sample = GetNewItem(sample);
        Prewarm(quantity);
    }

// PUBLIC

    public T Pop()
    {
        if(_pool.Count == 0)
            Prewarm(1);
        var i = _pool.Pop();
        i.transform.SetParent(null);
        i.gameObject.SetActive(true);
        return i;
    }

    public void Prewarm(int quantity)
    {
        for(int i = 0; i < quantity; i++)
            GetNewItem(_sample);
    }

    public void ClearPool()
    {
        foreach(var i in _pool)
            GameObject.Destroy(i.gameObject);
        _pool.Clear();
    }

    public void DestroyPool()
    {
        ClearPool();
        GameObject.Destroy(_poolParent);
    }

// USED BY POOLITEM. YOU DO NOT HAVE TO USE IT

    public void ReturnItem(T item)
        => PutIntoPool(item);

// PRIVATE

    private T GetNewItem(T sample)
    {
        var i = GameObject.Instantiate(sample);
        PutIntoPool(i);
        return i;
    }

    private void PutIntoPool(T item)
    {
        item.transform.SetParent(_poolParent.transform);
        item.transform.position = _poolParent.transform.position;
        SetUpPoolItem(item);
        item.gameObject.SetActive(false);
        _pool.Push(item);
    }

    private void SetUpPoolItem(T item)
    {
        var pI = item.GetComponent<PoolItem>();
        if (pI == null)
            pI = item.gameObject.AddComponent<PoolItem>();
        pI.ResetEvent();
        pI.OnReturnRequested += () => ReturnItem(item);
    }
}