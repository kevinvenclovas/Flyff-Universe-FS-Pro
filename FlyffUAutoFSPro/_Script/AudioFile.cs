using System.IO;

namespace FlyffUAutoFSPro._Script
{
    public class AudioFile
    {
        public int ID { get; set ;}
        public UnmanagedMemoryStream Stream { get; set; }
        public string Name { get; set; }
    }
}
