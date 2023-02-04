using System;
using System.Collections.Immutable;
using TeamShrimp.GGJ23.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamShrimp.GGJ23
{
    public class Joiner : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private TMP_InputField ipInput;
        [SerializeField] private TMP_InputField nameInput;


        private string Ip => ipInput.text.Split(":")[0];

        private int Port => int.Parse(ipInput.text.Split(":")[1]);

        private string GuestName => nameInput.text;


        public void OnJoinPressed()
        {
            SendConnectCommand();
        }

        public void OnConnectedCmd(BaseCommand cmd)
        {
            if (!Blackboard.IsHost && cmd is ConnectionInitCommand connectCmd)
                OnConnected(connectCmd);
            else throw new Exception("Incorrect cmd type!");
        }

        public void OnWorldInitCmd(BaseCommand cmd)
        {
            if (cmd is WorldInitCommand worldCmd)
                OnWorldInit(worldCmd);
            else throw new Exception("Incorrect cmd type!");
        }

        private void OnConnected(ConnectionInitCommand cmd)
        {
            Debug.Log($"Connected to {cmd.playerName}'s game!");
            Blackboard.Game =
                new Game(
                    ImmutableDictionary<Team, string>.Empty
                        .Add(Team.Red, cmd.playerName)
                        .Add(Team.Blue, GuestName), 0, 0);
        }

        private void OnWorldInit(WorldInitCommand cmd)
        {
            Blackboard.Game = Blackboard.Game with
            {
                MapSeed = cmd.seed,
                MapSize = cmd.size
            };
            GoToGame();
        }

        private static void GoToGame()
        {
            SceneManager.LoadScene("Game");
        }

        private void SendConnectCommand()
        {
            var cmd = new ConnectionInitCommand(GuestName);
            networkManager.SendCommand(cmd);
        }
    }
}