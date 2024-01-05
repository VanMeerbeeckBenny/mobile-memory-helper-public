using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mde.Project.Mobile.Domain.Services.Interfaces
{
    public interface IFileService
    {
        void SaveStreamToFile(string fileFullPath, Stream stream);
    }
}
