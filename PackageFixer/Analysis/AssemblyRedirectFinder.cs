using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageFixer
{
    public static class AssemblyRedirectFinder
    {
        public static void FindAssemblyRedirects(this Solution solution, string projectName)
        {
            var cache = new AssemblyCache();

            var assemblyVersion = new Dictionary<string, IDictionary<Version, ICollection<Reference>>>();

//            var project = solution.Projects[projectName];

            foreach (var p in solution.Projects)
            {
                var project = p.Value;

                Action<string, Version, Reference> addVersion = (a, v, ar) =>
                {
                    if (!assemblyVersion.ContainsKey(a))
                    {
                        assemblyVersion.Add(a, new Dictionary<Version, ICollection<Reference>>());
                    }
                    if (!assemblyVersion[a].ContainsKey(v))
                    {
                        assemblyVersion[a].Add(v, new List<Reference>());
                    }
                    assemblyVersion[a][v].Add(ar);
                };

                foreach (var assembly in project.References)
                {
//                    if (string.IsNullOrEmpty(assembly.Package) && string.IsNullOrEmpty(assembly.HintPath))
//                        continue;


                    if (string.IsNullOrEmpty(assembly.HintPath))
                    {
//                        Console.WriteLine($"Assembly without HintPath {assembly.Name}");
                    }
                    else
                    {
                        try
                        {
                            var asm = cache.GetAssembly(project.AbsolutePath(assembly.HintPath));
                            addVersion(assembly.Name, asm.Version, assembly);

                            foreach (var r in asm.References)
                            {
                                if (!r.IsFrameworkAssembly())
                                {
                                    var v = r.Version.ToString();
                                    addVersion(r.Name, r.Version, assembly);
                                }
                                else
                                {
//                                    Console.WriteLine($"Skipping framework assembly: {r.Name}");
                                }

                            }
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine($"({project.Name}) File not found for assembly {assembly.Name}: {assembly.HintPath}");
                        }
                    }
                }

            }

            var multipleVersions = assemblyVersion.Where(k => k.Value.Count > 1).ToDictionary(k => k.Key, k => k.Value);
            foreach (var kvp in multipleVersions )
            {
                if (kvp.Value.Count > 1)
                {
                    var versions = kvp.Value.Select(v => v.Key).ToList();

                    var conversions = string.Join("=>", versions);
//                    Console.WriteLine($"Found multiple versions for {kvp.Key}: {conversions}");

                    var maxVersion = versions.OrderByDescending(v => v).First();

                    var assemblies = cache.GetCache(kvp.Key).ToList();
                    if (assemblies.Any())
                    {
                        var assembly = assemblies.SingleOrDefault(ar => ar.Version == maxVersion);

                        if (assembly != null)
                        {
                            Console.WriteLine("  <dependentAssembly>");
                            Console.WriteLine(
                                $"    <assemblyIdentity name =\\\"{kvp.Key}\\\" publicKeyToken=\\\"{assembly.PublicKeyToken}\\\" culture=\\\"neutral\\\" />");
                            Console.WriteLine(
                                $"    <bindingRedirect oldVersion =\\\"0.0.0.0-{maxVersion}\\\" newVersion=\\\"{maxVersion}\\\" />");
                            Console.WriteLine("  </dependentAssembly >");
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find maxVersion {maxVersion} of assembly {kvp.Key}");
                        }
                    }

                }
            }

        }

    }
}