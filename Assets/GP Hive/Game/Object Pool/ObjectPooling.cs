using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPHive.Game
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {
        public ObjectToPool[] objectsToPool;

        public delegate void OnGetFromPool();

        public static event OnGetFromPool GotFromPool;

        void Awake()
        {
            InitializePool();
        }

        public GameObject GetFromPool(string name)
        {
            GameObject _objectToReturn = null;
            foreach (ObjectToPool pool in objectsToPool)
            {
                if (pool.name != name) continue;

                if (pool.pooledObjects.Count > 0)
                    return pool.pooledObjects.Dequeue();
                

                for (int i = 0; i < pool.expandAmount; i++)
                {
                    GameObject _pooled = Instantiate(pool.objectToPool, transform);
                    _pooled.name = pool.name;
                    _pooled.SetActive(false);
                    pool.pooledObjects.Enqueue(_pooled);
                }

                _objectToReturn = pool.pooledObjects.Dequeue();
                break;
            }

            GotFromPool?.Invoke();
            return _objectToReturn;
        }

        public void Deposit(GameObject gameObject)
        {
            foreach (ObjectToPool pool in objectsToPool)
            {
                if (pool.pooledObjects.Contains(gameObject))
                {
                    gameObject.transform.SetParent(transform);
                    gameObject.SetActive(false);
                    gameObject.transform.ResetTransform();

                    if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                        rigidbody.ResetVelocity();

                    return;
                }
            }

            Debug.LogError("Trying to deposit an object that isn't in the pool.");
        }

        public void Deposit(string poolName)
        {
            foreach (ObjectToPool pool in objectsToPool)
            {
                if (pool.name != poolName) continue;

                foreach (var pooledObject in pool.pooledObjects)
                {
                    pooledObject.transform.SetParent(transform);
                    pooledObject.SetActive(false);
                    pooledObject.transform.ResetTransform();

                    if (pooledObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                        rigidbody.ResetVelocity();
                }
            }
        }

        public void DepositAll()
        {
            foreach (ObjectToPool pool in objectsToPool)
            {
                foreach (GameObject gameObject in pool.pooledObjects)
                {
                    gameObject.transform.SetParent(transform);
                    gameObject.transform.ResetTransform();
                    gameObject.SetActive(false);

                    if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                        rigidbody.ResetVelocity();
                }
            }
        }

        public void Clear()
        {
            foreach (ObjectToPool pool in objectsToPool)
            {
                foreach (GameObject gameObject in pool.pooledObjects)
                {
                    Destroy(gameObject);
                }

                pool.pooledObjects.Clear();
            }
        }

        private void InitializePool()
        {
            foreach (ObjectToPool pool in objectsToPool)
            {
                for (int i = 0; i < pool.poolCount; i++)
                {
                    GameObject _pooled = Instantiate(pool.objectToPool, transform);
                    _pooled.name = pool.name;
                    _pooled.SetActive(false);
                    pool.pooledObjects.Enqueue(_pooled);
                }
            }
        }
    }
}