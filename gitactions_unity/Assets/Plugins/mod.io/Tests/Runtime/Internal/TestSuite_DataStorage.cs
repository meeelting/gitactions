using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.TestTools;
using NUnit.Framework;

using ModIO;
using ModIO.Implementation;
using ModIO.Implementation.Platform;

namespace ModIOTesting.Internal
{
    internal class TestSuite_DataStorage
    {
        const long FileSize_1MiB = 1024 * 1024;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            // - initialize DataStorage -
            long gameId = 0;
            BuildSettings settings = new BuildSettings() {
                extData = new List<string>(),
            };

            var udsTask = PlatformConfiguration.CreateUserDataService("TEST", gameId, settings);
            while(!udsTask.IsCompleted) { yield return null; }
            DataStorage.user = udsTask.GetValue();

            var pdsTask = PlatformConfiguration.CreatePersistentDataService(gameId, settings);
            while(!pdsTask.IsCompleted) { yield return null; }
            DataStorage.persistent = pdsTask.GetValue();

            var tdsTask = PlatformConfiguration.CreateTempDataService(gameId, settings);
            while(!tdsTask.IsCompleted) { yield return null; }
            DataStorage.temp = tdsTask.GetValue();

            // - create test directories -
            if(Directory.Exists(DataStorage.user.RootDirectory))
            {
                Directory.Delete(DataStorage.user.RootDirectory, true);
            }
            Directory.CreateDirectory(DataStorage.user.RootDirectory);

            if(Directory.Exists(DataStorage.persistent.RootDirectory))
            {
                Directory.Delete(DataStorage.persistent.RootDirectory, true);
            }
            Directory.CreateDirectory(DataStorage.persistent.RootDirectory);

            if(Directory.Exists(DataStorage.temp.RootDirectory))
            {
                Directory.Delete(DataStorage.temp.RootDirectory, true);
            }
            Directory.CreateDirectory(DataStorage.temp.RootDirectory);

            yield break;
        }

        [UnityTest]
        public IEnumerator IterateFilesInDirectory()
        {
            // set up
            const int FilesPerDir = 4;

            int seed = (int)DateTime.UtcNow.ToFileTime();
            string rootDir = DataStorage.persistent.RootDirectory + @"/openAll";
            Dictionary<string, byte[]> files =
                this.CreateRandomFiles(rootDir, seed, true, FilesPerDir);

            // test
            List<string> unreadFiles = new List<string>(files.Keys);

            foreach(ResultAnd<ModIOFileStream> result in DataStorage.IterateFilesInDirectory(
                        rootDir))
            {
                if(!result.result.Succeeded())
                {
                    Assert.AreEqual(ResultCode.Success, result.result.code,
                                    $"Unexpected result code [Seed={seed}]");
                }
                else
                {
                    ModIOFileStream stream = result.value;

                    Assert.That(
                        unreadFiles.Contains(stream.FilePath),
                        $"File was not listed in unread files [Seed={seed}, FilePath={stream.FilePath}]");

                    if(unreadFiles.Contains(stream.FilePath))
                    {
                        unreadFiles.Remove(stream.FilePath);
                    }
                }
            }

            Assert.AreEqual(new List<string>(), unreadFiles,
                            $"Files left in unread files list [Seed={seed}]");

            yield break;
        }

        // ---------[ Utility ]---------
        Dictionary<string, byte[]> CreateRandomFiles(string rootDirectory, int seed,
                                                     bool createNestedDirs, int filesPerDirectory)
        {
            var random = new System.Random(seed);

            // generate directories
            string[] dirs;

            if(!createNestedDirs)
            {
                dirs = new string[] { rootDirectory };
            }
            else
            {
                dirs = new string[] {
                    rootDirectory, // 00
                    rootDirectory + @"/A01", // 01
                    rootDirectory + @"/A01/B01", // 02
                    rootDirectory + @"/A01/B01/C01", // 03
                    rootDirectory + @"/A01/B02", // 04
                    rootDirectory + @"/A02", // 05
                    rootDirectory + @"/EMPTY", // 06
                };
            }

            // generate files
            var files = new Dictionary<string, byte[]>();

            for(int dir_i = 0; dir_i < dirs.Length - 1; ++dir_i)
            {
                for(int file_i = 0; file_i < filesPerDirectory; ++file_i)
                {
                    string fileIndex = (dir_i * filesPerDirectory + file_i).ToString("000");
                    string filePath = Path.GetFullPath($@"{dirs[dir_i]}/FILE_{fileIndex}.test");

                    byte[] fileContent = new byte[TestSuite_DataStorage.FileSize_1MiB];
                    random.NextBytes(fileContent);

                    files.Add(filePath, fileContent);
                }
            }

            // create dirs
            this.CreateDirectories(dirs);

            // write files
            this.WriteFiles(files);

            return files;
        }

        void CreateDirectories(IEnumerable<string> directories)
        {
#if UNITY_EDITOR

            foreach(var path in directories)
            {
                if(!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }

#endif // UNITY_EDITOR
        }

        void WriteFiles(IEnumerable<KeyValuePair<string, byte[]>> files)
        {
#if UNITY_EDITOR

            foreach(var kvp in files) { System.IO.File.WriteAllBytes(kvp.Key, kvp.Value); }

#endif // UNITY_EDITOR
        }
    }
}
