using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MHWRoommates.Room;

namespace MHWRoommates
{
    public partial class MainWindow : Window
    {
        private NPCList npcList;

        private readonly string curDir = Directory.GetCurrentDirectory();

        public MainWindow(NPCList list)
        {
            InitializeComponent();
            PopulateRoomComboBox();
            npcList = list;
            PopulateNPCComboBox();
            PopulateAnimationsComboBox();
        }

        private void PopulateRoomComboBox()
        {
            Room_Select.Items.Add(LIVING_QUARTERS);
            Room_Select.Items.Add(PRIVATE_QUARTERS);
            Room_Select.Items.Add(PRIVATE_SUITE);
            Room_Select.Items.Add(RESEARCH_BASE);
        }

        private void PopulateNPCComboBox()
        {
            for (int i = 0; i < npcList.NPCs.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();

                SetWarningColour(npcList.NPCs[i], item);

                if (npcList.NPCs[i].Warning != "Ignore")
                {
                    item.Content = npcList.NPCs[i];
                    NPC_Select.Items.Add(item);
                }
            }
        }

        private void SetWarningColour(NPC npc, ComboBoxItem item)
        {
            switch (npc.Warning)
            {
                case "Crash": item.Background = Brushes.Red; item.Foreground = Brushes.White; break;
                case "Missing": item.Background = Brushes.Gray; item.Foreground = Brushes.White; break;
                case "Missing-Rooms": item.Background = Brushes.DarkGray; item.Foreground = Brushes.White; break;
                case "Missing-Research": item.Background = Brushes.LightGray; break;
                case "NoLoop": item.Background = Brushes.Purple; item.Foreground = Brushes.White; break;
                case "Cheat": item.Background = Brushes.DarkRed; item.Foreground = Brushes.White; item.IsEnabled = false; break;
                case "Bounds": item.Background = Brushes.Yellow; break;
                case "Story": item.Background = Brushes.Pink; break;
                case "Placeholder": item.Background = Brushes.Green; item.Foreground = Brushes.White; break;
                case "Animation": item.Background = Brushes.Blue; item.Foreground = Brushes.White; break;
            }
        }

        private void PopulateAnimationsComboBox()
        {
            for (int i = 0; i < npcList.NPCs.Count; i++)
            {
                switch (npcList.NPCs[i].Warning)
                {
                    case "Crash":
                    case "Ignore":
                    case "Placeholder":
                    case "Animation":
                    case "Cheat":
                        break;
                    default: AddNPCAnimations(npcList.NPCs[i]); break;
                }
            }
        }

        private void AddNPCAnimations(NPC npc)
        {
            bool hasMultipleAnimations = npc.Animations != null && npc.Animations.Count > 0;

            if (hasMultipleAnimations)
            {
                for (int j = 0; j < npc.Animations.Count; j++)
                {
                    ComboBoxItem item = new ComboBoxItem();

                    NPC npcToAdd = new NPC
                    {
                        Name = $"{npc.Name} [{npc.Animations[j]:D4}]",
                        Animation = npc.Animations[j]
                    };

                    item.Content = npcToAdd;
                    Animation_Select.Items.Add(item);
                }
            } else {
                ComboBoxItem item = new ComboBoxItem{ Content = npc };

                Animation_Select.Items.Add(item);
            }
        }

        private void Select_Button_Click(object sender, RoutedEventArgs e)
        {
            if (NPC_Select.SelectedItem == null || Room_Select.SelectedItem == null || NPC_Instance.Value == null)
                return;

            EnableControls();

            NPC selectedNPC = ((ComboBoxItem)NPC_Select.SelectedItem).Content as NPC;

            string npcFilePath = GetNPCFilePath(GetNPCDirectory());
            if (!File.Exists(npcFilePath))
            {
                Delete_Button.IsEnabled = false;
                FillControlsWithDefault(selectedNPC);
            } else
            {
                Delete_Button.IsEnabled = true;
                LoadNPC(npcFilePath);
            }
        }

        private void EnableControls()
        {
            X_Position.IsEnabled = true;
            Y_Position.IsEnabled = true;
            Z_Position.IsEnabled = true;

            X_Rotation.IsEnabled = true;
            Y_Rotation.IsEnabled = true;
            Z_Rotation.IsEnabled = true;

            Animation_Select.IsEnabled = true;

            Save_Button.IsEnabled = true;

            Select_Button.IsEnabled = false;
        }

