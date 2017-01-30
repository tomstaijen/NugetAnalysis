using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageFixer.Analysis
{
    public class FindMissingReferences
    {
        public void Check(Solution solution)
        {
            foreach (var p in solution.Projects)
            {
                foreach (var r in p.Value.References)
                {
                    {
                        if (string.IsNullOrEmpty(r.HintPath))
                        {

                        }
                    }
                }
            }
        }
    }
}