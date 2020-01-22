namespace MHWRoommates
{
    public static class RMOffsets
    {
        public const int TRANSFORM_START = 0x32;
        public const int ID = 0xBF;
        public const int ANIM = 0xC3;
        public const int UNKWN = 0xC7; // Unknown value, usually matches animation or npcID
        public const int COMMENT = 0x70;
        public const int FSM = 0x165; // TODO: Verify this offset
        public const int FSM_ROOM = 0x16F;
        public const int FSM_FOLDER = 0x174;
        public const int FSM_FILE = 0x17D;
        public const int FSM_END = 0x1E;

        public const int SOBJL_COUNT = 0x08;
        public const int SOBJL_NPC_START = 0x0C;
        // Rest of the offsets are relative to NPC object, not global
        public const int SOBJL_NPC_ST50X = 0x16;
        public const int SOBJL_NPC_FOLDER = 0x1B;
        public const int SOBJL_NPC_FILE = 0x20;
        public const int SOBJL_NPC_INSTANCE = 0x24;
        public const int SOBJL_NPC_SIZE = 0x2C;
    }
}