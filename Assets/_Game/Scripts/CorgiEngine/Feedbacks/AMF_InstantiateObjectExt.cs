
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using Ameba.Editor;

namespace Ameba.MMFeedback {

  [AddComponentMenu("Ameba/Corgi Extensions/Feedbacks/Instantiate Object")]
  [FeedbackPath("Ameba/GameObject/Instantiate Object")]
  [FeedbackHelp("This feedback allows you to instantiate the object specified in its inspector, at the feedback's position (plus an optional Offset). You can also optionally use an object pool at initialization to save on performance. You can specify an existing pool or get one automatically created. If the latter, you'll need specify a pool size (usually the maximum amount of these instantiated objects you plan on having in your scene at each given time).")]
  [MMFHiddenProperties("CreateObjectPool", "ObjectPoolSize", "MutualizePools", "PoolParentTransform")]
  public class AMF_InstantiateObjectExt : MMF_InstantiateObject {

    public enum ObjectPoolType { None, AutoCreate, ReuseExisting }

    [MMFInspectorGroup("Object Pool", true, 40)]
    ///whether or not to use an object pool for this object
    [Tooltip("whether or not to use an object pool for this object")]
    public ObjectPoolType UseObjectPool = ObjectPoolType.None;

    /// the initial and planned size of this object pool
		[Tooltip("the initial and planned size of this object pool")]
    [MMFEnumCondition("UseObjectPool", (int)ObjectPoolType.AutoCreate)]
    //[LabelOverride("Object Pool Size")]
    public int ObjectPoolSize2 = 5;
    /// whether or not to create a new pool even if one already exists for that same prefab
    [Tooltip("whether or not to create a new pool even if one already exists for that same prefab")]
    [MMFEnumCondition("UseObjectPool", (int)ObjectPoolType.AutoCreate)]
    //[LabelOverride("Mutualize Pools")]
    public bool MutualizePools2 = false;
    /// the transform the pool of objects will be parented to
    [Tooltip("the transform the pool of objects will be parented to")]
    [MMFEnumCondition("UseObjectPool", (int)ObjectPoolType.AutoCreate)]
    //[LabelOverride("Pool Parent Transform")]
    public Transform PoolParentTransform2;

    [Tooltip("When using an object pool, specify an existent object pool")]
    [MMFEnumCondition("UseObjectPool", (int)ObjectPoolType.ReuseExisting)]
    public MMMiniObjectPooler ObjectPooler;

    //public MMMiniObjectPooler ObjectPooler {
    //  get => _objectPooler;
    //  set => _objectPooler = value;
    //}

    protected override void CustomInitialization(MMF_Player owner) {

      switch (UseObjectPool) {
        case ObjectPoolType.None:
          CreateObjectPool = false; break;

        case ObjectPoolType.AutoCreate:
          CreateObjectPool = true;
          ObjectPoolSize = ObjectPoolSize2;
          MutualizePools = MutualizePools2;
          PoolParentTransform = PoolParentTransform2;
          Debug.Log($"Creating object pool with size {ObjectPoolSize}");
          break;

        case ObjectPoolType.ReuseExisting:
          if (ObjectPooler != null) {
            // check the provided pooler is compatible with the object to instantiate
            if (!ObjectPooler.GameObjectToPool.GetType().Equals(GameObjectToInstantiate.GetType())) {
              Debug.LogWarning("[Object Instantiate Feedback] The provided Object Pool" + ObjectPooler.name + " is not setup to instantiate [" + GameObjectToInstantiate.GetType() + "] objects. The object pool settings will be ignored");
              break;
            }

            if (_objectPooler != null) {
              _objectPooler.DestroyObjectPool();
              owner.ProxyDestroy(_objectPooler.gameObject);
            }

            // set the private object pooler property with the provided pooler.
            _objectPooler = ObjectPooler;

            // retrofill the parent object pool properties to ensure backwards compatibility
            CreateObjectPool = true;
            ObjectPoolSize = _objectPooler.PoolSize;
            MutualizePools = _objectPooler.MutualizeWaitingPools;
            if (_objectPooler.transform.parent != null) {
              PoolParentTransform = _objectPooler.transform.parent;
            }
            // these lines were copied from base.CustomInitialization(owner) to ensure the pool is created or found before calling the base method.
            //_objectPooler.FillObjectPool();
            //if ((Owner != null) && (_objectPooler.gameObject.transform.parent == null)) {
            //  SceneManager.MoveGameObjectToScene(_objectPooler.gameObject, Owner.gameObject.scene);
            //}
            _poolCreatedOrFound = true;
          }
          break;
      }
      base.CustomInitialization(owner);
    }

  }
}