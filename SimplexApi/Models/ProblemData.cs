using System.Collections.Generic;

namespace SimplexApi.Models
{
    public class ProblemData
    {
        public FunctionData Function { get; set; }
        public List<VariableData> Variables { get; set; }
        public List<RestrictionFunctionData> Restrictions { get; set; }
    }
}
