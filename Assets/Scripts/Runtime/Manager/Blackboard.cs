using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Blackboard
    {
        public static Game Game { get; set; }
        
        public static bool IsHost { get; set; }
    }
}
