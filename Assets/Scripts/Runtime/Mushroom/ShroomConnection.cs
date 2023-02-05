using System.Collections;
using System.Collections.Generic;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class ShroomConnection : MonoBehaviour
    {

        [SerializeField] private LineRenderer line;

        private ShroomBase _start;

        private ShroomBase _end;

        private List<Vector3Int> _segments;

        private MapKeeper map;

        public ShroomBase StartShroom
        {
            get => _start;
            set
            {
                _start = value;
                // TODO Change line
            }
        }

        public ShroomBase EndShroom
        {
            get => _end;
            set
            {
                _end = value;
                // TODO Change line
            }
        }

        public Vector3Int[] Segments
        {
            get => _segments.ToArray();
        }

        public void Initialize(ShroomBase start, ShroomBase end, MapKeeper mapKeeper)
        {
            this._end = end;
            this._start = start;
            if (mapKeeper != null) this.map = mapKeeper;

            this.DrawSegments();
        }

        public void DrawSegments()
        {
            this._segments = this.map.GetLerpPathCubed(
                this.map.WorldToGridPos(_start.transform.position), this.map.WorldToGridPos(_end.transform.position)
            );

            this.line.positionCount = this._segments.Count;
            List<Vector3> worldPositions = new List<Vector3>();
            foreach (Vector3Int segment in _segments)
            {
                Vector3 worldPos = map.GridToWorldPos(segment.CubeToOffset());
                worldPos.z = 1f;
                worldPositions.Add(worldPos);
            }
            this.line.SetPositions(worldPositions.ToArray());
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
