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

                var projectFile = Dir.Glob($"{projectDir}\\*.csproj").Single();

                var project = new Project(projectFile)
                {
                    Name = projectName
                };

                project.References = CsProjManager.GetReferences(project, projectFile).ToList();
                project.Packages = PackagesConfigReader.GetDependencies(file).ToList();
                
                projects.Add(project);
            }

            var relevantProjects = projects.Where(n =>
            {
                if (projectNames.Contains(n.Name))
                    return true;
                return false;
            }).ToList();

            return new Solution(solutionFile)
            {
                Projectnames = projectNames.ToArray(),
                Projects = relevantProjects.ToDictionary(p => p.Name, p => p)
            };
        }
    }
}