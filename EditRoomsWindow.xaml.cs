using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using static MHWRoommates.Room;

namespace MHWRoommates
{
    public partial class EditRoomsWindow : Window
    {
        private ObservableCollection<NPC> usingLivingNPCs;
        private ObservableCollection<NPC> usingPrivateNPCs;
        private ObservableCollection<NPC> usingSuiteNPCs;
        private ObservableCollection<NPC> usingResearchBaseNPCs;

        private Room selectedRoom;
        private ObservableCollection<NPC> selectedList;

        public EditRoomsWindow(NPCList npcList)
        {
            InitializeComponent();

            usingLivingNPCs = new ObservableCollection<NPC>();
            usingPrivateNPCs = new ObservableCollection<NPC>();
            usingSuiteNPCs = new ObservableCollection<NPC>();
            usingResearchBaseNPCs = new ObservableCollection<NPC>();

            NPCs_Available_Living.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_Private.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_Suite.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_ResearchBase.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));

            NPCs_Using_Living.ItemsSource = usingLivingNPCs;
            NPCs_Using_Private.ItemsSource = usingPrivateNPCs;
            NPCs_Using_Suite.ItemsSource = usingSuiteNPCs;
            NPCs_Using_ResearchBase.ItemsSource = usingResearchBaseNPCs;

