using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class DummyCommandHandler : MonoBehaviour
    {
        public void DebugLogCommand(BaseCommand baseCommand)
        {
            Debug.Log("dummy command handler received command: " + baseCommand);
        }
    }
}