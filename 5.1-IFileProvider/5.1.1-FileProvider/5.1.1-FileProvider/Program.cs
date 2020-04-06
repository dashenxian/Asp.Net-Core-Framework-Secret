using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace _5._1._1_FileProvider
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //static void Print(int layer, string name) =>
            //    Console.WriteLine($"{new string(' ', layer * 4)}{name}");
            //new ServiceCollection()
            //    .AddSingleton<IFileProvider>(new PhysicalFileProvider(@"d:\test"))
            //    .AddSingleton<IFileManager, FileManager>()
            //    .BuildServiceProvider()
            //    .GetRequiredService<IFileManager>()
            //    .ShowStructure(Print);

            //var content = await new ServiceCollection()
            //     .AddSingleton<IFileProvider>(new PhysicalFileProvider(@"d:\test"))
            //     .AddSingleton<IFileManager, FileManager>()
            //     .BuildServiceProvider()
            //     .GetRequiredService<IFileManager>()
            //     .ReadAllTextAsync("data.txt");
            //Console.WriteLine(content);
            //Debug.Assert(content == File.ReadAllText(@"d:\test\data.txt"));


            var assembly = Assembly.GetEntryAssembly();

            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.data.txt");
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var content2 = Encoding.Default.GetString(buffer);


            var content1 = await new ServiceCollection()
                .AddSingleton<IFileProvider>(new EmbeddedFileProvider(assembly))
                .AddSingleton<IFileManager, FileManager>()
                .BuildServiceProvider()
                .GetRequiredService<IFileManager>()
                .ReadAllTextAsync("data.txt");

            Debug.Assert(content1 == content2);


            //await FileChangeWatch();
        }

        static async Task FileChangeWatch()
        {
            using var fileProvider = new PhysicalFileProvider(@"d:\test");
            string original = null;
            ChangeToken.OnChange(() => fileProvider.Watch("data.txt"), Callback);
            while (true)
            {
                File.WriteAllText(@"d:\test\data.txt", DateTime.Now.ToString());
                await Task.Delay(5000);
            }

            async void Callback()
            {
                using var stream = fileProvider.GetFileInfo("data.txt").CreateReadStream();
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                string current = Encoding.Default.GetString(buffer);
                if (current != original)
                {
                    Console.WriteLine(original = current);
                }
            }
        }
    }
}
