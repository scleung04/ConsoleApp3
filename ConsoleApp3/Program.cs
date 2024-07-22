using System;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Common.Step21;
using Xbim.IO;

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
            using (var model = IfcStore.Open(ifcFilePath, StorageType.Ifc, XbimSchemaVersion.Ifc4, Xbim.IO.XbimModelType.MemoryModel, new XbimEditorCredentials()))
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

                var elements = model.Instances.OfType<IIfcElement>().ToList();
                foreach (var element in elements)
                {
                    // Change IfcBuildingElementProxy to specific classes based on conditions
                    if (element.Name != null && element.Name.Value.Contains("Wall"))
                    {
                        var wall = model.Instances.New<Xbim.Ifc4.ProductExtension.IfcWall>();
                        CopyProperties(element, wall);
                    }
                    else if (element.Name != null && element.Name.Value.Contains("Slab"))
                    {
                        var slab = model.Instances.New<Xbim.Ifc4.ProductExtension.IfcSlab>();
                        CopyProperties(element, slab);
                    }
                    // Add more conditions as needed
                }
            }
        }

        static void CopyProperties(IIfcObject source, IIfcObject target)
        {
            target.GlobalId = source.GlobalId;
            target.OwnerHistory = source.OwnerHistory;
            target.Name = source.Name;
            target.Description = source.Description;
            target.ObjectType = source.ObjectType;

            // Add more property copies as needed
        }
    }
}
