using System.IO;

namespace MHWRoommates
{
    class Room
    {
        public string Name { get; }
        public string ID { get; }
        public string Path { get; }
        public string[] SOBJPaths { get; }
        public Point3D DefaultPosition { get; }
        public Point3D DefaultRotation { get; }

        public Room(string name, string id, string path, string[] sobjs, Point3D position, Point3D rotation)
        {
            Name = name;
            ID = id;
            Path = Directory.GetParent(Directory.GetCurrentDirectory()) + path;
            DefaultPosition = position;
            DefaultRotation = rotation;

            SOBJPaths = new string[sobjs.Length];
            for (int i = 0; i < sobjs.Length; i++)
            {
                SOBJPaths[i] = $"{Path}sobjl\\st{ID}_sn{sobjs[i]}.sobjl";
            }
        }

        public struct Point3D
        {
            public float x, y, z;
        }

        public override string ToString()
        {
            return Name;
        }

        public static readonly Room LIVING_QUARTERS = new Room(
            "Living Quarters",
            "501",
            "\\nativePC\\village\\st501\\",
            new string[] {
                "120000",
                "120800",
                "210000",
                "320000",
                "400000",
                "410000",
                "420000",
                "430000",
                "520000",
                "600000",
                "800000",
                "810000",
                "900000" },
            new Point3D { x = 10207.8134765625f, y = 1601.39001464844f, z = 9.34542846679688f },
            new Point3D { x = 0f, y = 88.3622589111328f, z = 0f });

        public static readonly Room PRIVATE_QUARTERS = new Room(
            "Private Quarters",
            "502",
            "\\nativePC\\village\\st502\\",
            new string[] { "010000", "410000" },
            new Point3D { x = 12348.55078125f, y = 5838.5400390625f, z = -6231.52294921875f },
            new Point3D { x = 0f, y = -148.999847412109f, z = 0f });

        public static readonly Room PRIVATE_SUITE = new Room(
            "Private Suite",
            "503",
            "\\nativePC\\village\\st503\\",
            new string[] { "010000", "900000"},
            new Point3D { x = 17648.513671875f, y = 11554.5869140625f, z = -498.106353759766f },
            new Point3D { x = 0f, y = -2.99981689453125f, z = 0f});

        public static readonly Room RESEARCH_BASE = new Room(
            "Research Base",
            "303",
            "\\nativePC\\village\\st303\\",
            new string[] {
                "420000",
                "420750",
                "430400",
                "500000",
                "510000",
                "520000",
                "600000",
                "800000",
                "900000" },
            new Point3D { x = 1595.64f, y = -127.0225f, z = 675.9922f }, // Handler pos
            new Point3D { x = 0f, y = -142.4786f, z = 0f });
    }
}