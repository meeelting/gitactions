#if UNITY_SWITCH || (MODIO_COMPILE_ALL && UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using nn.fs;

#pragma warning disable 1998 // These async functions don't use await!

namespace ModIO.Implementation.Platform
{
    /// <summary>Switch implementation of the various data services.</summary>
    internal class SwitchDataService : IUserDataService, IPersistentDataService, ITempDataService
    {
#region Data

        /// <summary>Root directory for the data service.</summary>
        string rootDir = null;

        /// <summary>Root directory for the data service.</summary>
        public string RootDirectory
        {
            get {
                return this.rootDir;
            }
        }

#endregion // Data

#region Initialization

        /// <summary>Init as IUserDataService.</summary>
        async Task<Result> IUserDataService.InitializeAsync(string userProfileIdentifier,
                                                            long gameId, BuildSettings settings)
        {
            string mountPoint = settings.GetSwitchUserDataMountPoint();
            if(string.IsNullOrEmpty(mountPoint))
            {
                Result r = ResultBuilder.Create(ResultCode.IO_InvalidMountPoint);
                return r;
            }

            // TODO(@jackson): Test mount point
            // TODO(@jackson): Check trailing slash

            this.rootDir = $@"{mountPoint}/mod.io/{userProfileIdentifier}";

            Logger.Log(LogLevel.Verbose, "Initialized SwitchDataService for User Data: " + rootDir);

            // TODO(@jackson): Test dir creation

            return ResultBuilder.Success;
        }

        /// <summary>Init as IPersistentDataService.</summary>
        async Task<Result> IPersistentDataService.InitializeAsync(long gameId,
                                                                  BuildSettings settings)
        {
            string mountPoint = settings.GetSwitchPersistentDataMountPoint();
            if(string.IsNullOrEmpty(mountPoint))
            {
                Result r = ResultBuilder.Create(ResultCode.IO_InvalidMountPoint);
                return r;
            }

            // TODO(@jackson): Test mount point
            // TODO(@jackson): Check trailing slash

            this.rootDir = $@"{mountPoint}/mod.io";

            Logger.Log(LogLevel.Verbose,
                       "Initialized SwitchDataService for Persistent Data: " + rootDir);

            // TODO(@jackson): Test dir creation

            return ResultBuilder.Success;
        }

        /// <summary>Init as ITempDataService.</summary>
        async Task<Result> ITempDataService.InitializeAsync(long gameId, BuildSettings settings)
        {
            string mountPoint = settings.GetSwitchTempDataMountPoint();
            if(string.IsNullOrEmpty(mountPoint))
            {
                Result r = ResultBuilder.Create(ResultCode.IO_InvalidMountPoint);
                return r;
            }

            // TODO(@jackson): Test mount point
            // TODO(@jackson): Check trailing slash

            this.rootDir = $@"{mountPoint}/mod.io";

            Logger.Log(LogLevel.Verbose, "Initialized SwitchDataService for Temp Data: " + rootDir);

            // TODO(@jackson): Test dir creation

            return ResultBuilder.Success;
        }

#endregion // Initialization

#region Operations

        /// <summary>Opens a file stream for reading.</summary>
        public ModIOFileStream OpenReadStream(string filePath, out Result result)
        {
            // TODO(@jackson): Translate results

            DebugUtil.AssertPathValid(filePath, this.rootDir);

            nn.Result nnResult;
            FileHandle handle = new FileHandle();

            // check file exists
            if(!this.FileExists(filePath, out result))
            {
                return null;
            }

            // open
            nnResult = File.Open(ref handle, filePath, OpenFileMode.Read);
            if(!nnResult.IsSuccess())
            {
                result = ResultBuilder.Create(ResultCode.IO_FileCouldNotBeRead);
                return null;
            }


            // finalize
            SwitchFileStream stream = new SwitchFileStream(handle);
            result = ResultBuilder.Success;

            return stream;
        }

