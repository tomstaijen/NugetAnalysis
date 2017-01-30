using System;
using System.Linq;

namespace PackageFixer
{
    public static class AnalysePackages
    {
        public static void AnalyzePackages(this Solution solution, string referenceProject)
        {
            var mainProject = solution.Projects[referenceProject];

            foreach (var package in mainProject.Packages)
            {
                var references = mainProject.ReferenceByPackage(package);

                var once = false;
                Action printName = () =>
                {
                    if (once == false)
                    {
                        once = true;
                        Console.WriteLine($"{package.Id}");
                    }
                };

                foreach (var otherProject in solution.Projects.Values)
                {
                    var otherPackage = otherProject.PackageById(package.Id);

                    // detect different versions of package
                    if (otherPackage != null && (otherPackage.Version != package.Version ) )
                    {
                        printName();
                        Console.WriteLine(
                            $"\t({otherProject.Name}) TODO {otherPackage.Version} => {package.Version} | {otherPackage.TargetFramework} => {package.TargetFramework}");
                    }

                    // check the references, they must match the package
                    if (otherPackage != null)
                    {
                        foreach (var reference in references)
                        {
                            var otherReference = otherProject.ReferenceByName(reference.Name);
                            // has package but not the package
                            if (otherReference == null )
                            {
                                printName();
                                Console.WriteLine($"\t({otherProject.Name}) TODO is missing refernece from package {reference.Name}");
                            }

                            if (otherReference != null && (otherReference.Package != reference.Package || otherReference.PackageVersion != reference.PackageVersion))
                            {
                                printName();
                                Console.WriteLine($"\t({otherProject.Name}) TODO has reference from outside the package: path={reference.HintPath}");
                            }
                        }
                    }

                    // detect the same lib without a package
                    if (otherPackage == null )
                    {
                        var otherReferences =
                            references.Select(r => otherProject.ReferenceByName(r.Name)).Where(r => r != null).ToList();

                        if (otherReferences.Any())
                        {
                            printName();
                            var refString = string.Join(", ", otherReferences.Select(o => o.Name));
                            Console.WriteLine($"\t({otherProject.Name}) TODO missing nuget, but has references: {refString}");
                        }
                    }
                }
            }
        }
    }
}