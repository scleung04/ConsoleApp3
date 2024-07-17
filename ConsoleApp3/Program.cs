using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace ModelInspector
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Input root folder containing IFC models
            string rootFolderPath = @"C:\Users\scleu\Downloads\2024 Summer Research\R2024";

            // Process each IFC file in the folder structure
            ProcessIFCFilesInFolder(rootFolderPath);
        }

        static void ProcessIFCFilesInFolder(string folderPath)
        {
            // Get all IFC files in the folder and its subfolders
            string[] ifcFiles = Directory.GetFiles(folderPath, "*.ifc", SearchOption.AllDirectories);

            foreach (var filePath in ifcFiles)
            {
                ProcessIFCFile(filePath);
            }
        }

        static void ProcessIFCFile(string ifcFilePath)
        {
            using (var model = IfcStore.Open(ifcFilePath))
            {
                var proxies = model.Instances.OfType<IIfcBuildingElementProxy>();
                if (proxies.Any())
                {
                    // CSV file path
                    string csvFilePath = Path.ChangeExtension(ifcFilePath, ".csv");

                    // Write proxies to CSV file
                    using (var writer = new StreamWriter(csvFilePath))
                    {
                        writer.WriteLine("Count,EntityLabel,ExpressType,Name");

                        int count = 0;
                        foreach (var proxyElement in proxies)
                        {
                            count++;
                            string line = $"{count},{proxyElement.EntityLabel},{proxyElement.ExpressType},{proxyElement.Name}";
                            writer.WriteLine(line);
                        }
                    }

                    Console.WriteLine($"Processed {ifcFilePath} and generated report at {csvFilePath}");
                }
                else
                {
                    Console.WriteLine($"No IfcBuildingElementProxy instances found in {ifcFilePath}");
                }
            }
        }
    }
}
