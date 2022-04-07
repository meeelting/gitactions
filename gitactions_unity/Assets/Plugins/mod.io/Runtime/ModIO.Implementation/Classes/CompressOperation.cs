using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using JetBrains.Annotations;

namespace ModIO.Implementation
{
    /// <summary>
    /// Acts as a wrapper to handle a zip extraction operation. Can be cached for cancelling,
    /// pausing, etc
    /// </summary>
    internal class CompressOperation : IModIOZipOperation
    {
        public Task<ResultAnd<MemoryStream>> operation;

        public Task currentOperation
        {
            get {
                return operation;
            }
        }

        public bool sizeLimitReached;

        bool cancel;

        public string directory;
        public ProgressHandle progressHandle;

        public CompressOperation(string directory, [CanBeNull] ProgressHandle progressHandle = null)
        {
            this.directory = directory;
            this.progressHandle = progressHandle;
        }

        public async Task<ResultAnd<MemoryStream>> Compress()
        {
            operation = CompressAll();
            return await operation;
        }

        // ---------[ Interface ]---------
        /// <summary>Compresses the contents of an archive.</summary>
        async Task<ResultAnd<MemoryStream>> CompressAll()
        {
            Logger.Log(LogLevel.Verbose, $"COMPRESSING {directory}");

            // TODO @Steve create a compress method to write to disk so we can return a FileStream
            // (this will mean we can use less memory when submitting the Binary data to a WWWForm)

            // TODO @Steve scan to determine entire stream size for accurate progress tracking

            ResultAnd<MemoryStream> resultAnd = new ResultAnd<MemoryStream>();
            resultAnd.value = new MemoryStream();

            using(ZipOutputStream stream = new ZipOutputStream(resultAnd.value))
            {
                stream.SetLevel(3);

                int folderOffset = directory.Length + (directory.EndsWith("\\") ? 0 : 1);

                var directories = DataStorage.IterateFilesInDirectory(directory);

                foreach(var dir in directories)
                {
                    if(dir.result.Succeeded() && !cancel && !ModIOUnityImplementation.shuttingDown)
                    {
                        using(dir.value)
                        {
                            // Make the name in zip based on the folder
                            // eg,
                            // Library/Application
                            // Support/DefaultCompany/Shooter/mods/BobsMod/items/entryName

                            // should become:

                            // BobsMod/items/entryName
                            string entryName = dir.value.FilePath.Substring(folderOffset);

                            // Remove drive from name and fix slash direction
                            entryName = ZipEntry.CleanName(entryName);

                            ZipEntry newEntry = new ZipEntry(entryName);

                            stream.PutNextEntry(newEntry);

                            int size;
                            long max = dir.value.Length;
                            byte[] data = new byte[4096];
                            while(true)
                            {
                                // TODO @Jackson ensure ReadAsync and WriteAsync are
                                // implemented on all filestream wrappers
                                size = await dir.value.ReadAsync(data, 0, data.Length);
                                if(size > 0)
                                {
                                    await stream.WriteAsync(data, 0, size);
                                    if(progressHandle != null)
                                    {
                                        // This is only the progress for the current entry
                                        progressHandle.Progress = stream.Position / (float)max;
                                    }
                                }
                                else
                                {
                                    break;                                    
                                }
                            }

                            stream.CloseEntry();
                            
                        }
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error,
                                   cancel ? "Cancelled compress operation"
                                          : $"Failed to compress files at directory: "
                                                + $"{directory}\nResult[{dir.result.code}])");
                        goto Cancel;
                    }
                }

                if(cancel || ModIOUnityImplementation.shuttingDown)
                {
                    goto Cancel;
                }

                stream.IsStreamOwner = false;

                resultAnd.result = ResultBuilder.Success;
            }

            Logger.Log(LogLevel.Verbose, $"COMPRESSED [{resultAnd.result.code}] {directory}");
            return resultAnd;

        // This is a GOTO cleanup if the compress operation is cancelled
        Cancel:

            Logger.Log(LogLevel.Verbose,
                       $"FAILED COMPRESSION [{resultAnd.result.code}] {directory}");
            // no need to .Dispose() mem streams, this happens automatically when using goto in a
            // using block because it's technically a try-catch-finally

            resultAnd.result = sizeLimitReached
                                   ? ResultBuilder.Create(ResultCode.IO_FileSizeTooLarge)
                                   : ResultBuilder.Create(ResultCode.Internal_OperationCancelled);

            return resultAnd;
        }

        // Implemented from IModIOZipOperation interface
        public void Cancel()
        {
            cancel = true;
        }

        // Implemented from IDisposable interface
        public void Dispose()
        {
            operation?.Dispose();
        }
    }
}