        /// <summary>Opens a file stream for writing.</summary>
        public ModIOFileStream OpenWriteStream(string filePath, out Result result)
        {
            // TODO(@jackson): Translate results

            DebugUtil.AssertPathValid(filePath, this.rootDir);

            nn.Result nnResult;
            FileHandle handle = new FileHandle();

            // delete old file
            nnResult = File.Delete(filePath);
            if(!nnResult.IsSuccess() && !FileSystem.ResultPathNotFound.Includes(nnResult))
            {
                result = ResultBuilder.Create(ResultCode.IO_FileCouldNotBeCreated);
                return null;
            }

            // ensure directory exists
            if(!this.TryCreateParentDirectory(filePath, out result))
            {
                return null;
            }

            // create the file
            nnResult = File.Create(filePath, 0L);
            if(!nnResult.IsSuccess())
            {
                result = ResultBuilder.Create(ResultCode.IO_FileCouldNotBeCreated);
                return null;
            }

            // open the file
            nnResult =
                File.Open(ref handle, filePath, OpenFileMode.Write | OpenFileMode.AllowAppend);
            if(!nnResult.IsSuccess())
            {
                result = ResultBuilder.Create(ResultCode.IO_FileCouldNotBeOpened);
                return null;
            }

            // finalize
            SwitchFileStream stream = new SwitchFileStream(handle);
            result = ResultBuilder.Success;

            return stream;
        }

        public async Task<ResultAnd<byte[]>> ReadFileAsync(string filePath)
        {
            DebugUtil.AssertPathValid(filePath, this.rootDir);

            byte[] data = null;
            long fileSize = -1;
            nn.Result result;

            FileHandle handle = new FileHandle();

            // open
            result = File.Open(ref handle, filePath, OpenFileMode.Read);
            if(!result.IsSuccess())
            {
                if(FileSystem.ResultPathNotFound.Includes(result))
                {
                    return ResultAnd.Create(ResultCode.IO_FileDoesNotExist, data);
                }
                else if(FileSystem.ResultTargetLocked.Includes(result))
                {
                    return ResultAnd.Create(ResultCode.IO_AccessDenied, data);
                }
                else
                {
                    Logger.Log(LogLevel.Warning, "Unhandled error when attempting to open file."
                                                     + $"\n.path={filePath}"
                                                     + $"\n.nn.Result:{result.ToString()}");
                    return ResultAnd.Create(ResultCode.Unknown, data);
                }
            }

            // get size
            result = File.GetSize(ref fileSize, handle);
            if(!result.IsSuccess())
            {
                File.Close(handle);
                data = null;

                Logger.Log(LogLevel.Warning, "Unhandled error when attempting to get file size."
                                                 + $"\n.path={filePath}"
                                                 + $"\n.nn.Result:{result.ToString()}");
                return ResultAnd.Create(ResultCode.Unknown, data);
            }

            // read
            result = File.Read(handle, 0, data, data.Length);
            if(!result.IsSuccess())
            {
                File.Close(handle);
                data = null;

                Logger.Log(LogLevel.Warning, "Unhandled error when attempting to read file."
                                                 + $"\n.path={filePath}"
                                                 + $"\n.nn.Result:{result.ToString()}");

                return ResultAnd.Create(ResultCode.Unknown, data);
            }

            // finalize
            File.Close(handle);
            return ResultAnd.Create(ResultCode.Success, data);
        }

        public async Task<Result> WriteFileAsync(string filePath, byte[] data)
        {
            string absolutePath = $@"{this.rootDir}/{filePath}";
            var output = await SystemIOWrapper.WriteFileAsync(absolutePath, data);
            return output;
        }

        /// <summary>Attempts to create a parent directory.</summary>
        public bool TryCreateParentDirectory(string path, out Result result)
        {
            DebugUtil.AssertPathValid(path, this.rootDir);

            string parentDir = System.IO.Path.GetDirectoryName(path);

            // check if already exists
            if(parentDir.EndsWith(":/") // Is the parent the mount point?
               || this.DirectoryExists(parentDir, out result))
            {
                result = ResultBuilder.Success;
                return true;
            }

            // At this point we can guarantee that path does not exist and that
            // the path is not the mount point.

            // Assert the parent directories are created
            if(!this.TryCreateParentDirectory(parentDir, out result))
            {
                return false;
            }

            // Create directory
            nn.Result nnResult = Directory.Create(parentDir);

            if(!nnResult.IsSuccess())
            {
                // TODO(@jackson): Translate result
                Logger.Log(LogLevel.Warning,
                           "Unhandled directory creation failure result."
                               + $"\n.path={parentDir}\n.result={nnResult.GetDescription()}");

                result = ResultBuilder.Create(ResultCode.IO_DirectoryCouldNotBeCreated);
                return false;
            }

            result = ResultBuilder.Success;
            return true;
        }

