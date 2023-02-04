using Networking;
using UnityEngine;

namespace TeamShrimp.GGJ23.Runtime.Networking
{
    public class DummyCommandHandler : MonoBehaviour
    {
        public void DebugLogCommand(BaseCommand baseCommand)
        {
            Debug.Log(baseCommand);
        }
    }
}