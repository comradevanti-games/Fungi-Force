using System;
using System.Collections.Immutable;
using TeamShrimp.GGJ23.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TeamShrimp.GGJ23
{
    public class LobbyKeeper : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private TMP_InputField mapSizeInput;
        [SerializeField] private TMP_InputField mapSeedInput;
        [SerializeField] private TMP_InputField hostNameInput;

        private void OnEnable()
        {
            networkManager.Init(true);
            ReadInput();
        }

        private void ReadInput()
        {
            var mapSize = int.Parse(mapSizeInput.text);
            var mapSeed = int.Parse(mapSeedInput.text);
            var hostName = hostNameInput.text;
            Blackboard.Game =
                new Game(
                    ImmutableDictionary<Team, string>.Empty
                        .Add(Team.Red, hostName), mapSize, mapSeed);
        }

        public void OnConnectCommand(BaseCommand cmd)
        {
            if (Blackboard.IsHost && cmd is ConnectionInitCommand connectCmd)
                OnGuestJoined(connectCmd.playerName);
            else
                throw new Exception("Incorrect cmd type!");
        }

        private void OnGuestJoined(string playerName)
        {
            Blackboard.Game = Blackboard.Game with
            {
                PlayerNamesByTeam = Blackboard.Game.PlayerNamesByTeam
                    .Add(Team.Blue, playerName)
            };
            SendConnectInit();
            SendWorldInit();
            GoToGame();
        }

        private void SendConnectInit()
        {
            var hostName = Blackboard.Game.PlayerNamesByTeam[Team.Red];
            var cmd = new ConnectionInitCommand(hostName);
            networkManager.SendCommand(cmd);
        }

        private void SendWorldInit()
        {
            var cmd = new WorldInitCommand(
                Blackboard.Game.MapSize,
                Blackboard.Game.MapSeed);
            networkManager.SendCommand(cmd);
        }

        private static void GoToGame()
        {
            SceneManager.LoadScene("Game");
        }
    }
}