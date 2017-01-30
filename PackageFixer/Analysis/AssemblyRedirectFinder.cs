using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageFixer
{
    public static class AssemblyRedirectFinder
    {
        public static void FindAssemblyRedirects(this Solution solution)
        {
            var cache = new AssemblyCache();

            var assemblyVersion = new Dictionary<string, ICollection<Version>>();

            foreach (var p in solution.Projects.Values)
            {

                Action<string, Version> addVersion = (a, v) =>
                {
                    if (!assemblyVersion.ContainsKey(a))
                    {
                        assemblyVersion.Add(a, new List<Version>());
                    }
                    assemblyVersion[a].Add(v);
                };

                foreach (var assembly in p.References)
                {
//                    if (string.IsNullOrEmpty(assembly.Package) && string.IsNullOrEmpty(assembly.HintPath))
//                        continue;


                    if (string.IsNullOrEmpty(assembly.HintPath))
                    {
                        //Console.WriteLine($"{p.Name} assembly without HintPath {assembly.Name}");
                    }
                    else
                    {
                        try
                        {
                            var asm = cache.GetAssembly(p.AbsolutePath(assembly.HintPath));
                            addVersion(assembly.Name, asm.Version);

                            foreach (var r in asm.References)
                            {
                                var v = r.Version.ToString();
                                addVersion(r.Name, r.Version);
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine($"({p.Name}) File not found for assembly {assembly.Name}: {assembly.HintPath}");
                        }
                    }
                }

            }
            foreach (var kvp in assemblyVersion.Where(a => !a.Key.StartsWith("System") && !a.Key.StartsWith("mscorlib")))
            {
                var versions = kvp.Value.Distinct().ToList().OrderBy(a => a);


                if (versions.Count() > 1)
                {
                    var conversions = string.Join("=>", versions);
//                    Console.WriteLine($"Found multiple versions for {kvp.Key}: {conversions}");

                    var maxVersion = versions.OrderByDescending(v => v).First();

                    Console.WriteLine("<dependentAssembly>");
                    Console.WriteLine($"\t<assemblyIdentity name =\"{kvp.Key}\" publicKeyToken=\"????\" culture=\"neutral\" />");
                    Console.WriteLine($"\t<bindingRedirect oldVersion =\"0.0.0.0-{maxVersion}\" newVersion=\"{maxVersion}\" />");
                    Console.WriteLine("</dependentAssembly >");

                }
            }

        }

    }
}