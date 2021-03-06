﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static MHWRoommates.Room;

namespace MHWRoommates
{
    public partial class EditRoomsWindow : Window
    {
        public bool MoveBothWindows;

        private ObservableCollection<NPC> usingLivingNPCs;
        private ObservableCollection<NPC> usingPrivateNPCs;
        private ObservableCollection<NPC> usingSuiteNPCs;
        private ObservableCollection<NPC> usingResearchBaseNPCs;
        private ObservableCollection<NPC> usingSelianaSuiteNPCs;
        private ObservableCollection<NPC> usingSelianaPubNPCs;
        private ObservableCollection<NPC> usingSelianaNPCs;
        private ObservableCollection<NPC> usingAsteraNPCs;

        private Room selectedRoom;
        private ObservableCollection<NPC> selectedList;

        public EditRoomsWindow(NPCList npcList)
        {
            InitializeComponent();
            MoveBothWindows = true;

            usingLivingNPCs = new ObservableCollection<NPC>();
            usingPrivateNPCs = new ObservableCollection<NPC>();
            usingSuiteNPCs = new ObservableCollection<NPC>();
            usingResearchBaseNPCs = new ObservableCollection<NPC>();
            usingSelianaSuiteNPCs = new ObservableCollection<NPC>();
            usingSelianaPubNPCs = new ObservableCollection<NPC>();
            usingSelianaNPCs = new ObservableCollection<NPC>();
            usingAsteraNPCs = new ObservableCollection<NPC>();

            NPCs_Available_Living.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_Private.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_Suite.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_ResearchBase.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_SelianaSuite.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_SelianaPub.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_Seliana.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));
            NPCs_Available_Astera.ItemsSource = npcList.NPCs.Where(x => !x.Warning.Equals("Ignore"));

            NPCs_Using_Living.ItemsSource = usingLivingNPCs;
            NPCs_Using_Private.ItemsSource = usingPrivateNPCs;
            NPCs_Using_Suite.ItemsSource = usingSuiteNPCs;
            NPCs_Using_ResearchBase.ItemsSource = usingResearchBaseNPCs;
            NPCs_Using_SelianaSuite.ItemsSource = usingSelianaSuiteNPCs;
            NPCs_Using_SelianaPub.ItemsSource = usingSelianaPubNPCs;
            NPCs_Using_Seliana.ItemsSource = usingSelianaNPCs;
            NPCs_Using_Astera.ItemsSource = usingAsteraNPCs;

