using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Common.Utils
{
    public static class FileStorageUtils
    {
        public static string FixFileName(this string fileName) => 
            fileName.Replace("\\", "/");

        public static void DeletePersistentDirectory(string dirPath)
        {
            var path = Path.Combine(Application.persistentDataPath, dirPath);
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
        }

        public static void SaveFileToPersistent(string folder, string fileName, string data)
        {
            var path = PrepareFolder(folder);
            path = Path.Combine(path, fileName);
            File.WriteAllText(path, data);
        }

        private static string PrepareFolder(string folder)
        {
            var path = Path.Combine(Application.persistentDataPath, folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public static void SaveFileToPersistent(string folder, string fileName, byte[] bytes)
        {
            string path = PrepareFolder(folder);
            path = Path.Combine(path, fileName);
            File.WriteAllBytes(path, bytes);
        }

        public static string LoadFileFromPersistent(string folder, string fileName, bool remove = false)
        {
            var path = Path.Combine(Application.persistentDataPath, folder);
            path = Path.Combine(path, fileName);

            if (File.Exists(path))
            {
                if (remove)
                {
                    File.Delete(path);
                    return null;
                }

                return File.ReadAllText(path, Encoding.UTF8);
            }

            return null;
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }


        private static object ByteArrayToObject(byte[] arrBytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }

        public static void SaveTextFile(string path, string data)
        {
            File.WriteAllText(path, data, Encoding.UTF8);
        }

        public static string LoadTextFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }

        public static void DeleteAllPersistent()
        {
            var dirInfo = new DirectoryInfo(Application.persistentDataPath);

            foreach (var fileInfo in dirInfo.GetFiles())
            {
                try
                {
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            foreach (var directoryInfo in dirInfo.GetDirectories())
            {
                try
                {
                    if (directoryInfo.Exists)
                    {
                        directoryInfo.Delete(true);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        public static void DeleteFileInCache(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(path))
                File.Delete(path);
            else
                Debug.LogWarning("No file: " + path);
        }


        public static int GetPersistenceFileCount(string relativePath)
        {
            var path = Path.Combine(Application.persistentDataPath, relativePath);

            var directory = new DirectoryInfo(path);
            return !directory.Exists ? 0 : directory.GetFiles().Length;
        }
    }
}