using Mde.Project.Mobile.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mde.Project.Mobile.Domain.Services.Local
{
    public  class FileService:IFileService
    {
        public void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0) return;
         
            using (FileStream fileStream = File.Create(fileFullPath, (int)stream.Length))
            {
                
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);
              
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
        }
    }
}
