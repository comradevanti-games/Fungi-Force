using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Mono.Cecil;
using TMPro;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class LobbyKeeper : MonoBehaviour
    {
        public TMP_InputField mapSizeInput;
        public TMP_InputField mapSeedInput;
        public TMP_InputField hostNameInput;

        private void OnEnable()
        {
            ReadInput();
        }

        private void ReadInput()
        {
            var mapSize = int.Parse(mapSizeInput.text);
            var mapSeed = int.Parse(mapSeedInput.text);
            var hostName = hostNameInput.text;
            Blackboard.Game = new Game(ImmutableDictionary<Team, string>.Empty.Add(Team.Red,hostName),mapSize,mapSeed);
            
        }
    }
    
}
