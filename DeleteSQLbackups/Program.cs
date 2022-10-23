using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DeleteBackups
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(ConfigurationManager.AppSettings.Get("TextFile")))
                {
                    var txt = File.CreateText(ConfigurationManager.AppSettings.Get("TextFile"));
                    txt.Close();
                }
                var rootFolder = ConfigurationManager.AppSettings.Get("root");
                string[] files = Directory.GetFiles(rootFolder);
                using (StreamWriter textFileWithDeletedBackups = new StreamWriter(ConfigurationManager.AppSettings.Get("TextFile"), true))
                {
                    var fileCount = 0;
                    var nowTime = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                    if (!files.Any())
                    {
                        textFileWithDeletedBackups.WriteLine($"{nowTime} : no files to be deleted");
                    }
                    else
                    {
                        foreach (string file in files)
                        {
                            if (Path.GetExtension(file).ToLower().EndsWith("zip"))
                            {
                                var lastMod = File.GetLastWriteTime(file);
                                var modDate = lastMod.Date;
                                var result = (DateTime.Now.Date - modDate).Days;
                                if (result >= 3)
                                {
                                    File.Delete(file);
                                    fileCount++;
                                    textFileWithDeletedBackups.WriteLine($"{nowTime} : {file} deleted");
                                }
                            }
                        }
                        if (fileCount == 0) textFileWithDeletedBackups.WriteLine($"{nowTime} : no files to be deleted");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"The process failed with error {e.Message} \n \n {e.StackTrace}");
            }
        }

    }
}



