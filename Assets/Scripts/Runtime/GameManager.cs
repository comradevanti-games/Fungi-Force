using System.Collections;
using TeamShrimp.GGJ23.Networking;
using UnityEngine;
using UnityEngine.Events;

namespace TeamShrimp.GGJ23
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent onGameStarted;
        [SerializeField] private UnityEvent onRoundStarted;

        private Team currentTeam = Team.Red;

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
            EndRound();
        }

        private void StartGame()
        {
            Debug.Log("Start game");
            onGameStarted.Invoke();

            if (Blackboard.IsHost)
                StartRound();
        }

        private void EndOpponentRound()
        {
            SwitchTeam();
            StartRound();
        }

        private void StartRound()
        {
            Debug.Log($"Round started for {currentTeam}");
            onRoundStarted.Invoke();
        }

        private void EndRound()
        {
            Debug.Log($"Round ended for{currentTeam}");
            SwitchTeam();
        }

        private void SwitchTeam()
        {
            Debug.Log("Switched team");
            currentTeam = currentTeam == Team.Red ? Team.Blue : Team.Red;
        }
    }
}