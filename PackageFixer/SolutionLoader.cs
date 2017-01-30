using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackageFixer
{
    public class SolutionLoader
    {
        public static Solution Load(string solutionFile)
        {
            var solutionDir = new FileInfo(solutionFile).DirectoryName;

            var files = Dir.Glob($"{solutionDir}\\**\\packages.config");

            var projects = new List<Project>();

            var projectNames = CsProjManager.Projects(solutionFile).ToList();

            foreach (var file in files)
            {
                var projectDir = new FileInfo(file).DirectoryName;

                var projectName = new DirectoryInfo(projectDir).Name;

                var project = Dir.Glob($"{projectDir}\\*.csproj").Single();

                var dependencies = PackagesConfigReader.GetDependencies(file);
                var references = CsProjManager.GetReferences(project);
                projects.Add(new Project(project)
                {
                    Name = projectName,
                    Packages = dependencies.ToList(),
                    References = references.ToList()
                });
            }

            var relevantProjects = projects.Where(n =>
            {
                if (projectNames.Contains(n.Name))
                    return true;
                return false;
            }).ToList();

            return new Solution()
            {
                Projectnames = projectNames.ToArray(),
                Projects = relevantProjects.ToDictionary(p => p.Name, p => p)
            };
        }
    }
}