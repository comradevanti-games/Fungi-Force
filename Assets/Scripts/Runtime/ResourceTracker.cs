using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Resource = TeamShrimp.GGJ23.Runtime.Resource;

namespace TeamShrimp.GGJ23
{
    public class ResourceTracker : MonoBehaviour
    {

        private static Dictionary<Resource, float> resources = new Dictionary<Resource, float>();

        public static UnityEvent<Resource> OnResourceChangedEvent = new UnityEvent<Resource>();

        public static float Get(Resource resource)
        {
            float result;
            if (resources.TryGetValue(resource, out result))
            {
                return result;
            }

            return float.MinValue;
        }

        public static bool Set(Resource resource, float value) => resources.TryAdd(resource, value);

        public static bool Add(Resource resource, float toAdd)
        {
            if (!resources.ContainsKey(resource))
                return false;

            float current = Get(resource);
            if (current.Equals(float.MinValue))
                return false;
            resources[resource] += toAdd;
            OnResourceChangedEvent.Invoke(resource);
            return true;
        }

        public static bool Subtract(Resource resource, float toSubtract) => Add(resource, toSubtract < 0 ? toSubtract : toSubtract * -1);

        public static String ResourceToString(Resource resource)
        {
            float value = Get(resource);
            return resource.ToString().ToArray()[0] + resource.ToString().Substring(1).ToLower() + ": " + 
                   (value.Equals(float.MinValue) ? "ERROR" : value);
        }
    }
}