using System;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Creates a pool of requested gameObject clones</summary>
public class ObjectPool<T> where T: Component
{
    private Stack<T> _pool = new Stack<T>();
    private T _sample;
    private Transform _poolParent;
    
    // TRACKERS
    // for hierarchy naming
    private static int _poolsCreated = -1;

    ///<summary>
    ///Creates a pool of requested gameObject clones with requested quantity, <br/> 
    ///cpecial name might be provided for cleaner hierarchy</summary>
    public ObjectPool(T sample, int quantity, string specName = null)
    {
        // VALIDATION
        if(sample == null) { Debug.LogError("Cannot create a pool of null objects"); return; }
        quantity = Mathf.Max(quantity, 0); 
        try 
        { 
            // checks if the class instance was created via constructor and thereby missing MonoBehaviour related references
            if(((Component)sample).gameObject == null)
                throw new Exception();
        }
        catch (Exception) 
        { 
            Debug.LogError("You shouldn't create MonoBehaviour subtypes via contructor! Use AddComponent or Instantiate instead");
            return;
        }
        // ENDVAL

        string number = _poolsCreated++ == 0 ? "" : $"({_poolsCreated})";
        // TODO: figure out why Image's ReflectedType call throws null reference exc
        string specialName = specName == null ? $"of {sample.GetType().ToString()}'s" : specName;
        _poolParent = new GameObject($"Pool {number} {specialName}").transform;

        // master object for all pools. get's created via non-generic class since different T realizations don't share static fields
        _poolParent.SetParent(PoolingMasterObject.PoolingParent.transform);

        _sample = GetNewItem(sample);
        
        Prewarm(quantity);
    }

    // returns an intem from pool
    public T Pop()
    {
        if(_pool.Count == 0)
            Prewarm(1);
        var i = _pool.Pop();
        i.transform.SetParent(null);
        i.gameObject.SetActive(true);
        return i;
    }
    
    // generater new instances of a pooled sample
    public void Prewarm(int quantity)
    {
        for(int i = 0; i < quantity; i++)
            GetNewItem(_sample);
    }

    // use this or GetComponent<PoolItem>.ReturnToPool on an item you want to return 
    public void ReturnItem(T item)
        => PutIntoPool(item);

// CLEARING

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

    // adds a PoolItem class which ReturnToPool method might be used by a comsumer if it doesn't have a pool reference
    private void SetUpPoolItem(T item)
    {
        var pI = item.GetComponent<PoolItem>();
        pI ??= item.gameObject.AddComponent<PoolItem>();
        pI.ResetEvent();
        pI.OnReturnRequested += () => ReturnItem(item);
    }
}