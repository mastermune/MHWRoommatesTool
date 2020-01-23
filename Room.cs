using System.IO;

namespace MHWRoommates
{
    public class Room
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
                "900000",
                "1000000",
                "1100000" },
            new Point3D { x = 10147.533203125f, y = 1601.39001464844f, z = -12.034893989563f },
            new Point3D { x = 0f, y = -31.5141468048096f, z = 0f });

        public static readonly Room PRIVATE_QUARTERS = new Room(
            "Private Quarters",
            "502",
            "\\nativePC\\village\\st502\\",
            new string[] {
                "010000",
                "410000" },
            new Point3D { x = 12275.533203125f, y = 5836.35986328125f, z = -6215.0712890625f },
            new Point3D { x = 0f, y = 74.0428695678711f, z = 0f });

        public static readonly Room PRIVATE_SUITE = new Room(
            "Private Suite",
            "503",
            "\\nativePC\\village\\st503\\",
            new string[] {
                "010000",
                "900000" },
            new Point3D { x = 17703.70703125f, y = 11554.5869140625f, z = -449.223327636719f },
            new Point3D { x = 0f, y = -174.542831420898f, z = 0f});

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

        public static readonly Room SELIANA_SUITE = new Room(
            "Seliana Suite",
            "506",
            "\\nativePC\\village\\st506\\",
            new string[] { "0000000" },
            new Point3D { x = 938.647766113281f, y = 2853.37548828125f, z = 615.363586425781f },
            new Point3D { x = 0f, y = -111.148902893066f, z = 0f });

        public static readonly Room SELIANA_PUB = new Room(
            "Seliana Pub",
            "306",
            "\\nativePC\\village\\st306\\",
            new string[] { "0000000" },
            new Point3D { x = 0, y = 0, z = 0 },
            new Point3D { x = 0, y = 0, z = 0});
    }
}