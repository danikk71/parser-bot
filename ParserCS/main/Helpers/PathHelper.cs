using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace main.Helpers
{
    public static class PathHelper
    {
        public static string GetDataFolderPath()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            
            while(directory != null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, "ParserCS")))
                {
                    string dataPath = Path.Combine(directory.FullName, "Data");

                    if (!Directory.Exists(dataPath))
                    {
                        Directory.CreateDirectory(dataPath);
                        Console.WriteLine($"[System] Created Data folder: {dataPath}");
                    }

                    return dataPath;
                }

                directory = directory.Parent;
            }
            throw new Exception("Root folder not found");
        }
    }
}
