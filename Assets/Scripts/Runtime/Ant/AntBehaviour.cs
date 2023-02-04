using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace TeamShrimp.GGJ23
{
    public class AntBehaviour : MonoBehaviour
    {
        [SerializeField] private MapKeeper mapKeeper;

        [SerializeField] private MushroomManager mushroomManager;

        [SerializeField] private Vector3Int anthillPosition;

        [SerializeField] private LinkedList<Vector3Int> trailPrediction = new LinkedList<Vector3Int>();

        public enum AntBehaviourState
        {
            SEARCHING,
            ATTRACTED,
            WALKING_STRAIGHT
        }
        
        public enum OffsetRotation
        {
            UP_RIGHT,
            RIGHT,
            DOWN_RIGHT,
            DOWN_LEFT,
            LEFT,
            UP_LEFT
        }

        private List<Vector3Int> offsets = new List<Vector3Int>()
        {
            new Vector3Int(1, -1), // UP RIGHT
            new Vector3Int(1, 0, -1), // RIGHT
            new Vector3Int(0, 1, -1), // RIGHT DOWN
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, +1),
            new Vector3Int(0, -1, 1) // UP LEFT
        };
        
        
        
        public void Start()
        {
            
        }

        public void GetNextStep()
        {
            
        }
    }
}
