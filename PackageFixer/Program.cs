using System;
using System.CodeDom;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageFixer
{
    class Program
    {

        static void Main(string[] args)
        {
            new Program().Run();
            Console.ReadKey();
        }

        public void Run()
        {
            var solution = SolutionLoader.Load("d:\\Projects\\NControlLegacy\\nc\\Source\\NControl\\NControl.sln");

//            solution.AnalyzePackages("NeedsRemoval");

            solution.FindAssemblyRedirects();
//            FindAnomalies(solution);

        }

        public void PrintAllPackages(Solution solution)
        {
            foreach (var p in solution.AllPackages.OrderBy(o => o))
            {
                Console.WriteLine(p);
            }
        }

        public void PrintHttpFormatting(Solution solution)
        {
            foreach (var p in solution.Projects.Values)
            {
                var reference = p.ReferenceByName("System.Net.Http.Formatting");
                if (reference != null)
                {
                    var package = p.PackageByReference(reference);
                    Console.WriteLine($"{p.Name} - {reference.Name} - {reference.Package} - {reference.PackageVersion} - {package?.Version} - {package?.TargetFramework}");
                }
            }
        }
    }
}