        public async Task<Result> DeleteFileAsync(string filePath)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>Deletes a directory and its contents recursively.</summary>
        public Result DeleteDirectory(string directoryPath)
        {
            DebugUtil.AssertPathValid(directoryPath, this.rootDir);

            nn.Result result = Directory.DeleteRecursively(directoryPath);

            if(result.IsSuccess() || FileSystem.ResultPathNotFound.Includes(result))
            {
                return ResultBuilder.Success;
            }
            else if(FileSystem.ResultTargetLocked.Includes(result))
            {
                return ResultBuilder.Create(ResultCode.IO_AccessDenied);
            }
            else
            {
                Logger.Log(LogLevel.Warning, "Unhandled error when attempting to delete directory."
                                                 + $"\n.directoryPath={directoryPath}"
                                                 + $"\n.nn.Result:{result.ToString()}");

                return ResultBuilder.Unknown;
            }
        }

#endregion // Operations

#region Utility

        /// <summary>Determines whether a file exists.</summary>
        public bool FileExists(string filePath)
        {
            DebugUtil.AssertPathValid(filePath, this.rootDir);

            EntryType entryType = 0;
            nn.Result result = FileSystem.GetEntryType(ref entryType, filePath);

            return (result.IsSuccess() && entryType == EntryType.File);
        }

        /// <summary>Determines whether a file exists.</summary>
        public bool FileExists(string filePath, out Result result)
        {
            DebugUtil.AssertPathValid(filePath, this.rootDir);

            EntryType entryType = 0;
            nn.Result nnResult = FileSystem.GetEntryType(ref entryType, filePath);

            if(nnResult.IsSuccess() && entryType == EntryType.File)
            {
                result = ResultBuilder.Success;
                return true;
            }
            else
            {
                result = ResultBuilder.Create(ResultCode.IO_FileDoesNotExist);
                return false;
            }
        }

        /// <summary>Checks for the existence of a directory.</summary>
        public bool DirectoryExists(string path, out Result result)
        {
            EntryType entryType = 0;
            nn.Result nnResult = FileSystem.GetEntryType(ref entryType, path);

            if(nnResult.IsSuccess() && entryType == EntryType.Directory)
            {
                result = ResultBuilder.Success;
                return true;
            }
            else
            {
                result = ResultBuilder.Create(ResultCode.IO_DirectoryDoesNotExist);
                return false;
            }
        }

        /// <summary>Gets the size and hash of a file.</summary>
        public async Task<ResultAnd<(long fileSize, string fileHash)>> GetFileSizeAndHash(
            string filePath)
        {
            ResultAnd<byte[]> readResult = await this.ReadFileAsync(filePath);
            long size = -1;
            string hash = null;

            if(!readResult.result.Succeeded())
            {
                return ResultAnd.Create(readResult.result, (size, hash));
            }

            size = readResult.value.Length;
            hash = IOUtil.GenerateMD5(readResult.value);

            return ResultAnd.Create(ResultCode.Success, (size, hash));
        }

        /// <summary>Determines whether a directory exists.</summary>
        public bool DirectoryExists(string directoryPath)
        {
            DebugUtil.AssertPathValid(directoryPath, this.rootDir);

            EntryType entryType = 0;
            nn.Result result = FileSystem.GetEntryType(ref entryType, directoryPath);

            return (result.IsSuccess() && entryType == EntryType.Directory);
        }

        /// <summary>Lists all the files in the given directory recursively.</summary>
        public ResultAnd<List<string>> ListAllFiles(string directoryPath)
        {
            throw new NotImplementedException();
        }

#endregion // Utility
    }
}

#pragma warning restore 1998 // These async functions don't use await!

#endif // UNITY_SWITCH || (MODIO_COMPILE_ALL && UNITY_EDITOR)
