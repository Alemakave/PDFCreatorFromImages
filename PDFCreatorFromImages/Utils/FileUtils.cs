using System.IO;

namespace PDFCreatorFromImages.Utils
{
    public class FileUtils
    {
        public static string GetFileName(string fileAbsolutePath)
        {
            string fileName = fileAbsolutePath;
            
            int indexOfLastPathSep = fileAbsolutePath.LastIndexOf(Path.DirectorySeparatorChar);
            if (indexOfLastPathSep > 0)
                fileName = fileAbsolutePath.Substring(indexOfLastPathSep + 1);
            int indexOfLastExtSep = fileAbsolutePath.LastIndexOf('.');
            if (indexOfLastExtSep > 0)
                fileName = fileAbsolutePath.Substring(0, indexOfLastExtSep);

            return fileName;
        }
    }
}