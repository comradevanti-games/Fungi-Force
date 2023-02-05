
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace TeamShrimp.GGJ23
{
    public class AntBehaviour : MonoBehaviour
    {
        public bool debug;
        
        [SerializeField] private MapKeeper mapKeeper;

        [SerializeField] private MushroomManager mushroomManager;

        public Grid grid;
        
        [SerializeField] private Vector3Int anthillPosition;

        [SerializeField] public Vector3Int currentCubePosition;
        
        [SerializeField] private LinkedList<Vector3Int> trailPrediction = new LinkedList<Vector3Int>();

        private IEnumerator<AntBehaviourPredictionState> pathCreator;

        private LineRenderer lr;
        public int mapSize = 10;
        
        public OffsetRotation offsetRotation;
        


        public byte hp = 3;
        
        

        private List<Vector3Int> offsets = new List<Vector3Int>()
        {
            new Vector3Int(0, 1, -1), // RIGHT DOWN
            new Vector3Int(1, 0, -1), // RIGHT
            new Vector3Int(1, -1), // UP RIGHT
            new Vector3Int(0, -1, 1), // UP LEFT
            new Vector3Int(-1, 0, +1),
            new Vector3Int(-1, 1, 0),
        };

        private List<Vector3Int> currentPredictionCubeCoords = new List<Vector3Int>();

        private IEnumerator patheveryframe;
        
        public OffsetRotation RotateDir(OffsetRotation rotation, int rot)
        {
            rotation += rot;
            if (rotation < 0)
            {
                rotation += 6;
            }

            rotation = (OffsetRotation)(((int)rotation) % 6);

            return (OffsetRotation) rotation;
        }

        public IEnumerator AnimatePositionTo(Vector3 start, Vector3 end, float time, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                transform.position = Vector3.Lerp(start, end, i/(float)steps);
                yield return new WaitForSeconds(time / steps);
            }
            transform.position = end;
        }
        public IEnumerator AnimateScaleTo(Vector3 start, Vector3 end, float time, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                transform.localScale = Vector3.Lerp(start, end, i/(float)steps);
                yield return new WaitForSeconds(time / steps);
            }
            transform.localScale = end;
        }

        private int repetition = 1;
        private int j = 0;
        private int simulateSteps = 50;

        public void Start()
        {
            lr = GetComponent<LineRenderer>();
            //InvokeRepeating("Turn", 0.5f, 0.5f);
        }

        public void Initialize(Vector3Int cubeStartPosition, OffsetRotation offsetRotationInit, MapKeeper mapKeeper, MushroomManager mushroomManager, Grid grid)
        {
            anthillPosition = cubeStartPosition;
            this.offsetRotation = offsetRotationInit;
            this.mapKeeper = mapKeeper;
            this.mushroomManager = mushroomManager;
            this.grid = grid;
            pathCreator = PathCreator(new AntBehaviourPredictionState()
            {
                cubePos = anthillPosition,
                rotation = offsetRotationInit
            });
            mapSize = Blackboard.Game.MapSize;
            
            patheveryframe = CreatePathEveryFrame(pathCreator);
            
            Debug.Log("INITIALIZED ANT");
            //InvokeRepeating(nameof(Turn), 1f, 1f);
        }

        public Vector3Int? FindClosebyMushroom(Vector3Int pos, OffsetRotation rotation, bool isSimulated, List<Vector3Int> ignore)
        {
            List<ShroomBase> sb = mapKeeper.AllShrooms.ToList().FindAll((ShroomBase b) => b.ShroomPosition.To3Int().OffsetToCube().CubeDistance(pos) ==1);
            foreach (var ig in ignore)
            {
                sb.RemoveAll(baseshroom => baseshroom.ShroomPosition.To3Int().OffsetToCube().Equals(ig));
            }
            if (sb.Count == 0)
            {
                return null;
            }

            Debug.Log("FOUND CLOSE BY MUSHROOMS, STEALING ONE");
            foreach (var shroomBase in sb)
            {
                Vector3Int nextSpace = pos + offsets[(int) rotation];
                if (nextSpace.Equals(shroomBase.ShroomPosition.To3Int().OffsetToCube()))
                {
                    if(!isSimulated)
                        KillShroom(shroomBase);
                    return nextSpace;
                }
            }
            
            sb.Sort((base1, base2) =>
            {
                return mushroomManager.FindAllShroomConnectionsInvolving(base1).Count >
                       mushroomManager.FindAllShroomConnectionsInvolving(base2).Count
                    ? 1
                    : -1;
            });
            Vector3Int next = sb[0].ShroomPosition.To3Int().OffsetToCube();
            if(!isSimulated)
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
            Vector3 newPos = grid.CellToWorld(currentCubePosition.CubeToOffset());
            IEnumerator ienum = AnimatePositionTo(transform.position, newPos, 0.2f, 5);
            float usualXScale = Mathf.Abs(transform.localScale.x);
            IEnumerator ienum2 = AnimateScaleTo(transform.localScale, new Vector3(transform.position.x < newPos.x ? -usualXScale : usualXScale,usualXScale, 0.1f), 0.2f, 3);
            StartCoroutine(ienum2);
            StartCoroutine(ienum);
        }

        public void Turn()
        {
            //currentCubePosition = currentCubePosition + offsets[(int) offsetRotation];
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
                    currentCubePosition = (Vector3Int) abps.cubePos;
                    //g.transform.position = grid.CellToWorld(((Vector3Int) abps.cubePos).CubeToOffset());
                    abps.loadPrediction = true;
                }

                {
                    List<Vector3> positionsPredicted = new List<Vector3>();
                    IEnumerator ienum = PathCreator(abps, true);

                    currentPredictionCubeCoords.Clear();
                    for (int i = 0; i < 30; i++)
                    {
                        if (!ienum.MoveNext())
                        {
                            Debug.LogError("NO NEXT MOVE ON MOVE SIMULATOR. DROPPING SIM, THIS SHOULD NEVER HAPPEN");
                            break;
                        }
                        AntBehaviourPredictionState predState = (AntBehaviourPredictionState) ienum.Current;
                        currentPredictionCubeCoords.Add(predState.cubePos);
                        positionsPredicted.Add(grid.CellToWorld(((Vector3Int) predState.cubePos).CubeToOffset()));
                    }
                    
                    if(debug) lr.SetPositions(positionsPredicted.ToArray()) ;
                }
                yield return null;
            }
        }


        public IEnumerator<AntBehaviourPredictionState> PathCreator(AntBehaviourPredictionState predictionState, bool isSimulation = false)
        {
            List<Vector3Int> stolenShrooms = new List<Vector3Int>();
            List<Vector3Int> takenBaits = new List<Vector3Int>();
            int dontDie = 0;
            var repetitionInternal = 1;
            Vector3Int lastPos = predictionState.cubePos;
            OffsetRotation internalOffsetRotation = predictionState.rotation;
            
            int rotDir = 1;
            while (true)
            {
                for (int phases = 0; phases < 3; phases++)
                {
                    for (int rep = 0; rep < repetitionInternal; rep++)
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

                        AntManager manager = AntManager.instance;
                        Vector3Int? baitPos = manager.CheckForBaitInRange(lastPos, takenBaits);
                        Vector3Int? vector3Int = FindClosebyMushroom(lastPos, internalOffsetRotation, isSimulation, stolenShrooms);
                        if (baitPos.HasValue)
                        {
                            if (!baitPos.Value.Equals(lastPos))
                            {
                                List<Vector3Int> route = manager.GetRoute(lastPos, baitPos.Value);
                                foreach (var pos in route)
                                {
                                    if (!pos.Equals(lastPos))
                                    {
                                        lastPos = pos;
                                        break;
                                    }
                                }
                                Debug.Log("BAITED! MOVING TO " + lastPos);
                            }
                            else
                            {
                                Debug.Log("ADDING TO TAKEN BAITS");
                                takenBaits.Add(lastPos);
                            }
                        }
                        else if (vector3Int.HasValue)
                        {
                            var off = vector3Int.Value - lastPos;
                            internalOffsetRotation = (OffsetRotation) offsets.FindIndex(i => i.Equals(off));
                            lastPos = vector3Int.Value;
                            if(isSimulation) stolenShrooms.Add(lastPos);
                        }
                        else
                            lastPos += offsets[(int) internalOffsetRotation];
                         
                        anthillPosition = lastPos;
                        if(dontDie++ > 100000)
                            yield break;
                        if (Vector3Int.zero.CubeDistance(lastPos) >= mapSize)
                        {
                            internalOffsetRotation = RotateDir(internalOffsetRotation, 2);
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
                    internalOffsetRotation = RotateDir(internalOffsetRotation, rotDir);
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
    }
}
