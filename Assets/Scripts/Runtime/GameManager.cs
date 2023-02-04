using System.Collections;
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

        private void StartRound()
        {
            Debug.Log($"Round started for {currentTeam}");
            onRoundStarted.Invoke();
        }

        private void EndRound()
        {
            SwitchTeam();
        }

        private void SwitchTeam()
        {
            currentTeam = currentTeam == Team.Red ? Team.Blue : Team.Red;
        }
    }
}