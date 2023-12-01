using BrowserService.Interfaces;
using Infrastructure.Database.FileSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrowserService.Services
{
    public class FileBrowserService : IFileBrowserService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FileBrowserService> _logger;

        public FileBrowserService(IServiceScopeFactory scopeFactory, ILogger<FileBrowserService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<string> SearchFileSystem(string? input)
        {
            string folderPath = "/";
            string fileName = "";
            string result = "/";

            if (!string.IsNullOrEmpty(input))
            {
                var cleanedInput = input.Trim();
                if (cleanedInput.Length > 0 && cleanedInput[0] == '/')
                {
                    cleanedInput = cleanedInput.TrimStart('/');
                }

                fileName = cleanedInput.Substring(cleanedInput.LastIndexOf('/') + 1);
                folderPath = cleanedInput.Substring(0, cleanedInput.LastIndexOf('/') + 1);

                //search file within folder
                if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(folderPath))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<FileSystemDbContext>();

                    var path = await dbContext.PathDb.FirstOrDefaultAsync(x => x.FolderPath == folderPath);
                    if (path != null)
                    {
                        var files = await dbContext.FileDb.Where(x => x.PathId == path.Id && x.FileName.StartsWith(fileName)).ToListAsync();
                        if (files.Count > 0)
                        {
                            foreach (var file in files)
                            {
                                if (result == "/")
                                {
                                    result = "";
                                }

                                result += "/" + path.FolderPath + file.FileName + "\n";
                            }
                        }
                    }
                }
                //search for file within root
                else if (!string.IsNullOrEmpty(fileName))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<FileSystemDbContext>();

                    var files = await dbContext.FileDb.Where(x => x.FileName.StartsWith(fileName) && x.Path == null).ToListAsync();

                    foreach (var file in files)
                    {
                        if (result == "/")
                        {
                            result = "";
                        }

                        result += "/" + file.FileName + "\n";
                    }
                }
            }
            return result;
        }

        public async Task<string> CreateFileOrFolder(string input)
        {
            string folderPath = "/";
            string fileName = "";
            string result = "/";

            if (!string.IsNullOrEmpty(input))
            {
                var cleanedInput = input.Trim();
                if (cleanedInput.Length > 0 && cleanedInput[0] == '/')
                {
                    cleanedInput = cleanedInput.TrimStart('/');
                }

                fileName = cleanedInput.Substring(cleanedInput.LastIndexOf('/') + 1);
                folderPath = cleanedInput.Substring(0, cleanedInput.LastIndexOf('/') + 1);

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FileSystemDbContext>();

                var folderPathExists = await dbContext.PathDb.AnyAsync(x => x.FolderPath == folderPath);


                //create folder only
                if (!folderPathExists && !string.IsNullOrEmpty(folderPath) && string.IsNullOrEmpty(fileName))
                {
                    await dbContext.PathDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.PathDb()
                    {
                        FolderPath = folderPath
                    });
                    result += folderPath;
                }
                //create file and folder
                else if (!folderPathExists && !string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var newFolderPath = await dbContext.PathDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.PathDb()
                    {
                        FolderPath = folderPath
                    });

                    var file = await dbContext.FileDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.FileDb()
                    {
                        FileName = fileName,
                        PathId = newFolderPath.Entity.Id,
                        Path = newFolderPath.Entity
                    });

                    result += folderPath + fileName;
                }
                //create file in folder
                else if (folderPathExists && !string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var existingFolderPath = await dbContext.PathDb.FirstOrDefaultAsync(x => x.FolderPath == folderPath);

                    if (existingFolderPath != null)
                    {
                        await dbContext.FileDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.FileDb()
                        {
                            FileName = fileName,
                            PathId = existingFolderPath.Id,
                            Path = existingFolderPath
                        });
                        result += folderPath + fileName;
                    }
                }
                //create file only
                else if (!folderPathExists && string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    await dbContext.FileDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.FileDb()
                    {
                        FileName = fileName
                    });
                    result += fileName;
                }

                await dbContext.SaveChangesAsync();
            }
            return result;
        }

        public async Task<string> DeleteFileOrFolder(string input)
        {
            string folderPath = "/";
            string fileName = "";
            string result = "/";

            if (!string.IsNullOrEmpty(input))
            {
                var cleanedInput = input.Trim();
                if (cleanedInput.Length > 0 && cleanedInput[0] == '/')
                {
                    cleanedInput = cleanedInput.TrimStart('/');
                }

                fileName = cleanedInput.Substring(cleanedInput.LastIndexOf('/') + 1);
                folderPath = cleanedInput.Substring(0, cleanedInput.LastIndexOf('/') + 1);

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FileSystemDbContext>();

                var folderPathExists = await dbContext.PathDb.AnyAsync(x => x.FolderPath == folderPath);


                //remove folder only
                if (folderPathExists && !string.IsNullOrEmpty(folderPath) && string.IsNullOrEmpty(fileName))
                {
                    var folderForRemoval = dbContext.PathDb.FirstOrDefault(x => x.FolderPath == folderPath);

                    if (folderForRemoval != null)
                    {
                        foreach (var file in folderForRemoval.Files)
                        {
                            if (file != null)
                            {
                                dbContext.FileDb.Remove(file);
                            }
                        }

                        dbContext.PathDb.Remove(folderForRemoval);
                        result += folderPath;
                    }
                }
                //remove file in folder
                else if (folderPathExists && !string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var folderForRemoval = dbContext.PathDb.FirstOrDefault(x => x.FolderPath == folderPath);

                    if (folderForRemoval != null)
                    {
                        var fileForRemoval = dbContext.FileDb.FirstOrDefault(x => x.FileName == fileName);

                        if (fileForRemoval != null)
                        {
                            dbContext.FileDb.Remove(fileForRemoval);
                            result += folderPath + fileName;
                        }
                    }
                }
                //remove file only
                else if (!folderPathExists && string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var fileForRemoval = dbContext.FileDb.FirstOrDefault(x => x.FileName == fileName);

                    if (fileForRemoval != null)
                    {
                        dbContext.FileDb.Remove(fileForRemoval);
                        result += fileName;
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            return result;
        }
    }
}
