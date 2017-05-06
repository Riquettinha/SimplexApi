using System.Collections.Generic;
using System.ComponentModel;

namespace SimplexApi.Models
{
    public enum RestrictionType
    {
        [Description("Menor ou igual a")]
        LessThan = 0,
        [Description("Maior ou igual a")]
        MoreThan = 1,
        [Description("Igual a")]
        EqualTo = 2,
        Default = -1
    }

    public class RestrictionFunctionData
    {
        public RestrictionType RestrictionType { get; set; }
        public decimal RestrictionValue { get; set; }
        public decimal RestrictionFinalSum { get; set; }
        public List<RestrictionVariableData> RestrictionData { get; set; }
        public LeftOverData RestrictionLeftOver { get; set; }
    }
}
