using System.Collections.Generic;
using UnityEngine;

namespace GPHive.Game
{
    [System.Serializable]
    public class ObjectToPool
    {
        public string name;
        public GameObject objectToPool;
        public int poolCount;
        public int expandAmount;
        public Queue<GameObject> pooledObjects = new();
    }
}
