using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class AntManager : MonoBehaviour
    {
        public AntBehaviour antPrefab;
        public List<AntBehaviour> ants;
        private MushroomManager mushroomManager;
        private MapKeeper mapKeeper;
        public Grid grid;
        public void Start()
        {
            ants.Add(GameObject.Instantiate(antPrefab));
            mapKeeper = GetComponent<MapKeeper>();
            mushroomManager = GetComponent<MushroomManager>();
            foreach (var ant in ants)
            {
                ant.Initialize(Vector3Int.zero, AntBehaviour.OffsetRotation.DOWN_RIGHT,mapKeeper, mushroomManager, grid);
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