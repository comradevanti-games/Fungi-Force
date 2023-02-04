using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class TestNetCommandSender : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        public void SendDummyConnectionInitCommand()
        {
            BaseCommand bc = new ConnectionInitCommand("playerTestName");
            networkManager.SendCommand(bc);
        }

        public void SendDummyPlaceCommand()
        {
            BaseCommand bc = new PlaceCommand("mushTypeString", 512L,
                new Vector2Int(10, 12), new Vector2Int(16, 21));
            networkManager.SendCommand(bc);
        }

        public void SendDummyReadyCommand()
        {
            BaseCommand bc = new ReadyCommand();
            networkManager.SendCommand(bc);
        }

        public void SendDummyCutCommand()
        {
            BaseCommand bc = new CutCommand(new Vector2Int(12, 13),
                new Vector2Int(51, 23));
            networkManager.SendCommand(bc);
        }


        public void SendDummyWorldInitCommand()
        {
            BaseCommand bc = new WorldInitCommand(15, 150);
            networkManager.SendCommand(bc);
        }
    }
}