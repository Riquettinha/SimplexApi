using System.ComponentModel;

namespace SimplexApi.Models
{
    public enum SimplexStatus
    {
        [Description("Pendente")]
        Pending = 0,
        [Description("Ponto ótimo encontrado")]
        Sucess = 1,
        [Description("Solução impossível")]
        Impossible = 3,
        [Description("Solução infinita")]
        Infinite = 4
    }

    public class SimplexData
    {
        public ProblemData Problem { get; set; }
        public GridCell[][] GridArray { get; set; }
        public int AllowedColumn { get; set; }
        public int AllowedRow { get; set; }
        public string[] NonBasicVariables { get; set; }
        public string[] BasicVariables { get; set; }
        public SimplexStatus Status { get; set; }
    }
}
