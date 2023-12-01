namespace BrowserService.Interfaces
{
    public interface IFileBrowserService
    {
        public Task<string> SearchFileSystem(string? input);
        public Task<string> CreateFileOrFolder(string input);
        public Task<string> DeleteFileOrFolder(string input);

    }
}