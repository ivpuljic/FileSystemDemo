using BrowserService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace FileSystemRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSystemController : ControllerBase
    {
        private readonly ILogger<FileSystemController> _logger;
        private readonly IFileBrowserService _fileBrowser;

        public FileSystemController(ILogger<FileSystemController> logger, IFileBrowserService fileBrowser)
        {
            _logger = logger;
            _fileBrowser = fileBrowser;
        }

        [HttpGet(Name = "SearchFileSystem")]
        public async Task<string> SearchFileSystem([FromQuery]string searchInput)
        {
            return await _fileBrowser.SearchFileSystem(searchInput);
        }

        [HttpPost(Name = "CreateFileOrFolder")]
        public async Task<string> CreateFileOrFolder([FromQuery]string input)
        {
            return await _fileBrowser.CreateFileOrFolder(input);
        }

        [HttpDelete(Name = "DeleteFileOrFolder")]
        public async Task<string> DeleteFileOrFolder([FromQuery] string input)
        {
            return await _fileBrowser.DeleteFileOrFolder(input);
        }
    }
}
