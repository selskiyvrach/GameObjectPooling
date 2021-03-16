using UnityEngine;


public static class PoolingMasterObject
{
    private static GameObject _poolingParentHolder = null;
    public static GameObject PoolingParent 
        => _poolingParentHolder != null ? _poolingParentHolder : _poolingParentHolder = new GameObject("Object Pooling");
}