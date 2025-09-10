using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPictureCreation;

namespace PKHeX.PokePic.ViewModels
{
    public record NamedProcessor(string Name, XmlPictureProcessor? Processor, IEnumerable<Exception> Errors)
    {
    }
}
