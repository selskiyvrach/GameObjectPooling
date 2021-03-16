# GameObjectPooling

Overview
  
  Simple pooling system for early prototypes.
  
Features
    
    - game object clones
        pool creates copies of 'sample' game object, passed as a parameter
    - type-safety. 
        the class is generic. it's constrained to Component subtypes only
    - neat runtime hierarchy appearance. 
        all pools reside under runtime created "Pooling" gameObject. 
        the pools are also packed under parent objects with concrete pools' names
    - clear runtime hierarcy appearance. 
        all pools' names consist from pool's order of creation appended with 
        the type's name upon which this pool instance was created.
    - error-safety. 
        Constructor's parameter 'Sample' is being checked for not having gameObject
        property - a case, where MonoBehaviour subtype instance is being created via class 
        constructor by mistake.
    - further hierarchy cleaniness - if failed to return to pool, pool item destroys itself

API

    - call GetPool to get fresh new pool of requested game object copies of requested quantity
    
    - call ClearPool to destroy all it's items
    
    - call DestroyPool to clear this pool and also destroy it's parent object
    
    - get PoolItem component on a pool item you don't need anymore and call ReturnToPool on it
      Item will autonavigate to the pool it belongs to or if failed will destroy itself

Have fun! Ivan Kryuchkov
