using System.Linq;
using SimplexApi.Models;

namespace SimplexApi.Classes
{
    public class SimplexSteps
    {
        public ProblemData CreateRestrictionLeftover(ProblemData problem)
        {
            // Esse método cria a variável de folga das restrições
            for (int i = 0; i < problem.Restrictions.Count; i++)
            {
                var restr = problem.Restrictions[i];
                restr.RestrictionLeftOver.LeftOverVariable = new VariableData
                {
                    Value = "x" + (i + 1 + problem.Variables.Count).ToString().SubscriptNumber(),
                    Description = "Folga Restrição"
                };

                if (restr.RestrictionType == RestrictionType.EqualTo)
                {
                    // Caso a restrição seja do tipo igual, então a folga deve sempre ser 0
                    restr.RestrictionLeftOver.LeftOverVariable.FunctionValue = 0;
                }
                else if (restr.RestrictionType == RestrictionType.MoreThan)
                {
                    // Caso a restrição exija que seja maior ou igual ao valor passado,
                    // então negativa o valor da folga
                    restr.RestrictionLeftOver.LeftOverVariable.FunctionValue = -1;
                }
                else if (restr.RestrictionType == RestrictionType.LessThan)
                {
                    // Caso a restrição exija que seja maior ou igual ao valor passado,
                    // então positiva o valor da folga
                    restr.RestrictionLeftOver.LeftOverVariable.FunctionValue = 1;
                }
            }

            return problem;
        }

        public ProblemData IsolateTheLeftOver(ProblemData problem)
        {
            foreach (var restriction in problem.Restrictions)
            {
                // O membro livre é o valor da restrição multiplicado pelo valor do leftover
                // O valor do leftover é definido em "CreateRestrictionLeftOver"
                var restLeftOver = restriction.RestrictionLeftOver;
                restLeftOver.FreeMember = restriction.RestrictionValue * restLeftOver.LeftOverVariable.FunctionValue;

                // Se a variável de sobra for negativa, positiva ela e troca sinal do membro livre
                if (restLeftOver.LeftOverVariable.FunctionValue.IsNegative())
                {
                    restLeftOver.LeftOverVariable.FunctionValue *= -1;
                }

                foreach (var restrictionVariables in restriction.RestrictionData)
                {
                    // Define o valor de cada variável inicial para a função do membro livre
                    // O símbolo das variáveis sempre é igual ao do membro nível
                    // Ex: -10 + 2x + 3y tem que se transformar em -10-(-2x-3y), que matemáticamente é a mesma coisa
                    // O símbolo de menos intermediário será contabilizado no fim do método
                    restLeftOver.RestrictionVariables.Add(new RestrictionVariableData
                    {
                        RestrictionVariable = restrictionVariables.RestrictionVariable,
                        RestrictionValue =
                            DifferentSymbol(restrictionVariables.RestrictionValue, restLeftOver.FreeMember)
                                ? restrictionVariables.RestrictionValue * -1
                                : restrictionVariables.RestrictionValue
                    });
                }
            }

            return problem;
        }

        private bool DifferentSymbol(decimal value1, decimal value2)
        {
            return value1.IsNegative() != value2.IsNegative();
        }

        public int FirstStageCheckForTheEnd(GridCell[][] simplexGrid)
        {
            // Procura por um membro livre negativo
            for (int i = 1; i < simplexGrid[0].Length; i++)
                if (simplexGrid[0][i].Superior.IsNegative())
                    return i;
            return 0;
        }

        public int FirstStageGetAllowedColumn(GridCell[][] simplexGrid)
        {
            // Procura por variáveis negativas na linha do membro livre negativo
            var rowWithNegativeFreeNumber = FirstStageCheckForTheEnd(simplexGrid);
            for (int n = 1; n < simplexGrid.Length; n++)
                if (simplexGrid[n][rowWithNegativeFreeNumber].Superior.IsNegative())
                    return n;


            return 0;
        }

        public int FirstStageGetAllowedRow(SimplexData simplexData)
        {
            // Procura pelo menor quociente de ML / variável da coluna permitidda
            int allowedRow = 0;
            decimal minorNumber = -1;
            for (int n = 1; n < simplexData.GridArray[0].Length; n++)
            {
                if (simplexData.GridArray[simplexData.AllowedColumn][n].Superior != 0)
                {
                    var quoc = simplexData.GridArray[0][n].Superior /
                               simplexData.GridArray[simplexData.AllowedColumn][n].Superior;
                    if (quoc > 0 && (quoc < minorNumber || minorNumber == -1))
                    {
                        minorNumber = quoc;
                        allowedRow = n;
                    }
                }
            }
            return allowedRow;
        }

        public SimplexData FirstStageFillInferiorCells(SimplexData simplexData)
        {
            var allowedMember = simplexData.GridArray[simplexData.AllowedColumn][simplexData.AllowedRow];
            allowedMember.Inferior = 1 / allowedMember.Superior;

            for (int c = 0; c < simplexData.GridArray.Length; c++)
                if (c != simplexData.AllowedColumn)
                {
                    // Está na linha permitida, portanto múltiplica o superior pelo inferior do membro permitido
                    var editingMember = simplexData.GridArray[c][simplexData.AllowedRow];
                    editingMember.Inferior = editingMember.Superior * allowedMember.Inferior;
                }

            for (int r = 0; r < simplexData.GridArray[0].Length; r++)
                if (r != simplexData.AllowedRow)
                {
                    // Está na coluna permitida, portando múltiplica pelo negativo do inferior do membro permitido
                    var editingMember = simplexData.GridArray[simplexData.AllowedColumn][r];
                    editingMember.Inferior = editingMember.Superior * (allowedMember.Inferior * -1);
                }

            for (int c = 0; c < simplexData.GridArray.Length; c++)
                for (int r = 0; r < simplexData.GridArray[c].Length; r++)
                    if (c != simplexData.AllowedColumn && r != simplexData.AllowedRow)
                    {
                        // Caso não esteja nem na linha e nem na coluna permitida, soma os valores da
                        // celula superior na linha atual e da coluna permitda com da celula superior da coluna atual com a linha permitida
                        simplexData.GridArray[c][r].Inferior = simplexData.GridArray[c][simplexData.AllowedRow].Superior *
                                                               simplexData.GridArray[simplexData.AllowedColumn][r].Inferior;
                    }

            return simplexData;
        }

