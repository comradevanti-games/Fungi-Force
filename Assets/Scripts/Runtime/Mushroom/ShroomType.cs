using UnityEngine;

namespace TeamShrimp.GGJ23
{
    [CreateAssetMenu(menuName = "GGJ23/Shroom-type")]
    public class ShroomType : ScriptableObject
    {
        [SerializeField] private int id;

        public int Id => id;
    }
}