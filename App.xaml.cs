using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace MHWRoommates
{
    public partial class App : Application
    {
        private readonly string curDir = Directory.GetCurrentDirectory();
        private NPCList npcList;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool skipExeCheck = e.Args.Length > 0 && e.Args[0].Equals("skip-exe-check");

            if ((!skipExeCheck && IsMonHunEXEMissing()) || IsReqdFilesMissing()) return;

            LoadNPCList();

            MainWindow mainWindow = new MainWindow(npcList);
            EditRoomsWindow editRoomsWindow = new EditRoomsWindow(npcList);
            mainWindow.Show();
            editRoomsWindow.Owner = mainWindow;
            editRoomsWindow.Top = mainWindow.Top + mainWindow.Height;
            editRoomsWindow.Left = mainWindow.Left;
            editRoomsWindow.Show();
        }

        private void LoadNPCList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NPCList));
            using (FileStream fileStream = new FileStream(RMFiles.NPCLIST, FileMode.Open))
            {
                npcList = (NPCList)serializer.Deserialize(fileStream);
            }
            npcList.setNPCIndexes();
        }

        private bool IsMonHunEXEMissing()
        {
            bool monHunEXEMissing = false;

            string[] parentDirFiles = Directory.GetFiles(Directory.GetParent(curDir).FullName);
            string[] parentFileNames = parentDirFiles.Select(file => Path.GetFileName(file)).ToArray();

            if (!parentFileNames.Contains("MonsterHunterWorld.exe"))
            {
                monHunEXEMissing = true;
                const string errorMessage = "\"MonsterHunterWorld.exe\" not found in parent directory!";
                MessageBox.Show(errorMessage, "MonHun EXE Missing!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return monHunEXEMissing;
        }

        private bool IsReqdFilesMissing()
        {
            bool isMissingFile = false;

            string missingFile = "";
            if (!File.Exists(RMFiles.FSM)) { missingFile = RMFiles.FSM; }
            else
            if (!File.Exists(RMFiles.SOBJ)) { missingFile = RMFiles.SOBJ; }
            else
            if (!File.Exists(RMFiles.SOBJL)) { missingFile = RMFiles.SOBJL; }
            else
            if (!File.Exists(RMFiles.NPCLIST)) { missingFile = RMFiles.NPCLIST; }

            if (!missingFile.Equals(""))
            {
                isMissingFile = true;
                MessageBox.Show($"\"{missingFile}\" not found!", "Important File Missing!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isMissingFile;
        }
    }
}