        private void FillControlsWithDefault(NPC selectedNPC)
        {
            Room selectedRoom = (Room_Select.SelectedItem as Room);

            bool defaultExists = selectedNPC.DefaultRooms != null && selectedNPC.DefaultRooms.Contains(selectedRoom.ID);
            // Housekeeper and Handler defaults are the RoomDefaults and not used in xml
            defaultExists = defaultExists && (selectedNPC.NpcID != 2 && selectedNPC.NpcID != 16);

            Point3D defaultPosition = (defaultExists)? selectedNPC.DefaultPosition : selectedRoom.DefaultPosition;
            X_Position.Value = defaultPosition.x;
            Y_Position.Value = defaultPosition.y;
            Z_Position.Value = defaultPosition.z;

            Point3D defaultRotation = (defaultExists)? selectedNPC.DefaultRotation : selectedRoom.DefaultRotation;
            X_Rotation.Value = defaultRotation.x;
            Y_Rotation.Value = defaultRotation.y;
            Z_Rotation.Value = defaultRotation.z;

            LoadAnimation(selectedNPC.Animation);
        }

        private void LoadNPC(string npcFilePath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(npcFilePath, FileMode.Open)))
            {
                reader.BaseStream.Position = RMOffsets.TRANSFORM_START;
                X_Position.Value = reader.ReadSingle();
                Y_Position.Value = reader.ReadSingle();
                Z_Position.Value = reader.ReadSingle();

                X_Rotation.Value = reader.ReadSingle();
                Y_Rotation.Value = reader.ReadSingle();
                Z_Rotation.Value = reader.ReadSingle();

                reader.BaseStream.Position = RMOffsets.ANIM;
                uint animationToLoad = reader.ReadUInt32();
                LoadAnimation(animationToLoad);
            }
        }

        private void LoadAnimation(uint animationToLoad)
        {
            var temp = Animation_Select.Items.OfType<ComboBoxItem>().First(x => ((NPC)x.Content).Animation == animationToLoad);
            Animation_Select.SelectedItem = temp ?? Animation_Select.Items.GetItemAt(1);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string npcDirectory = GetNPCDirectory();
            Directory.CreateDirectory(npcDirectory);
            string npcFilePath = GetNPCFilePath(npcDirectory, NPC_Instance.Value.Value);

            CreateBaseNPCFile(npcFilePath);

            byte[] fsm = GetNpcFSM();

            using (BinaryWriter writer = new BinaryWriter(File.Open(npcFilePath, FileMode.Open)))
            {
                SaveNPC(fsm, writer);
            }

            Delete_Button.IsEnabled = true;

            MessageBox.Show($"Saved to \"{npcFilePath}\".", "NPC Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private byte[] GetNpcFSM()
        {
            const int HOUSEKEEPER = 14;

            if ((((ComboBoxItem)NPC_Select.SelectedItem).Content as NPC).Index != HOUSEKEEPER)
                return new byte[] { };

            byte[] fsm;
            using (BinaryReader reader = new BinaryReader(File.Open(RMFiles.FSM, FileMode.Open)))
            {
                fsm = reader.ReadBytes((int)reader.BaseStream.Length);
            }

            return fsm;
        }

        private void SaveNPC(byte[] fsm, BinaryWriter writer)
        {
            NPC selectedNPC = (((ComboBoxItem)NPC_Select.SelectedItem).Content as NPC);
            uint selectedAnimation = (((ComboBoxItem)Animation_Select.SelectedItem).Content as NPC).Animation;

            writer.BaseStream.Position = RMOffsets.TRANSFORM_START;
            writer.Write(X_Position.Value.Value);
            writer.Write(Y_Position.Value.Value);
            writer.Write(Z_Position.Value.Value);

            writer.Write(X_Rotation.Value.Value);
            writer.Write(Y_Rotation.Value.Value);
            writer.Write(Z_Rotation.Value.Value);

            writer.BaseStream.Position = RMOffsets.ID;
            writer.Write(selectedNPC.Index);

            writer.Write(selectedAnimation);
            // Unknown value, usually matches animation or npcID, doesn't seem to affect anything?
            writer.Write(selectedAnimation % 1000);

            WriteFSMLink(fsm, writer, selectedNPC);
        }

        private void WriteFSMLink(byte[] fsm, BinaryWriter writer, NPC selectedNPC)
        {
            // Only supporting Housekeeper's FSM
            const int HOUSEKEEPER = 14;
            if (selectedNPC.Index != HOUSEKEEPER)
                return;

            string roomID = (Room_Select.SelectedItem as Room).ID;

            writer.BaseStream.Position = RMOffsets.FSM;
            writer.Write(fsm);

            writer.BaseStream.Position = RMOffsets.FSM_ROOM;
            writer.Write(roomID);

            writer.BaseStream.Position = RMOffsets.FSM_FOLDER;
            writer.Write("016".ToCharArray());

            writer.BaseStream.Position = RMOffsets.FSM_FILE;
            writer.Write("016".ToCharArray());

            // Possible typo fix, overwrites "_000"
            if (roomID != "501")
            {
                writer.Write("_00".ToCharArray());
                writer.Write(fsm, 0x26, 6);
            }
        }

        private static void CreateBaseNPCFile(string npcFilePath)
        {
            if (File.Exists(npcFilePath))
            {
                MakeNewNPCBackup(npcFilePath);
            }
            else
            {
                File.Copy(RMFiles.SOBJ, npcFilePath);
            }
        }

        private static void MakeNewNPCBackup(string npcFilePath)
        {
            if (File.Exists($"{npcFilePath}.bak")) File.Delete($"{npcFilePath}.bak");

            File.Copy(npcFilePath, $"{npcFilePath}.bak");
        }

        private string GetNPCFilePath(string npcDirectory)
        {
            return GetNPCFilePath(npcDirectory, NPC_Instance.Value.Value);
        }

        private string GetNPCFilePath(string npcDirectory, int instance)
        {
            string npcFile = npcDirectory;

            int index = (((ComboBoxItem)NPC_Select.SelectedItem).Content as NPC).Index; //.npcid

            npcFile += $"n{index:D3}_{instance:D3}.sobj";

            return npcFile;
        }

        private string GetNPCDirectory()
        {
            string npcDirectory = curDir;

            npcDirectory = (Room_Select.SelectedItem as Room).Path;

            int index = (((ComboBoxItem)NPC_Select.SelectedItem).Content as NPC).Index; //.npcid

            npcDirectory += $"n{index:D3}\\";

            return npcDirectory;
        }

        private void NPCRoom_Select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisableControls();

            NPC_Instance.Value = 0;
        }

        private void NPC_Instance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DisableControls();
        }

        private void DisableControls()
        {
            X_Position.IsEnabled = false;
            Y_Position.IsEnabled = false;
            Z_Position.IsEnabled = false;

            X_Rotation.IsEnabled = false;
            Y_Rotation.IsEnabled = false;
            Z_Rotation.IsEnabled = false;

            X_Position.Value = 0;
            Y_Position.Value = 0;
            Z_Position.Value = 0;

            X_Rotation.Value = 0;
            Y_Rotation.Value = 0;
            Z_Rotation.Value = 0;

            Animation_Select.IsEnabled = false;
            Animation_Select.SelectedItem = null;

            Delete_Button.IsEnabled = false;
            Save_Button.IsEnabled = false;

            NPC_Instance.IsEnabled = NPC_Select.SelectedItem != null && Room_Select.SelectedItem != null;

            Select_Button.IsEnabled = NPC_Instance.IsEnabled && NPC_Instance.Value != null;
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            string npcDirectory = GetNPCDirectory();
            string npcPath = GetNPCFilePath(npcDirectory);

            string message = $"Are you sure you want to delete \"{npcPath}\"?";
            MessageBoxResult result = MessageBox.Show(message, "Delete NPC?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result.Equals(MessageBoxResult.Yes))
            {
                DeleteNPC(npcPath, npcDirectory);
            }
        }

        private void DeleteNPC(string npcPath, string npcDirectory)
        {
            Delete_Button.IsEnabled = false;

            string npcBackup = $"{npcPath}.bak";

            DeleteNPCFile(npcPath, npcBackup);
            DeleteNPCDirectory(npcDirectory);
        }

        private static void DeleteNPCFile(string npcPath, string npcBackup)
        {
            if (!File.Exists(npcPath))
                return;
            
            File.Delete(npcPath);

            if (File.Exists(npcBackup)) File.Delete(npcBackup);

            MessageBox.Show($"Deleted \"{npcPath}\".", "NPC Deleted!", MessageBoxButton.OK, MessageBoxImage.Exclamation);            
        }

        private static void DeleteNPCDirectory(string npcDirectory)
        {
            if (Directory.GetFiles(npcDirectory).Length <= 0)
                Directory.Delete(npcDirectory);
        }

        private void Colour_Help_Button_Click(object sender, RoutedEventArgs e)
        {
            string colourInfo =
                "Red: Possible Crash\n\n" +
                "Pink: NPC is likely important to story and might mess something up. Don't @ me\n\n" +
                "Yellow: Interacting with NPC might push you out of bounds\n\n" +
                "Gray: Possibly doesn't appear, but won't crash the game. Can use their animations on other NPCs\n" +
                "      Shades from darkest to lightest: missing in all > missing in rooms > missing in just research\n\n" + 
                "Purple: Animation does not loop\n\n" +
                "Blue: Animation either unkown or doesn't exist, can use animation from a different NPC\n\n" +
                "Green: Likely a dummy NPC that won't normally appear in game. Also fits Blue\n\n" +
                "Dark Red: Can be used to cheat, disabled by default";

            MessageBox.Show(colourInfo, "Colour Key", MessageBoxButton.OK, MessageBoxImage.Question);
        }
    }
}