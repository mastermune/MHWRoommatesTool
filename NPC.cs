using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MHWRoommates
{
    public class NPC
    {
        [XmlElement("NPCID")]
        public int NpcID { get; set; }

        [XmlElement("Animation")]
        public UInt32 Animation { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Warning")]
        public string Warning { get; set; }

        public int Index { get; set; }

        [XmlArray("Animations")]
        [XmlArrayItem("Animation")]
        public List<UInt32> Animations { get; set; }

        [XmlArray("Positions")]
        [XmlArrayItem("Position")]
        public List<Room.Point3D> DefaultPositions { get; set; }

        [XmlArray("Rooms")]
        [XmlArrayItem("Room")]
        public List<string> DefaultRooms { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}