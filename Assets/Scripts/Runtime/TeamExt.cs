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
                Team.Red => new Color(1f, 0.65f, 0.67f),
                Team.Blue => new Color(0.7f, 0.91f, 1f),
                _ => throw new ArgumentOutOfRangeException(nameof(team), team,
                    null)
            };
        }
    }
}
