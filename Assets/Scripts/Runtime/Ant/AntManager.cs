using System;
using System.Collections.Generic;
using System.Linq;
using TeamShrimp.GGJ23.Runtime.Util;
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

        public List<Vector3Int> baitPositions = new List<Vector3Int>();

        public static AntManager instance;
        
        public void Start()
        {
            instance = this;
            ants.Add(GameObject.Instantiate(antPrefab));
            mapKeeper = GetComponent<MapKeeper>();
            mushroomManager = GetComponent<MushroomManager>();
            foreach (var ant in ants)
            {
                ant.Initialize(Vector3Int.zero, AntBehaviour.OffsetRotation.DOWN_RIGHT,mapKeeper, mushroomManager, grid);
                Debug.Log("INITING ANT");
            }
            //InvokeRepeating("Turn", 0.5f, 0.5f);
        }

        public void Turn()
        {
            foreach (var ant in ants)
            {
                try
                {
                    ant.Turn();
                }
                catch
                {
                    Debug.Log("CANT DO ANT TURN YET");
                }
            }
        }

        public Vector3Int? CheckForBaitInRange(Vector3Int pos, List<Vector3Int> takenBaits)
        {
            var v = baitPositions.Where(bait => bait.CubeDistance(pos) <= 3 && bait.CubeDistance(pos) > 0).ToList();
            v.RemoveAll(match => takenBaits.Contains(match));
            v.Sort((i, i2) => i.CubeDistance(pos).CompareTo(i2.CubeDistance(pos)));
            if (v.Count == 0)
                return null;
            return v[0];
        }


        public void OnDrawGizmosSelected()
        {
            foreach (var bait in baitPositions)
            {
                Gizmos.DrawCube(grid.CellToWorld(bait.CubeToOffset()),Vector3.one/3f);
            }
        }

        public List<Vector3Int> GetRoute(Vector3Int cubestart, Vector3Int cubeend)
        { 
            return mapKeeper.GetLerpPathCubed(cubestart.CubeToOffset(), cubeend.CubeToOffset());
        }
    }
}