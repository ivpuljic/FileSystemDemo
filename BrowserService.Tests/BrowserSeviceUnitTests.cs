using BrowserService.Services;
using Infrastructure.Database.FileSystem;
using Microsoft.EntityFrameworkCore;

namespace BrowserService.Tests
{
    public class BrowserSeviceUnitTests
    {
        [Fact]
        public async Task Test1()
        {
            var options = new DbContextOptionsBuilder<FileSystemDbContext>()
                .UseInMemoryDatabase(databaseName: "FileSystem")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new FileSystemDbContext(options))
            {
                var service = new FileBrowserService(context);

                //Create
                var result = await service.CreateFileOrFolder(null);
                Assert.Equal(result, "/");

                result = await service.CreateFileOrFolder("file");
                Assert.Equal(result, "/file");

                result = await service.CreateFileOrFolder("folder/file");
                Assert.Equal(result, "/folder/file");

                result = await service.CreateFileOrFolder("folder/file2");
                Assert.Equal(result, "/folder/file2");

                result = await service.CreateFileOrFolder("folder1/folder2/file");
                Assert.Equal(result, "/folder1/folder2/file");

                //Search
                result = await service.SearchFileSystem("/folder1/folder2/file");
                Assert.Equal(result, "/folder1/folder2/file\n");

                result = await service.SearchFileSystem("file5");
                Assert.Equal(result, "/");

                result = await service.SearchFileSystem("file");
                Assert.Equal(result, "/file\n/folder/file\n/folder/file2\n/folder1/folder2/file\n");

                //Delete
                result = await service.DeleteFileOrFolder("file");
                Assert.Equal(result, "/file");

                result = await service.DeleteFileOrFolder("folder/");
                Assert.Equal(result, "/folder/");

                result = await service.SearchFileSystem("file");
                Assert.Equal(result, "/folder1/folder2/file\n");

            }
        }
    }
}