        public SimplexData FirstStageUpdateHeaders(SimplexData simplexData)
        {
            // Troca a básica da linah permtida com a não básic da coluna permitida
            var oldColumnHeader = simplexData.NonBasicVariables[simplexData.AllowedColumn];
            simplexData.NonBasicVariables[simplexData.AllowedColumn] =
                simplexData.BasicVariables[simplexData.AllowedRow];
            simplexData.BasicVariables[simplexData.AllowedRow] = oldColumnHeader;

            return simplexData;
        }

        public SimplexData FirstStageReposition(SimplexData simplexData)
        {
            // Reescreve itens superiores
            for (int c = 0; c < simplexData.GridArray.Length; c++)
            {
                for (int r = 0; r < simplexData.GridArray[c].Length; r++)
                {
                    if (c == simplexData.AllowedColumn || r == simplexData.AllowedRow)
                    {
                        simplexData.GridArray[c][r].Superior = simplexData.GridArray[c][r].Inferior;
                        simplexData.GridArray[c][r].Inferior = 0;
                    }
                    else
                    {
                        simplexData.GridArray[c][r].Superior = simplexData.GridArray[c][r].Superior +
                                                               simplexData.GridArray[c][r].Inferior;
                        simplexData.GridArray[c][r].Inferior = 0;
                    }
                }
            }

            return simplexData;
        }

        public int SecondStageGetAllowedColumn(GridCell[][] simplexGrid)
        {
            // Procura por um membro da função positivo
            for (int i = 1; i < simplexGrid.Length; i++)
                if (!simplexGrid[i][0].Superior.IsNegative())
                    return i;
            return 0;
        }

        public bool SecondStageCheckIfValid(GridCell[][] simplexGrid)
        {
            // Procura por variáveis negativas na coluna do membro livre da função positiva
            var columnWithPositiveFunctionValue = SecondStageGetAllowedColumn(simplexGrid);
            for (int n = 1; n < simplexGrid[columnWithPositiveFunctionValue].Length; n++)
                if (!simplexGrid[columnWithPositiveFunctionValue][n].Superior.IsNegative())
                    return true;

            return false;
        }

        public int SecondStageGetAllowedRow(SimplexData simplexData)
        {
            // Procura pelo menor quociente de ML / variável da coluna permitidda
            int allowedRow = 0;
            decimal minorNumber = -1;
            for (int n = 1; n < simplexData.GridArray[0].Length; n++)
            {
                if (simplexData.GridArray[simplexData.AllowedColumn][n].Superior != 0)
                {
                    var quoc = simplexData.GridArray[0][n].Superior /
                               simplexData.GridArray[simplexData.AllowedColumn][n].Superior;
                    if (quoc > 0 && (quoc < minorNumber || minorNumber == -1))
                    {
                        minorNumber = quoc;
                        allowedRow = n;
                    }
                }
            }
            return allowedRow;
        }

        public SimplexData TransformFunction(SimplexData simplexData)
        {
            // Caso seja maximização troca o tipo
            // Caso seja minimização só troca o sinal das variáveis
            if (simplexData.Problem.Function.Maximiza)
            {
                simplexData.Problem.Function.Maximiza = false;
            }
            else
            {
                foreach (var problemVariable in simplexData.Problem.Variables)
                {
                    problemVariable.FunctionValue *= -1;
                }
            }

            return simplexData;
        }

        public SimplexData FillSucessData(SimplexData simplexData)
        {
            var problem = simplexData.Problem;
            var grid = simplexData.GridArray;
            var firstColumn = simplexData.BasicVariables;

            // Preenche o valor final das variáveis
            for (int i = 1; i < firstColumn.Length; i++)
            {
                var originalVariable = problem.Variables.FirstOrDefault(v => v.Value == firstColumn[i]);
                if (originalVariable != null)
                    originalVariable.FinalValue = grid[0][i].Superior;
            }

            // Preenche o valor final das restrições
            foreach (var restriction in simplexData.Problem.Restrictions)
            {
                // Calcula o valor total da restrição com base nos valores dela e valores finais das variáveis
                decimal sum = 0;
                foreach (RestrictionVariableData data in restriction.RestrictionData)
                    sum += data.RestrictionVariable.FinalValue * data.RestrictionValue;

                restriction.RestrictionFinalSum = sum;

                // Calcula o valor da sobra da restrição
                restriction.RestrictionLeftOver.LeftOverVariable.FinalValue = restriction.RestrictionValue -
                                                                              restriction.RestrictionFinalSum;
            }

            // Calcula o Z
            foreach (var problemVariable in simplexData.Problem.Variables)
            {
                simplexData.Problem.Function.FinalValue += (problemVariable.FunctionValue.IsNegative() ?
                    problemVariable.FunctionValue * -1 : problemVariable.FunctionValue)
                    * problemVariable.FinalValue;
            }

            return simplexData;
        }
    }
}