            NPCs_Using_Living.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_Private.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_Suite.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_ResearchBase.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_SelianaSuite.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_SelianaPub.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_Seliana.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
            NPCs_Using_Astera.Items.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));

            if (LoadSOBJL(npcList, LIVING_QUARTERS.SOBJPaths[0], usingLivingNPCs, "501"))
                Button_Delete_Living.IsEnabled = true;
            if (LoadSOBJL(npcList, PRIVATE_QUARTERS.SOBJPaths[0], usingPrivateNPCs, "502"))
                Button_Delete_Private.IsEnabled = true;
            if (LoadSOBJL(npcList, PRIVATE_SUITE.SOBJPaths[0], usingSuiteNPCs, "503"))
                Button_Delete_Suite.IsEnabled = true;
            if (LoadSOBJL(npcList, RESEARCH_BASE.SOBJPaths[1], usingResearchBaseNPCs, "303"))
                Button_Delete_ResearchBase.IsEnabled = true;
            if (LoadSOBJL(npcList, SELIANA_SUITE.SOBJPaths[0], usingSelianaSuiteNPCs, "506"))
                Button_Delete_SelianaSuite.IsEnabled = true;
            if (LoadSOBJL(npcList, SELIANA_PUB.SOBJPaths[0], usingSelianaPubNPCs, "306"))
                Button_Delete_SelianaPub.IsEnabled = true;
            if (LoadSOBJL(npcList, SELIANA.SOBJPaths[0], usingSelianaNPCs, "305"))
                Button_Delete_Seliana.IsEnabled = true;
            if (LoadSOBJL(npcList, ASTERA.SOBJPaths[0], usingAsteraNPCs, "301"))
                Button_Delete_Astera.IsEnabled = true;

            selectedRoom = LIVING_QUARTERS;
            selectedList = usingLivingNPCs;
        }

        private bool LoadSOBJL(NPCList npcList, string sobjlToLoad, ObservableCollection<NPC> usingNPCs, string roomID)
        {
            // If there is no NPC List to load, setup a default list and return
            if (!File.Exists(sobjlToLoad))
            {
                var defaultNPCs = npcList.NPCs.Where(x => x.DefaultRooms.Contains(roomID));

                foreach (NPC defaultNPC in defaultNPCs)
                {
                    usingNPCs.Add(defaultNPC);
                }

                return false;
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
                    if (found == false)
                    {
                        MessageBox.Show($"Couldn't parse index {npcIndex}", "Parsing error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            return true;
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

        private void Button_Add_NPC_SelianaSuite_Click(object sender, RoutedEventArgs e)
        {
            usingSelianaSuiteNPCs = AddNPC(NPCs_Available_SelianaSuite, usingSelianaSuiteNPCs);
            NPCs_Using_SelianaSuite.ItemsSource = usingSelianaSuiteNPCs;
        }

        private void Button_Rmv_NPC_SelianaSuite_Click(object sender, RoutedEventArgs e)
        {
            usingSelianaSuiteNPCs = RemoveNPC(NPCs_Using_SelianaSuite, usingSelianaSuiteNPCs);
            NPCs_Using_SelianaSuite.ItemsSource = usingSelianaSuiteNPCs;
        }

        private void Button_Add_NPC_SelianaPub_Click(object sender, RoutedEventArgs e)
        {
            usingSelianaPubNPCs = AddNPC(NPCs_Available_SelianaPub, usingSelianaPubNPCs);
            NPCs_Using_SelianaPub.ItemsSource = usingSelianaPubNPCs;
        }

        private void Button_Rmv_NPC_SelianaPub_Click(object sender, RoutedEventArgs e)
        {
            usingSelianaPubNPCs = RemoveNPC(NPCs_Using_SelianaPub, usingSelianaPubNPCs);
            NPCs_Using_SelianaPub.ItemsSource = usingSelianaPubNPCs;
        }

        private void Button_Add_NPC_Seliana_Click(object sender, RoutedEventArgs e)
        {
            usingSelianaNPCs = AddNPC(NPCs_Available_Seliana, usingSelianaNPCs);
            NPCs_Using_Seliana.ItemsSource = usingSelianaNPCs;
        }

        private void Button_Rmv_NPC_Seliana_Click(object sender, RoutedEventArgs e)
        {
            usingSelianaNPCs = RemoveNPC(NPCs_Using_Seliana, usingSelianaNPCs);
            NPCs_Using_Seliana.ItemsSource = usingSelianaNPCs;
        }

        private void Button_Add_NPC_Astera_Click(object sender, RoutedEventArgs e)
        {
            usingAsteraNPCs = AddNPC(NPCs_Available_Astera, usingAsteraNPCs);
            NPCs_Using_Astera.ItemsSource = usingAsteraNPCs;
        }

        private void Button_Rmv_NPC_Astera_Click(object sender, RoutedEventArgs e)
        {
            usingAsteraNPCs = RemoveNPC(NPCs_Using_Astera, usingAsteraNPCs);
            NPCs_Using_Astera.ItemsSource = usingAsteraNPCs;
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

        private void NPCs_Available_SelianaSuite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_SelianaSuite.IsEnabled = NPCs_Available_SelianaSuite.SelectedItem != null;
        }

        private void NPCs_Using_SelianaSuite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_SelianaSuite.IsEnabled = NPCs_Using_SelianaSuite.SelectedItem != null;
        }

        private void NPCs_Available_SelianaPub_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_SelianaPub.IsEnabled = NPCs_Available_SelianaPub.SelectedItem != null;
        }

        private void NPCs_Using_SelianaPub_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_SelianaPub.IsEnabled = NPCs_Using_SelianaPub.SelectedItem != null;
        }

        private void NPCs_Available_Seliana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_Seliana.IsEnabled = NPCs_Available_Seliana.SelectedItem != null;
        }

        private void NPCs_Using_Seliana_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_Seliana.IsEnabled = NPCs_Using_Seliana.SelectedItem != null;
        }

        private void NPCs_Available_Astera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Add_NPC_Astera.IsEnabled = NPCs_Available_Astera.SelectedItem != null;
        }

        private void NPCs_Using_Astera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Rmv_NPC_Astera.IsEnabled = NPCs_Using_Astera.SelectedItem != null;
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

            Button deleteButtonToEnable;
            switch(Room_Tabs.SelectedIndex)
            {
                case 0: deleteButtonToEnable = Button_Delete_Living; break;
                case 1: deleteButtonToEnable = Button_Delete_Private; break;
                case 2: deleteButtonToEnable = Button_Delete_Suite; break;
                case 3: deleteButtonToEnable = Button_Delete_SelianaSuite; break;
                case 4: deleteButtonToEnable = Button_Delete_Astera; break;
                case 6: deleteButtonToEnable = Button_Delete_Seliana; break;
                case 7: deleteButtonToEnable = Button_Delete_SelianaPub; break;
                case 8: deleteButtonToEnable = Button_Delete_ResearchBase; break;
                default: deleteButtonToEnable = null; break;
            }
            if (deleteButtonToEnable != null) deleteButtonToEnable.IsEnabled = true;
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
                ((Button)sender).IsEnabled = false;
                DeleteSelectedSOBJL(true);
                ResetSelectedList();
            }
        }

        private void Room_Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(Room_Tabs.SelectedIndex)
            {
                case 0: selectedRoom = LIVING_QUARTERS; selectedList = usingLivingNPCs; break;
                case 1: selectedRoom = PRIVATE_QUARTERS; selectedList = usingPrivateNPCs; break;
                case 2: selectedRoom = PRIVATE_SUITE; selectedList = usingSuiteNPCs; break;
                case 3: selectedRoom = SELIANA_SUITE; selectedList = usingSelianaSuiteNPCs; break;
                case 4: selectedRoom = ASTERA; selectedList = usingAsteraNPCs; break;
                case 6: selectedRoom = SELIANA; selectedList = usingSelianaNPCs; break;
                case 7: selectedRoom = SELIANA_PUB; selectedList = usingSelianaPubNPCs; break;
                case 8: selectedRoom = RESEARCH_BASE; selectedList = usingResearchBaseNPCs; break;
            }
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Window_LocationChanged(object sender, System.EventArgs e)
        {
            if (!MoveBothWindows) return;
            if (Owner is null) return;

            ((MainWindow)Owner).MoveBothWindows = false;
            Owner.Left = this.Left;
            Owner.Top = this.Top - Owner.Height;
            ((MainWindow)Owner).MoveBothWindows = true;
        }
    }
}