using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;

namespace CGFReader
{
    public class CGF_Reader
    {
        private readonly List<Files> FilesList = new List<Files>();

        public CGF_Reader(string path)
        {
            List<string> compressed_file = new List<string>();
            using (var fileStream = new FileStream(path, FileMode.Open))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                    compressed_file.Add(line);
            }
            foreach (string file_string in compressed_file)
            {
                try
                {
                    string[] date = file_string.Split(':');
                    int index = Convert.ToInt32(date[0].Trim('"'));
                    string name = date[1].Trim('"');
                    byte[] bytes = Convert.FromBase64String(date[2].Trim('"'));
                    Files file = new Files(index, name, bytes);
                    FilesList.Add(file);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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