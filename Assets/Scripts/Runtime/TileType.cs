using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamShrimp.GGJ23
{
    [CreateAssetMenu(menuName = "GGJ23/Tile-type")]
    public class TileType : ScriptableObject
    {
        [SerializeField] private float weight;
        [SerializeField] private Tile[] variants;


        public IReadOnlyCollection<Tile> Variants => variants;
    }
}