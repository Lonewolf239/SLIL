using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;

namespace CGFReader
{
    public class CGF_Reader
    {
        private readonly List<Files> FilesList = new List<Files>();
        private string Path { get; set; }

        public CGF_Reader(string path)
        {
            Path = path;
        }

        public async Task ProcessFileAsync(IProgress<int> progress)
        {
            List<string> compressed_file = new List<string>();
            using (var fileStream = new FileStream(Path, FileMode.Open))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    compressed_file.Add(line);
                    progress?.Report((int)((fileStream.Position / (double)fileStream.Length) * 100));
                }
            }
            for (int i = 0; i < compressed_file.Count; i++)
            {
                try
                {
                    string[] date = compressed_file[i].Split(':');
                    int index = Convert.ToInt32(date[0].Trim('"'));
                    string name = date[1].Trim('"');
                    byte[] bytes = Convert.FromBase64String(date[2].Trim('"'));
                    Files file = new Files(index, name, bytes);
                    FilesList.Add(file);
                }
                catch (Exception ex) { throw ex; }
                progress?.Report((int)(((i + 1) / (double)compressed_file.Count) * 100));
            }
            compressed_file.Clear();
        }

        public byte[] GetFile(string name)
        {
            byte[] bytes = { 0 };
            foreach(Files file in FilesList)
            {
                if (file.NAME == name)
                    bytes = file.BYTES;
            }
            return bytes;
        }
    }
}