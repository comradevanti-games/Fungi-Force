using System.Collections;
using System.Collections.Generic;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class TurretShroom : ShroomBase
    {

        public enum Direction
        {
            LEFT = -1,
            RIGHT = 1
        }

        [SerializeField] private GameObject baitPrefab;

        [SerializeField] private int shootDistance;

        [SerializeField] private int cooldown;

        private Direction shootDirection;
        
        private int turnCount;

        private Vector3Int cubeDirection;
        
        public override void Start()
        {
            SetShootDirection(Owner == Team.Red ? Direction.LEFT : Direction.RIGHT);
        }

        public override void Update()
        {
        }

        public void ShootBait(MapKeeper map)
        {
            if (turnCount < 0)
            {
                Vector3 spawnPos = map.WorldToGridPos(transform.position).OffsetToCube() +
                                   (cubeDirection * shootDistance);
                GameObject gameObject = Instantiate(baitPrefab, spawnPos, baitPrefab.transform.rotation);
                turnCount = 0;
            }
            else
            {
                turnCount++;
                if (turnCount > cooldown)
                    turnCount = -1;
            }
        }

        public void SetShootDirection(Direction direction)
        {
            shootDirection = direction;
            cubeDirection = new Vector3Int((int) shootDirection, 0, (int) shootDirection * -1);
        }
    }
}
