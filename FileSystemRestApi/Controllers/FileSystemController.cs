using BrowserService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileSystemRestApi.Controllers
{
    [ApiController]
    [Route("file-system-browser")]
    public class FileSystemController : ControllerBase
    {
        private readonly ILogger<FileSystemController> _logger;
        private readonly IFileBrowserService _fileBrowser;

        public FileSystemController(ILogger<FileSystemController> logger, IFileBrowserService fileBrowser)
        {
            _logger = logger;
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
        [Route("deletef-file-or-folder")]
        public async Task<string> DeleteFileOrFolder([FromQuery] string input)
        {
            return await _fileBrowser.DeleteFileOrFolder(input);
        }
    }
}
