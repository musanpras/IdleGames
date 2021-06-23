using System.IO;

    public static class IOUtil
    {
        public static bool ExistsFile(string _path)
        {
            return File.Exists(_path);
        }
        public static void SaveToFile(string _path, byte[] _data)
        {
            if (!File.Exists(_path))
            {
                CreateFileFolder(_path);
            }
            using (FileStream fileStream = new FileStream(_path, FileMode.Create))
            {
                fileStream.Write(_data, 0, _data.Length);
            }
        }
        public static byte[] LoadBytesFromFile(string _path)
        {
            if (!File.Exists(_path))
            {
                return null;
            }
            using (FileStream fileStream = new FileStream(_path, FileMode.Open))
            {
                byte[] array = new byte[(int)fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
                return array;
            }
        }
        public static void SaveToFile(string _path, string _data)
        {
            if (!File.Exists(_path))
            {
                CreateFileFolder(_path);
            }
            StreamWriter streamWriter = new StreamWriter(File.Open(_path, FileMode.Create));
            streamWriter.AutoFlush = true;
            streamWriter.Write(_data);
            streamWriter.Close();
        }
        public static string LoadStringFromFile(string _path)
        {
            if (!File.Exists(_path))
            {
                return null;
            }
            StreamReader streamReader = new StreamReader(File.Open(_path, FileMode.Open));
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }
        public static void RemoveFile(string _path)
        {
            File.Delete(_path);
        }
        private static void CreateFileFolder(string _filePath)
        {
            string text = _filePath.Substring(0, _filePath.LastIndexOf('/'));
            if (!string.IsNullOrEmpty(text))
            {
                Directory.CreateDirectory(text);
            }
        }
        public static string[] ListFiles(string folder, string extension, bool justFilename)
        {
            string[] files = Directory.GetFiles(folder, "*" +extension);
            if (justFilename)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileNameWithoutExtension(files[i]);
                }
            }
            return files;
        }
        public static string[] ListFolders(string folder, bool justFolderName)
        {
            string[] directories = Directory.GetDirectories(folder);
            if (justFolderName)
            {
                for (int i = 0; i < directories.Length; i++)
                {
                    directories[i] = directories[i].Substring(directories[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
                }
            }
            return directories;
        }
    }

