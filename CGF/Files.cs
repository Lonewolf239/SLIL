namespace CGFReader
{
    public class Files
    {
        public int INDEX { get; set; }
        public string NAME { get; set; }
        public byte[] BYTES { get; set; }
        public Files(int index, string name, byte[] bytes)
        {
            INDEX = index;
            NAME = name;
            BYTES = bytes;
        }
    }
}