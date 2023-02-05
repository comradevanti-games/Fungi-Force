using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class AntBehaviour : MonoBehaviour
    {
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

        public bool debug;

        [SerializeField] private MapKeeper mapKeeper;

        [SerializeField] private MushroomManager mushroomManager;

        public Grid grid;

        [SerializeField] private Vector3Int anthillPosition;

        [SerializeField] public Vector3Int currentCubePosition;
        public int mapSize = 10;

        public OffsetRotation offsetRotation;


        public byte hp = 3;

        private readonly List<Vector3Int> currentPredictionCubeCoords =
            new List<Vector3Int>();

        private int dontDie;
        private int j = 0;

        private LineRenderer lr;


        private readonly List<Vector3Int> offsets = new List<Vector3Int>
        {
            new Vector3Int(1, -1), // UP RIGHT
            new Vector3Int(1, 0, -1), // RIGHT
            new Vector3Int(0, 1, -1), // RIGHT DOWN
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, +1),
            new Vector3Int(0, -1, 1) // UP LEFT
        };

        private IEnumerator<AntBehaviourPredictionState> pathCreator;

        private IEnumerator patheveryframe;

        private int repetition = 1;
        private int simulateSteps = 50;

        [SerializeField]
        private LinkedList<Vector3Int> trailPrediction =
            new LinkedList<Vector3Int>();

        public void Start()
        {
            lr = GetComponent<LineRenderer>();
            pathCreator = PathCreator(new AntBehaviourPredictionState
            {
                cubePos = anthillPosition,
                rotation = offsetRotation
            });
            mapSize = Blackboard.Game.MapSize;

            patheveryframe = CreatePathEveryFrame(pathCreator);
            InvokeRepeating(nameof(Turn), 1f, 1f);
        }

        public OffsetRotation RotateDir(OffsetRotation rotation, int rot)
        {
            rotation += rot;
            if (rotation < 0) rotation += 6;

            rotation = (OffsetRotation) ((int) rotation % 6);

            return rotation;
        }

        public IEnumerator AnimatePositionTo(
            Vector3 start, Vector3 end, float time, int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                transform.position =
                    Vector3.Lerp(start, end, i / (float) steps);
                yield return new WaitForSeconds(time / steps);
            }

            transform.position = end;
        }

        public IEnumerator AnimateScaleTo(
            Vector3 start, Vector3 end, float time, int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                transform.localScale =
                    Vector3.Lerp(start, end, i / (float) steps);
                yield return new WaitForSeconds(time / steps);
            }

            transform.localScale = end;
        }

        public Vector3Int? FindClosebyMushroom(
            Vector3Int pos, OffsetRotation rotation)
        {
            var sb = mapKeeper.AllShrooms.Where(b =>
                b.ShroomPosition.To3Int().OffsetToCube().CubeDistance(pos) <=
                1).ToList();
            if (sb.Count == 0) return null;

            Debug.Log("FOUND CLOSEBY MUSHROOMS, STEALING ONE");
            foreach (var shroomBase in sb)
            {
                var nextSpace = pos + offsets[(int) rotation];
                if (nextSpace.Equals(shroomBase.ShroomPosition.To3Int()
                        .OffsetToCube()))
                {
                    KillShroom(shroomBase);
                    return nextSpace;
                }
            }

            sb.Sort((base1, base2) =>
            {
                return mushroomManager.FindAllShroomConnectionsInvolving(base1)
                           .Count >
                       mushroomManager.FindAllShroomConnectionsInvolving(base2)
                           .Count
                    ? 1
                    : -1;
            });
            var next = sb[0].ShroomPosition.To3Int().OffsetToCube();
            KillShroom(sb[0]);
            return next;
        }

        public void KillShroom(ShroomBase shroomBase)
        {
//            Debug.Log("Shroom '" + shroomBase +"' should be killed here, but not implemented");
            Destroy(shroomBase.gameObject);
            Debug.Log("KILLING SHROOM  " + shroomBase);
        }

        public void LoadPositionFromCubePos()
        {
            var newPos = grid.CellToWorld(currentCubePosition.CubeToOffset());
            var ienum = AnimatePositionTo(transform.position, newPos, 0.3f, 5);
            var ienum2 = AnimateScaleTo(transform.localScale,
                new Vector3(transform.position.x < newPos.x ? -0.1f : 0.1f,
                    0.1f, 0.1f), 0.1f, 3);
            StartCoroutine(ienum2);
            StartCoroutine(ienum);
        }

        public void Turn()
        {
            patheveryframe.MoveNext();
            LoadPositionFromCubePos();
        }


        public IEnumerator CreatePathEveryFrame(IEnumerator pathGen)
        {
            while (true)
            {
                AntBehaviourPredictionState abps;
                {
                    //GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    pathGen.MoveNext();
                    abps = (AntBehaviourPredictionState) pathGen.Current;
                    currentCubePosition = abps.cubePos;
                    //g.transform.position = grid.CellToWorld(((Vector3Int) abps.cubePos).CubeToOffset());
                    abps.loadPrediction = true;
                }

                {
                    var positionsPredicted = new List<Vector3>();
                    IEnumerator ienum = PathCreator(abps);

                    currentPredictionCubeCoords.Clear();
                    for (var i = 0; i < 5; i++)
                    {
                        if (!ienum.MoveNext())
                        {
                            Debug.LogError(
                                "NO NEXT MOVE ON MOVE SIMULATOR. DROPPING SIM, THIS SHOULD NEVER HAPPEN");
                            break;
                        }

                        var predState =
                            (AntBehaviourPredictionState) ienum.Current;
                        currentPredictionCubeCoords.Add(predState.cubePos);
                        positionsPredicted.Add(
                            grid.CellToWorld(predState.cubePos.CubeToOffset()));
                    }

                    if (debug) lr.SetPositions(positionsPredicted.ToArray());
                }
                yield return null;
            }
        }


        public IEnumerator<AntBehaviourPredictionState> PathCreator(
            AntBehaviourPredictionState predictionState)
        {
            var repetitionInternal = 1;
            var lastPos = predictionState.cubePos;
            var internalOffsetRotation = predictionState.rotation;
            var rotDir = 1;
            while (true)
            {
                for (var phases = 0; phases < 3; phases++)
                {
                    for (var rep = 0; rep < repetitionInternal; rep++)
                    {
                        if (predictionState.loadPrediction)
                        {
                            phases = predictionState.phases;
                            rep = predictionState.rep;
                            repetitionInternal = predictionState.repetitions;
                            rotDir = predictionState.rotDir;
                            predictionState.loadPrediction = false;
                            if (rep >= repetitionInternal)
                                break;
                        }

                        var vector3Int = FindClosebyMushroom(lastPos,
                            internalOffsetRotation);
                        if (vector3Int.HasValue)
                        {
                            var off = vector3Int.Value - lastPos;
                            internalOffsetRotation =
                                (OffsetRotation) offsets.FindIndex(i =>
                                    i.Equals(off));
                            lastPos = vector3Int.Value;
                        }
                        else
                        {
                            lastPos += offsets[(int) internalOffsetRotation];
                        }

                        // anthillPosition = lastPos;
                        if (dontDie++ > 1000)
                            yield break;
                        if (Vector3Int.zero.CubeDistance(lastPos) >=
                            mapSize - 1)
                        {
                            internalOffsetRotation =
                                RotateDir(internalOffsetRotation, 2);
                            phases = 0;
                            repetitionInternal = 1;
                            break;
                        }

                        yield return new AntBehaviourPredictionState
                        {
                            loadPrediction = false,
                            cubePos = lastPos,
                            phases = phases,
                            rep = rep + 1,
                            repetitions = repetitionInternal,
                            rotation = internalOffsetRotation,
                            rotDir = rotDir
                        };
                    }

                    internalOffsetRotation =
                        RotateDir(internalOffsetRotation, rotDir);
                }

                rotDir *= -1;
                repetitionInternal++;
            }
        }

        public struct AntBehaviourPredictionState
        {
            public Vector3Int cubePos;
            public OffsetRotation rotation;
            public bool loadPrediction;
            public int phases;
            public int rep;
            public int repetitions;
            public int rotDir;
        }
    }
}