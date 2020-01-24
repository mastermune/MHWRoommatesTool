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

        public Room(string name, string id, string path, string[] sobjs, Point3D position)
        {
            Name = name;
            ID = id;
            Path = Directory.GetParent(Directory.GetCurrentDirectory()) + path;
            DefaultPosition = position;

            SOBJPaths = new string[sobjs.Length];
            for (int i = 0; i < sobjs.Length; i++)
            {
                SOBJPaths[i] = $"{Path}sobjl\\st{ID}_sn{sobjs[i]}.sobjl";
            }
        }

        public struct Point3D
        {
            public float x, y, z, r;
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
            new Point3D { // Housekeeper
                x = 10147.533203125f,
                y = 1601.39001464844f,
                z = -12.034893989563f,
                r = -31.5141468048096f
            });

        public static readonly Room PRIVATE_QUARTERS = new Room(
            "Private Quarters",
            "502",
            "\\nativePC\\village\\st502\\",
            new string[] {
                "010000",
                "410000" },
            new Point3D { // Housekeeper
                x = 12275.533203125f,
                y = 5836.35986328125f,
                z = -6215.0712890625f,
                r = 74.0428695678711f 
            });

        public static readonly Room PRIVATE_SUITE = new Room(
            "Private Suite",
            "503",
            "\\nativePC\\village\\st503\\",
            new string[] {
                "010000",
                "900000" },
            new Point3D { // Housekeeper
                x = 17703.70703125f,
                y = 11554.5869140625f,
                z = -449.223327636719f,
                r = -174.542831420898f
            });

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
            new Point3D { // The Handler
                x = 1595.64f,
                y = -127.0225f,
                z = 675.9922f,
                r = -142.4786f
            });

        public static readonly Room SELIANA_SUITE = new Room(
            "Seliana Suite",
            "506",
            "\\nativePC\\village\\st506\\",
            new string[] { "0000000" },
            new Point3D { // Seliana Housekeeper
                x = 938.647766113281f,
                y = 2853.37548828125f,
                z = 615.363586425781f,
                r = -111.148902893066f
            });

        public static readonly Room SELIANA = new Room(
            "Seliana",
            "305",
            "\\nativePC\\village\\st305\\",
            new string[] {
                "0000000",
                "0900000",
                "1100300",
                "1200000",
                "1210000",
                "1300100",
                "1310000",
                "1330000",
                "1330300",
                "1400000",
                "1400200",
                "1410000",
                "1410100",
                "1420000",
                "1420100",
                "1430000",
                "1430250",
                "1500000",
                "1510000",
                "1510100",
                "1520000",
                "1520100",
                "1600000",
                "1700000",
                "1705000",
                "1710000",
                "1710100" },
            new Point3D
            { // The Handler
                x = -393.5f,
                y = 3105f,
                z = -3784f,
                r = -122f
            });

        public static readonly Room SELIANA_PUB = new Room(
            "Seliana Pub",
            "306",
            "\\nativePC\\village\\st306\\",
            new string[] { "0000000" },
            new Point3D { // Pub Event Manager
                x = -5282.1474609375f,
                y = 4552.3203125f,
                z = -4996.09326171875f,
                r = 87.1305770874023f
            });
    }
}