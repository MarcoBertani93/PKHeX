using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPictureCreation;

namespace PKHeX.PokePic.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name">Name of the XmlFileCreator (or path if none)</param>
    /// <param name="LoadResult">Result of the config loading operation (containr the Processor if loading was successful)</param>
    public record NamedProcessor(string Name, LoadResult LoadResult)
    {
    }
}
