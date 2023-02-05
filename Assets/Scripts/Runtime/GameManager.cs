using System;
using System.Collections;
using System.ComponentModel;
using System.Resources;
using TeamShrimp.GGJ23.Networking;
using TeamShrimp.GGJ23.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TeamShrimp.GGJ23
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent onGameStarted;
        [SerializeField] private UnityEvent onRoundStarted;
        [SerializeField] private UnityEvent<Team> onTeamChanged;

        [SerializeField] private Resource startResourceType;
        [SerializeField] private float startResourceValue;

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
            ResourceTracker.Set(startResourceType, startResourceValue);
            Debug.Log("Player starts with " + ResourceTracker.ResourceToString(startResourceType));
            onGameStarted.Invoke();
            onTeamChanged.Invoke(currentTeam);

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
            onTeamChanged.Invoke(currentTeam);
        }
    }
}