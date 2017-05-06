using System.Collections.Generic;

namespace SimplexApi.Models
{
    public class LeftOverData
    {
        public VariableData LeftOverVariable { get; set; }
        public decimal FreeMember { get; set; }
        public List<RestrictionVariableData> RestrictionVariables { get; set; }
    }
}
