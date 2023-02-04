using UnityEngine;

namespace TeamShrimp.GGJ23.Networking
{
    public class TestNetCommandSender : MonoBehaviour
    {
        public void SendDummyConnectionInitCommand()
        {
            BaseCommand bc = new ConnectionInitCommand("playerTestName");
            NetworkManager.client.SendCommand(bc);
        }
        
        public void SendDummyPlaceCommand()
        {
            BaseCommand bc = new PlaceCommand("mushTypeString", 512L, new Vector2Int(10, 12), new Vector2Int(16,21));
            NetworkManager.client.SendCommand(bc);
        }
        
        public void SendDummyReadyCommand()
        {
            BaseCommand bc = new ReadyCommand();
            NetworkManager.client.SendCommand(bc);
        }
        
        public void SendDummyCutCommand()
        {
            BaseCommand bc = new CutCommand(new Vector2Int(12, 13), new Vector2Int(51,23));
            NetworkManager.client.SendCommand(bc);
        }
        
        
        public void SendDummyWorldInitCommand()
        {
            BaseCommand bc = new WorldInitCommand(15, 150);
            NetworkManager.client.SendCommand(bc);
        }
        
    }
}