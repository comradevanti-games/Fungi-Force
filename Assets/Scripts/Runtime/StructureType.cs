using UnityEngine;

namespace TeamShrimp.GGJ23
{
    [CreateAssetMenu(menuName = "GGJ23/Structures")]
    public class StructureType : ScriptableObject
    {
        [SerializeField] private GameObject prefab;


        public GameObject Prefab => prefab;
    }
}