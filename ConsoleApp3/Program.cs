using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace ModelInspector
{
    //TODO: add feature to write down the report as a CSV file
    //TODO: use this in a program that can inspect a series of Ifc models within a folder structure. Input should be the root folder, and output should be a series of CSV files named after the original ifc models.
    public class Program
    {
        static void Main(string[] args)
        {
            string ifcModelPath = @"C:\Users\scleu\Downloads\2024 Summer Research\R2024\02-NHSJC-ARQ.ifc";

            using (var model = IfcStore.Open(ifcModelPath))
            {
                var proxies = model.Instances.OfType<IIfcBuildingElementProxy>();
                if (proxies.Any())
                {
                    int count = 0;
                    foreach (var proxyElement in proxies)
                    {
                        count++;
                        Console.WriteLine($"{count} {proxyElement.EntityLabel}\t{proxyElement.ExpressType}\t{proxyElement.Name}");
                    }
                }
            }
        }
    }
}