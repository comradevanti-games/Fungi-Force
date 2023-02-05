using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class TransformExt
    {
        public static IEnumerable<Transform> Children(this Transform t)
        {
            for (var i = 0; i < t.childCount; i++) yield return t.GetChild(i);
        }
    }
}