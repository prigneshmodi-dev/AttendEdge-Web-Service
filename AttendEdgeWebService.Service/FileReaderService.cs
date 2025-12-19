using AttendEdgeWebService.Infrastructure.IService;
using AttendEdgeWebService.Infrastructure.Utils;
using System;
using System.IO;
using System.Web.Hosting;

namespace AttendEdgeWebService.Service
{
    public class FileReaderService : IFileReaderService
    {
        public string ReadFileContent(string filePath)
        {
            if (filePath.IsNullOrEmpty())
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            string physicalPath = HostingEnvironment.MapPath(filePath);

            if (physicalPath == null || !File.Exists(physicalPath))
                throw new FileNotFoundException($"File not found at path: {filePath}");

            return File.ReadAllText(physicalPath);
        }
    }
}
