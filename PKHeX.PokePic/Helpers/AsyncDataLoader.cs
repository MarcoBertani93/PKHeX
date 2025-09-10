using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.PokePic.ViewModels;
using XmlPictureCreation;

namespace PKHeX.PokePic.Helpers
{
    // Classe per gestire il caricamento asincrono
    public class AsyncDataLoader
    {
        public event Action<NamedProcessor>? ProcessorLoaded;

        public List<NamedProcessor> NamedProcessors { get; private set; } = [];
        const string pokePicPath = "PokePic";

        public AsyncDataLoader()
        {
            Task.Run(LoadFilesAsync);
        }
        /// <summary>
        /// Searches in folder "PokePic" and subfolders for valid configuration files.
        /// </summary>
        /// <returns></returns>
        public async Task LoadFilesAsync()
        {
            var files = ListConfigFiles();

            var tasks = files.Select(f => LoadFileAsync(f));

            await Task.WhenAll(tasks);
        }

        public async Task LoadFileAsync(string file)
        {
            var name = Path.GetRelativePath(pokePicPath, file);

            var result = await XmlPictureLoader.LoadXmlAsync(file);

            var namedProcessor = new NamedProcessor(name, result.Processor, result.Errors); 
            NamedProcessors.Add(namedProcessor);
            ProcessorLoaded?.Invoke(namedProcessor);
        }


        public static List<string> ListConfigFiles()
        {
            var xmlFiles = new List<string>();
            
            Directory.CreateDirectory(pokePicPath);

            if (Directory.Exists(pokePicPath))
            {
                xmlFiles.AddRange(Directory.GetFiles(pokePicPath, "*.xml"));

                foreach (string subDir in Directory.GetDirectories(pokePicPath))
                {
                    xmlFiles.AddRange(Directory.GetFiles(subDir, "*.xml"));
                }
            }

            return xmlFiles;
        }
    }
}
