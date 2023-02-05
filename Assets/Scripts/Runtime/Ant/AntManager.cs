using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class AntManager : MonoBehaviour
    {
        public AntBehaviour antPrefab;
        public List<AntBehaviour> ants;

        public void Start()
        {
            foreach (var ant in ants)
            {
                ant.Initialize(Vector3Int.zero, AntBehaviour.OffsetRotation.DOWN_RIGHT);
                Debug.Log("INITING ANT");
            }
        }

        public void Turn()
        {
            foreach (var ant in ants)
            {
                ant.Turn();
            }
        }
    }
}