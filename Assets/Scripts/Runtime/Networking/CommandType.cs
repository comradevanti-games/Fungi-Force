namespace Networking
{
    public enum CommandType
    {
        // Connection management
        
        READY = 255, // LAYOUT: [255, namelength, ... name ...]
        START = 254, // 
        TIMEOUT = 252,
        LEAVE = 251,
        END = 250, // LAYOUT: [250, boolean winner]
        REMATCH = 249,
        WORLD_INIT = 248, // LAYOUT: [248, int side_length, long seed]
        
        // Gameplay
        PLACE = 32, // LAYOUT: [0, mushType, int id, vec2int pos, vec2int sourcePos]
        CUT = 1, // LAYOUT: [1, vec2int sourcePos, vec2int targetPos]
        DELETE = 2 // LAYOUT: [2, vec2int pos]
    }
}