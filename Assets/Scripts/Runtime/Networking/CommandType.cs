namespace TeamShrimp.GGJ23.Networking
{
    public enum CommandType
    {
        // Connection management
        
        READY = 255, // LAYOUT: [255, namelength, ... name ...] - ReadyCommand
        START = 254, // 
        TIMEOUT = 252,
        LEAVE = 251,
        END = 250, // LAYOUT: [250, boolean winner]
        REMATCH = 249,
        WORLD_INIT = 248, // LAYOUT: [248, int side_length, long seed] - WorldInitCommand
        CONNECT = 247, // LAYOUT: [247, namelength, ... name ...] - ConnectCommand
        WIN = 246,
        
        ALL = 127, // APP INTERNAL, SUBSCRIBING TO THIS SUBSCRIBES TO ALL COMMANDS
        
        // Gameplay
        PLACE = 32, // LAYOUT: [0, mushtype length, ... mushtype ..., int id, vec2int pos, vec2int sourcePos] - PlaceCommand
        CUT = 1, // LAYOUT: [1, vec2int sourcePos, vec2int targetPos] - CutCommand
        DELETE = 2 // LAYOUT: [2, vec2int pos]
    }
}