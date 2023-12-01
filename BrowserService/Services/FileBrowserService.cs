using BrowserService.Interfaces;
using Infrastructure.Database.FileSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrowserService.Services
{
    public class FileBrowserService : IFileBrowserService
    {
        private readonly FileSystemDbContext _dbContext;

        public FileBrowserService(FileSystemDbContext dbContext)
        {
            _dbContext = dbContext;
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
                    var path = await _dbContext.PathDb.FirstOrDefaultAsync(x => x.FolderPath == folderPath);
                    if (path != null)
                    {
                        var files = await _dbContext.FileDb.Where(x => x.PathId == path.Id && x.FileName.StartsWith(fileName)).ToListAsync();
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
                //search for any file
                else if (!string.IsNullOrEmpty(fileName))
                {
                    var files = await _dbContext.FileDb.Where(x => x.FileName.StartsWith(fileName)).ToListAsync();

                    foreach (var file in files.Take(10))
                    {
                        if (result == "/")
                        {
                            result = "";
                        }


                        if (file.PathId == null)
                        {
                            result += "/" + file.FileName + "\n";
                        }
                        else
                        {
                            var path = _dbContext.PathDb.First(x => x.Id == file.PathId);
                            result += "/" + path.FolderPath + file.FileName + "\n";
                        }
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

                var folderPathExists = await _dbContext.PathDb.AnyAsync(x => x.FolderPath == folderPath);


                //create folder only
                if (!folderPathExists && !string.IsNullOrEmpty(folderPath) && string.IsNullOrEmpty(fileName))
                {
                    await _dbContext.PathDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.PathDb()
                    {
                        FolderPath = folderPath
                    });
                    result += folderPath;
                }
                //create file and folder
                else if (!folderPathExists && !string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var newFolderPath = await _dbContext.PathDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.PathDb()
                    {
                        FolderPath = folderPath
                    });

                    var file = await _dbContext.FileDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.FileDb()
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
                    var existingFolderPath = await _dbContext.PathDb.FirstOrDefaultAsync(x => x.FolderPath == folderPath);

                    if (existingFolderPath != null)
                    {
                        await _dbContext.FileDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.FileDb()
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
                    await _dbContext.FileDb.AddAsync(new Infrastructure.Database.FileSystem.Entities.FileDb()
                    {
                        FileName = fileName
                    });
                    result += fileName;
                }

                await _dbContext.SaveChangesAsync();
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


                var folderPathExists = await _dbContext.PathDb.AnyAsync(x => x.FolderPath == folderPath);


                //remove folder only
                if (folderPathExists && !string.IsNullOrEmpty(folderPath) && string.IsNullOrEmpty(fileName))
                {
                    var foldersForRemoval = await _dbContext.PathDb.Where(x => x.FolderPath.StartsWith(folderPath)).ToListAsync();

                    if (foldersForRemoval.Count > 0)
                    {
                        foreach (var folderForRemoval in foldersForRemoval)
                        {
                            var filesForRemoval = await _dbContext.FileDb.Where(x => x.PathId == folderForRemoval.Id).ToListAsync();
                            foreach (var file in filesForRemoval)
                            {
                                if (file != null)
                                {
                                    _dbContext.FileDb.Remove(file);
                                }
                            }

                            _dbContext.PathDb.Remove(folderForRemoval);
                            result += folderPath;
                        }
                    }
                }
                //remove file in folder
                else if (folderPathExists && !string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var folderForRemoval = _dbContext.PathDb.FirstOrDefault(x => x.FolderPath == folderPath);

                    if (folderForRemoval != null)
                    {
                        var fileForRemoval = _dbContext.FileDb.FirstOrDefault(x => x.FileName == fileName);

                        if (fileForRemoval != null)
                        {
                            _dbContext.FileDb.Remove(fileForRemoval);
                            result += folderPath + fileName;
                        }
                    }
                }
                //remove file only
                else if (!folderPathExists && string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(fileName))
                {
                    var fileForRemoval = _dbContext.FileDb.FirstOrDefault(x => x.FileName == fileName);

                    if (fileForRemoval != null)
                    {
                        _dbContext.FileDb.Remove(fileForRemoval);
                        result += fileName;
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            return result;
        }
    }
}
