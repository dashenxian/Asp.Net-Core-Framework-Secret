using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _5._1._1_FileProvider
{
   public interface IFileManager
   {
       void ShowStructure(Action<int, string> render);
       Task<string> ReadAllTextAsync(string path);
   }
}
