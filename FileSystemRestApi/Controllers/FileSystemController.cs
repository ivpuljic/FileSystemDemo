using BrowserService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemRestApi.Controllers
{
    [ApiController]
    [Route("file-system-browser")]
    public class FileSystemController : ControllerBase
    {
        private readonly IFileBrowserService _fileBrowser;

        public FileSystemController(IFileBrowserService fileBrowser)
        {
            _fileBrowser = fileBrowser;
        }

        [HttpGet]
        [Route("search-file-system")]
        public async Task<string> SearchFileSystem([FromQuery] string? searchInput)
        {
            return await _fileBrowser.SearchFileSystem(searchInput);
        }

        [HttpPost]
        [Route("create-file-or-folder")]
        public async Task<string> CreateFileOrFolder([FromQuery] string input)
        {
            return await _fileBrowser.CreateFileOrFolder(input);
        }

        [HttpDelete]
        [Route("delete-file-or-folder")]
        public async Task<string> DeleteFileOrFolder([FromQuery] string input)
        {
            return await _fileBrowser.DeleteFileOrFolder(input);
        }
    }
}
