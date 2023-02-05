using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class TeamExt 
    {


        public static Color ToColor(this Team team)
        {
            return team switch
            {
                Team.Red => new Color(0.61f, 0.15f, 0.14f),
                Team.Blue => new Color(0.32f, 0.48f, 0.54f),
                _ => throw new ArgumentOutOfRangeException(nameof(team), team,
                    null)
            };
        }
    }
}
