using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MHWRoommates
{
    [XmlRoot("NPCList")]
    public class NPCList
    {
        [XmlElement("NPC")]
        public ObservableCollection<NPC> NPCs { get; set; }

        public string getName(UInt32 index)
        {
            return NPCs[(int)index-1].Name;
        }

        public void setNPCIndexes()
        {
            for (int i = 0; i < NPCs.Count; i++)
            {
                NPCs[i].Index = i + 1;
            }
        }
    }
}