
using System.Collections;
using System.Collections.Generic;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class AntBehaviour : MonoBehaviour
    {
        [SerializeField] private MapKeeper mapKeeper;

        [SerializeField] private MushroomManager mushroomManager;

        public Grid grid;
        
        [SerializeField] private Vector3Int anthillPosition;

        [SerializeField] private LinkedList<Vector3Int> trailPrediction = new LinkedList<Vector3Int>();

        private IEnumerator<Vector3Int> pathCreator;

        public int mapSize = 10;
        
        public OffsetRotation offsetRotation;
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

        private int repetition = 1;
        private int j = 0;
        private int simulateSteps = 50;
        
        public void Start()
        {
            pathCreator = PathCreator(new AntBehaviourPredictionState()
            {
                cubePos = this.anthillPosition,
                rotation = offsetRotation
            });
            IEnumerator patheveryframe = CreatePathEveryFrame(pathCreator);
            StartCoroutine(patheveryframe);
        }

        public IEnumerator CreatePathEveryFrame(IEnumerator pathGen)
        {
            while (true)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pathGen.MoveNext();
                g.transform.position = grid.CellToWorld(((Vector3Int) pathGen.Current).CubeToOffset());
                    yield return new WaitForSeconds(0.3f);
            }
        }


        private int dontDie = 0;
        
        public IEnumerator<Vector3Int> PathCreator(AntBehaviourPredictionState predictionState)
        {
            Vector3Int lastPos = predictionState.cubePos;
            OffsetRotation offsetRotation = predictionState.rotation;
            while (true)
            {
                int rotDir = 1;
                for (int i = 0; i < 5; i++)
                {
                    for (int phases = 0; phases < 3; phases++)
                    {
                        for (int rep = 0; rep < repetition; rep++)
                        {
                            lastPos += offsets[(int) offsetRotation];
                            anthillPosition = lastPos;
                            if(dontDie++ > 10000)
                                yield break;
                            if (Vector3Int.zero.CubeDistance(lastPos) >= mapSize - 1)
                            {
                                offsetRotation = RotateDir(offsetRotation, 2);
                                i = 0;
                                phases = 0;
                                rep = 0;
                                repetition = 1;
                                break;
                            }
                            yield return lastPos;
                        }
                        offsetRotation = RotateDir(offsetRotation, rotDir);
                    }
                    rotDir *= -1;
                    repetition++;
                }
            }
        }

        public struct AntBehaviourPredictionState
        {
            public Vector3Int cubePos;
            public OffsetRotation rotation;
        }
    }
}
