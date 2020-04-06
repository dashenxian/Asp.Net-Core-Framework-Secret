using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _5._1._1_FileProvider
{
    public class FileManager : IFileManager
    {
        private readonly IFileProvider _fileProvider;

        public FileManager(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<string> ReadAllTextAsync(string path)
        {
            byte[] buffer;
            await using (var stream = _fileProvider.GetFileInfo(path).CreateReadStream())
            {
                buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
            }

            return Encoding.Default.GetString(buffer);
        }

        public void ShowStructure(Action<int, string> render)
        {
            Render(-1, "");

            void Render(int index, string subPath)
            {
                index++;
                foreach (var fileInfo in _fileProvider.GetDirectoryContents(subPath))
                {
                    render(index, fileInfo.Name);
                    if (fileInfo.IsDirectory)
                    {
                        Render(index, $@"{subPath}\{fileInfo.Name}".TrimEnd('\\'));
                    }
                }
            }
        }
    }
}
