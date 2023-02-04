using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public abstract class StructureType : ScriptableObject
    {
        [SerializeField] private GameObject prefab;


        public GameObject Prefab => prefab;
    }
}