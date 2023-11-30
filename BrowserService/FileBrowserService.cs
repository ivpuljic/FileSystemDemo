namespace BrowserService
{
    public class FileBrowserService : IFileBrowserService
    {

        public async Task<string> SearchFileSystem(string input)
        {
            string filePath = "/";
            string fileName = "";
            string result = "/";
            var inputArray = input.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inputArray.Length == 2)
            {
                filePath = inputArray[0];
                fileName = inputArray[1];
            }
            else if (inputArray.Length == 1)
            {
                fileName = inputArray[0];
            }
            else
            {
                return result;
            }

            return result;
        }

        public async Task<string> CreateFileOrFolder(string input)
        {
            string filePath = "/";
            string fileName = "";
            string result = "/";
            var inputArray = input.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inputArray.Length == 2)
            {
                filePath = inputArray[0];
                fileName = inputArray[1];
            }
            else if (inputArray.Length == 1)
            {
                fileName = inputArray[0];
            }
            else
            {
                return result;
            }

            return result;
        }

        public async Task<string> DeleteFileOrFolder(string input)
        {
            string filePath = "/";
            string fileName = "";
            string result = "/";
            var inputArray = input.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (inputArray.Length == 2)
            {
                filePath = inputArray[0];
                fileName = inputArray[1];
            }
            else if (inputArray.Length == 1)
            {
                fileName = inputArray[1];
            }
            else
            {
                return result;
            }

            return result;
        }


    }
}
