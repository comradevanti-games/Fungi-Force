using System.Collections.Generic;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class ShroomConnection : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;

        private List<Vector3Int> _segments;

        private MapKeeper map;

        public ShroomBase StartShroom
        {
            get;
            set;
            // TODO Change line
        }

        public ShroomBase EndShroom
        {
            get;
            set;
            // TODO Change line
        }

        public Vector3Int[] Segments => _segments.ToArray();

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
        }

        public void Initialize(
            ShroomBase start, ShroomBase end, MapKeeper mapKeeper)
        {
            EndShroom = end;
            StartShroom = start;
            if (mapKeeper != null) map = mapKeeper;

            DrawSegments();
        }

        public void DrawSegments()
        {
            _segments = map.GetLerpPathCubed(
                map.WorldToGridPos(StartShroom.WorldPosition).To3Int(),
                map.WorldToGridPos(EndShroom.WorldPosition).To3Int()
            );

            line.positionCount = _segments.Count;
            var worldPositions = new List<Vector3>();
            foreach (var segment in _segments)
            {
                var worldPos = map.GridToWorldPos(segment.CubeToOffset());
                worldPos.z = -1f;
                worldPositions.Add(worldPos);
            }

            line.SetPositions(worldPositions.ToArray());
        }
    }
}