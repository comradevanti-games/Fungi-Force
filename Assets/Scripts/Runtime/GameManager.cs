using System.Collections;
using TeamShrimp.GGJ23.Networking;
using TeamShrimp.GGJ23.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TeamShrimp.GGJ23
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent onGameStarted;
        [SerializeField] private UnityEvent onRoundStarted;
        [SerializeField] private UnityEvent<Team> onTeamChanged;

        [SerializeField] private Resource startResourceType;
        [SerializeField] private float startResourceValue;
        
        public Team currentTeam = Team.Red;


        public Team MyTeam => Blackboard.IsHost ? Team.Red : Team.Blue;

        public Team OpponentTeam => Blackboard.IsHost ? Team.Blue : Team.Red;

        public Team CurrentTeam
        {
            get => currentTeam;
            private set
            {
                currentTeam = value;
                onTeamChanged.Invoke(value);
            }
        }

        public bool IsMyTurn => CurrentTeam == MyTeam;


        private void Start()
        {
            IEnumerator WaitAndStart()
            {
                yield return new WaitForSeconds(2); // Wait for map to generate
                StartGame();
            }

            StartCoroutine(WaitAndStart());
        }
        
        public void OnNetworkCommand(BaseCommand cmd)
        {
            switch (cmd)
            {
                case PlaceCommand _:
                case CutCommand _:
                    EndOpponentRound();
                    break;
            }
        }

        public void OnPlayerAction()
        {
            EndMyRound();
        }

        private void StartGame()
        {
            Debug.Log("Start game");
            ResourceTracker.Set(startResourceType, startResourceValue);
            Debug.Log("Player starts with " +
                      ResourceTracker.ResourceToString(startResourceType));
            onGameStarted.Invoke();

            if (Blackboard.IsHost)
                StartMyRound();
            else
                StartOpponentTurn();
        }

        private void StartOpponentTurn()
        {
            CurrentTeam = OpponentTeam;
        }

        private void EndOpponentRound()
        {
            StartMyRound();
        }

        private void StartMyRound()
        {
            CurrentTeam = MyTeam;
            onRoundStarted.Invoke();
        }

        private void EndMyRound()
        {
            StartOpponentTurn();
        }
    }
}