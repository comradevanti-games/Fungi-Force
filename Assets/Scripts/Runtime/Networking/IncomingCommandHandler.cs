using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Networking
{
    public class IncomingCommandHandler : MonoBehaviour
    {
        public delegate BaseCommand CommandHandlerMapping(BinaryReader br);

        private Dictionary<CommandType, CommandHandlerMapping> handler = new Dictionary<CommandType, CommandHandlerMapping>();

        public List<CommandSubscription> subscribedNetworkCommands = new List<CommandSubscription>();

        private Dictionary<CommandType, UnityEvent<BaseCommand>> subscribers = new Dictionary<CommandType, UnityEvent<BaseCommand>>();
        
        public void Start()
        {
            Register(CommandType.READY, HandleCommandType<ReadyCommand>);
            Register(CommandType.PLACE, HandleCommandType<PlaceCommand>);
            Register(CommandType.CUT, HandleCommandType<CutCommand>);
            SubscriptionsToDict();
        }

        private void OnValidate()
        {
            SubscriptionsToDict();
        }

        public void Register(CommandType commandType, CommandHandlerMapping mapping)
        {
            handler[commandType] = mapping;
        }

        public void SubscriptionsToDict()
        {
            subscribers = new Dictionary<CommandType, UnityEvent<BaseCommand>>();
            foreach (var subscribedCommand in subscribedNetworkCommands)
            {
                subscribers[subscribedCommand.ct] = subscribedCommand.subscribers;
            }
            
            
            Debug.Log("SUBSCRIBERS: ");
            foreach (var VARIABLE in subscribers)
            {
                Debug.Log(VARIABLE);
            }
        }
        
        public void HandleCommand(byte[] incoming)
        {
            CommandType type;
            try
            {
                type = (CommandType) incoming[0];
            }
            catch (Exception e)
            {
                Debug.LogError("INVALID PACKET TYPE RECEIVED: " + (int)incoming[0]);
                return;
            }

            if (!handler.ContainsKey(type))
                Debug.LogError("MISSING PACKET HANDLER FOR PACKET TYPE " + type);

            BinaryReader br = new BinaryReader(new MemoryStream(incoming));
            br.ReadByte();
            BaseCommand bc = handler[type].Invoke(br);
            if (!bc.ValidPackage)
            {
                Debug.LogError("PACKAGE OF TYPE " + type + " HAS A PROBLEM: " + bc.deserializeException);
                return;
            }

            if (!subscribers.ContainsKey(type))
            {
                Debug.LogError("RECEIVED EVENT, BUT NO SUBSCRIBERS FOR EVENT TYPE " + type);
                return;
            }
            Debug.Log("RUNNING SUBSCRIBER for event " + bc);
            subscribers[type].Invoke(bc);
        }

        public BaseCommand HandleCommandType<T>(BinaryReader br) where T : BaseCommand
        {
            return (BaseCommand) Activator.CreateInstance(typeof(T), br);
        }
    }
    
    [Serializable]
    public class CommandSubscription
    {
        public CommandType ct;
        public UnityEvent<BaseCommand> subscribers;
    }
}