            NPCs_Using_Living.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_Private.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_Suite.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_ResearchBase.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));

            LoadSOBJL(npcList, LIVING_QUARTERS.SOBJPaths[0], usingLivingNPCs);
            LoadSOBJL(npcList, PRIVATE_QUARTERS.SOBJPaths[0], usingPrivateNPCs);
            LoadSOBJL(npcList, PRIVATE_SUITE.SOBJPaths[0], usingSuiteNPCs);
            LoadSOBJL(npcList, RESEARCH_BASE.SOBJPaths[1], usingResearchBaseNPCs);

            selectedRoom = LIVING_QUARTERS;
            selectedList = usingLivingNPCs;
        }

        private void LoadSOBJL(NPCList npcList, string sobjlToLoad, ObservableCollection<NPC> usingNPCs)
        {
            // If there is no NPC List to load, setup a default list and return
            if (!File.Exists(sobjlToLoad))
            {
                const int HOUSEKEEPER_INDEX = 13;
                const int ACE_CADET_INDEX = 22;
                const int SRSHANDLER_INDEX = 23;
                //usingNPCs.Add(npcList.NPCs[HOUSEKEEPER_INDEX]);
                //usingNPCs.Add(npcList.NPCs[ACE_CADET_INDEX]);
                //usingNPCs.Add(npcList.NPCs[SRSHANDLER_INDEX]);
                
                return;
            }

            using (BinaryReader reader = new BinaryReader(File.Open(sobjlToLoad, FileMode.Open)))
            {
                reader.BaseStream.Position = RMOffsets.SOBJL_COUNT;
                uint npcCount = reader.ReadUInt32();

                for (int i = 0; i < npcCount; i++)
                {
                    reader.BaseStream.Position = RMOffsets.SOBJL_NPC_START + RMOffsets.SOBJL_NPC_FILE + (i * RMOffsets.SOBJL_NPC_SIZE);
                    int npcIndex = int.Parse(new string(reader.ReadChars(3)));
                    //usingNPCs.Add(npcList.NPCs[npcIndex - 1]);
                    // Needs optimization, too lazy to look up the proper way
                    bool found = false;
                    for (int j = 0; j < npcList.NPCs.Count; j++)
                    {
                        if (npcList.NPCs[j].Index == npcIndex) { usingNPCs.Add(npcList.NPCs[j]); found = true; }
                    }
                    if (found == false) MessageBox.Show($"Couldn't parse index {npcIndex}", "Parsing error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private ObservableCollection<NPC> RemoveNPC(ListBox sourceBox, ObservableCollection<NPC> sourceList)
        {
            sourceList.Remove((NPC)sourceBox.SelectedItem);
            sourceBox.SelectedItem = null;
            return new ObservableCollection<NPC>(sourceList.OrderBy(npc => npc.Index));            
        }

        private ObservableCollection<NPC> AddNPC(ListBox sourceBox, ObservableCollection<NPC> destList)
        {
            destList.Add((NPC)sourceBox.SelectedItem);
            sourceBox.SelectedItem = null;
            return new ObservableCollection<NPC>(destList.OrderBy(npc => npc.Index));
        }

        #region moveNPC events
        private void Button_Add_NPC_Personal_Click(object sender, RoutedEventArgs e)
        {
            usingLivingNPCs = AddNPC(NPCs_Available_Living, usingLivingNPCs);
            NPCs_Using_Living.ItemsSource = usingLivingNPCs;
        }

        private void Button_Rmv_NPC_Personal_Click(object sender, RoutedEventArgs e)
        {
            usingLivingNPCs = RemoveNPC(NPCs_Using_Living, usingLivingNPCs);
            NPCs_Using_Living.ItemsSource = usingLivingNPCs;
        }

        private void Button_Add_NPC_Private_Click(object sender, RoutedEventArgs e)
        {
            usingPrivateNPCs = AddNPC(NPCs_Available_Private, usingPrivateNPCs);
            NPCs_Using_Private.ItemsSource = usingPrivateNPCs;
        }

        private void Button_Rmv_NPC_Private_Click(object sender, RoutedEventArgs e)
        {
            usingPrivateNPCs = RemoveNPC(NPCs_Using_Private, usingPrivateNPCs);
            NPCs_Using_Private.ItemsSource = usingPrivateNPCs;
        }

        private void Button_Add_NPC_Suite_Click(object sender, RoutedEventArgs e)
        {
            usingSuiteNPCs = AddNPC(NPCs_Available_Suite, usingSuiteNPCs);
            NPCs_Using_Suite.ItemsSource = usingSuiteNPCs;
        }

        private void Button_Rmv_NPC_Suite_Click(object sender, RoutedEventArgs e)
        {
            usingSuiteNPCs = RemoveNPC(NPCs_Using_Suite, usingSuiteNPCs);
            NPCs_Using_Suite.ItemsSource = usingSuiteNPCs;
        }

        private void Button_Add_NPC_ResearchBase_Click(object sender, RoutedEventArgs e)
        {
            usingResearchBaseNPCs = AddNPC(NPCs_Available_ResearchBase, usingResearchBaseNPCs);
            NPCs_Using_ResearchBase.ItemsSource = usingResearchBaseNPCs;
        }

        private void Button_Rmv_NPC_ResearchBase_Click(object sender, RoutedEventArgs e)
        {
            usingResearchBaseNPCs = RemoveNPC(NPCs_Using_ResearchBase, usingResearchBaseNPCs);
            NPCs_Using_ResearchBase.ItemsSource = usingResearchBaseNPCs;
        }
        #endregion

        #region Enable/Disable Add and Remove buttons based on selections
        private void NPCs_Available_Private_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_Private.IsEnabled = NPCs_Available_Private.SelectedItem != null;
        }

        private void NPCs_Using_Private_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_Private.IsEnabled = NPCs_Using_Private.SelectedItem != null;
        }

        private void NPCs_Available_Suite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_Suite.IsEnabled = NPCs_Available_Suite.SelectedItem != null;
        }

        private void NPCs_Using_Suite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_Suite.IsEnabled = NPCs_Using_Suite.SelectedItem != null;
        }

        private void NPCs_Using_Personal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_Personal.IsEnabled = NPCs_Using_Living.SelectedItem != null;
        }

        private void NPCs_Available_Personal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_Personal.IsEnabled = NPCs_Available_Living.SelectedItem != null;
        }

        private void NPCs_Using_ResearchBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_ResearchBase.IsEnabled = NPCs_Using_ResearchBase.SelectedItem != null;
        }

        private void NPCs_Available_ResearchBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_ResearchBase.IsEnabled = NPCs_Available_ResearchBase.SelectedItem != null;
        }
        #endregion

        private void SaveSelectedSOBJL(object sender, RoutedEventArgs e)
        {
            selectedList = new ObservableCollection<NPC>(selectedList.ToList().OrderBy(x => x.Index));

            DeleteSelectedSOBJL(false);

            Directory.CreateDirectory($"{selectedRoom.Path}\\sobjl");

            File.Copy(RMFiles.SOBJL, selectedRoom.SOBJPaths[0]);

            byte[] templateNPC = GetTemplateNpc();
            WriteBaseSOBJL(templateNPC);
            CopyToSubSOBJs();

            MessageBox.Show($"Saved to \"{selectedRoom.SOBJPaths[0]}*\"",
                "NPC List Saved!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private byte[] GetTemplateNpc()
        {
            byte[] templateNPC = new byte[RMOffsets.SOBJL_NPC_SIZE];

            using (BinaryReader reader = new BinaryReader(File.Open(selectedRoom.SOBJPaths[0], FileMode.Open)))
            {
                reader.BaseStream.Position = RMOffsets.SOBJL_NPC_START;
                templateNPC = reader.ReadBytes(RMOffsets.SOBJL_NPC_SIZE);
            }

            return templateNPC;
        }

        private void WriteBaseSOBJL(byte[] templateNPC)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(selectedRoom.SOBJPaths[0], FileMode.Open)))
            {
                WriteTemplateNPCs(templateNPC, writer);

                int npcInstance = 0;
                for (int i = 0; i < selectedList.Count; i++)
                {
                    WriteNPC(writer, i);
                    npcInstance = WriteNpcCopies(writer, npcInstance, i);
                }
            }
        }

        private void WriteTemplateNPCs(byte[] templateNPC, BinaryWriter writer)
        {
            writer.BaseStream.Position = RMOffsets.SOBJL_COUNT;
            writer.Write(selectedList.Count);

            for (int i = 0; i < selectedList.Count; i++)
            {
                writer.Write(templateNPC);
            }
        }

        private void WriteNPC(BinaryWriter writer, int i)
        {
            /*
            NPCList npcList = new NPCList();
            XmlSerializer serializer = new XmlSerializer(typeof(NPCList));
            using (FileStream fileStream = new FileStream("NPCList.xml", FileMode.Open))
            {
                npcList = (NPCList)serializer.Deserialize(fileStream);
            }
            npcList.setNPCIndexes();
            int npcId = npcList.NPCs[selectedList[i].Index - 1].NpcID;
            */

            writer.BaseStream.Position = RMOffsets.SOBJL_NPC_START + (i * RMOffsets.SOBJL_NPC_SIZE) + RMOffsets.SOBJL_NPC_ST50X;
            byte[] bytesID = Encoding.ASCII.GetBytes(selectedRoom.ID);
            writer.Write(bytesID);

            char[] indexChars = selectedList[i].Index.ToString("D3").ToArray();

            writer.BaseStream.Position = RMOffsets.SOBJL_NPC_START + (i * RMOffsets.SOBJL_NPC_SIZE) + RMOffsets.SOBJL_NPC_FOLDER;
            writer.Write(indexChars);
            //writer.Write(npcId.ToString("D3").ToArray());

            writer.BaseStream.Position = RMOffsets.SOBJL_NPC_START + (i * RMOffsets.SOBJL_NPC_SIZE) + RMOffsets.SOBJL_NPC_FILE;
            writer.Write(indexChars);
            //writer.Write(npcId.ToString("D3").ToArray());
        }

        private int WriteNpcCopies(BinaryWriter writer, int npcInstance, int i)
        {
            bool isCopy = i > 0 && selectedList[i].Index == selectedList[i - 1].Index;

            if (!isCopy)
                return 0;

            npcInstance++;

            writer.BaseStream.Position = RMOffsets.SOBJL_NPC_START + (i * RMOffsets.SOBJL_NPC_SIZE) + RMOffsets.SOBJL_NPC_INSTANCE;
            writer.Write(npcInstance.ToString("D3").ToArray());

            return npcInstance;
        }

        private void CopyToSubSOBJs()
        {
            for (int i = 1; i < selectedRoom.SOBJPaths.Length; i++)
            {
                File.Copy(selectedRoom.SOBJPaths[0], selectedRoom.SOBJPaths[i]);
            }
        }

        private void ResetSelectedList()
        {
            while (selectedList.Count > 0)
            {
                selectedList.Remove(selectedList[0]);
            }
        }

        private void DeleteSelectedSOBJL(bool showMessage)
        {
            if (!File.Exists(selectedRoom.SOBJPaths[0]))
                return;

            foreach (string sobjl in selectedRoom.SOBJPaths) File.Delete(sobjl);

            Directory.Delete($"{selectedRoom.Path}\\sobjl");

            if (showMessage) MessageBox.Show($"Deleted \"{selectedRoom.SOBJPaths[0]}*\"",
                "NPC List Deleted!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            string message = $"Are you sure you want to delete \"{selectedRoom.SOBJPaths[0]}\"?";
            MessageBoxResult result = MessageBox.Show(message, "Delete NPC List?", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result.Equals(MessageBoxResult.Yes))
            {
                DeleteSelectedSOBJL(true);
                ResetSelectedList();
            }
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Room_Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(Room_Tabs.SelectedIndex)
            {
                case 0: selectedRoom = LIVING_QUARTERS; selectedList = usingLivingNPCs; break;
                case 1: selectedRoom = PRIVATE_QUARTERS; selectedList = usingPrivateNPCs; break;
                case 2: selectedRoom = PRIVATE_SUITE; selectedList = usingSuiteNPCs; break;
                case 3: selectedRoom = RESEARCH_BASE; selectedList = usingResearchBaseNPCs; break;
            }
        }
    }
}