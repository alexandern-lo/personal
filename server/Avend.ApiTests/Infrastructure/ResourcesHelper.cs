using System;
using System.IO;

namespace Avend.ApiTests.Infrastructure
{
    public static class ResourcesHelper
    {
        /// <summary>
        /// Auxiliary method to open the test resource as a stream by file name.
        /// </summary>
        /// 
        /// <param name="testFile">File name of the resource inside Resources folder of test project</param>
        /// 
        /// <returns>Valid reading stream</returns>
        /// 
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission to access current directory.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file. </exception>
        public static Stream GetTestSampleAsStream(string testFile)
        {
            var startupPath = Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(startupPath, "Resources", testFile);

            return File.Open(fullPath, FileMode.Open);
        }
    }
}