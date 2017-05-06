using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimplexApi.Models;

namespace SimplexApi.Classes
{
    public class SimplexMethod
    {
        private SimplexData _simplexData;
        private int _step, _stage;

        public SimplexMethod(SimplexData simplexData)
        {
            _simplexData = simplexData;
            _step = -1;
            _stage = 1;
        }

        public SimplexData ExecuteSimplexUntilEnd()
        {
            while (_simplexData.Status == SimplexStatus.Pending)
            {
                ExecuteSimplex();
            }

            return _simplexData;
        }

        private void ExecuteSimplex()
        {
            var simplexSteps = new SimplexSteps();
            if (_step == -1)
            {
                _simplexData = simplexSteps.TransformFunction(_simplexData);
                _simplexData.Problem = simplexSteps.CreateRestrictionLeftover(_simplexData.Problem);
                _step++;
            }
            else if (_step == 0)
            {
                _simplexData.Problem = simplexSteps.IsolateTheLeftOver(_simplexData.Problem);
                _step++;

                // Monta um array na ordem das variáveis básicas e não básica
                // Preenche um grid com as informações corretas
                _simplexData.NonBasicVariables = GetColumnsHeaderArray();
                _simplexData.BasicVariables = GetRowsHeaderArray();
                _simplexData.GridArray = GetProblemSimplexGrid();
            }
            else if (_stage == 1 && _step == 1)
            {
                // Verifica se existe membro livre negativpo
                if (simplexSteps.FirstStageCheckForTheEnd(_simplexData.GridArray) != 0)
                {
                    // Caso tenha vai para o próximo passo
                    _step++;
                }
                else
                {
                    // Caso não tenha, vai para o próximo estágio
                    _stage++;
                    _step =1;
                }
            }
            else if (_stage == 1 && _step == 2)
            {
                // Pega primeira coluna com valor negativo
                _simplexData.AllowedColumn = simplexSteps.FirstStageGetAllowedColumn(_simplexData.GridArray);
                if (_simplexData.AllowedColumn != 0)
                {
                    // Se coluna permitida existe vai para o próximo passo
                    _step++;
                }
                else
                {
                    // Se não tem, é um caso de região permissiva inexistente
                    _simplexData.Status = SimplexStatus.Impossible;
                }
            }
            else if (_stage == 1 && _step == 3)
            {
                // Pega linha com menor quocite do ML pela celula superior da coluna permitida
                _simplexData.AllowedRow = simplexSteps.FirstStageGetAllowedRow(_simplexData);
                _step++;
            }
            else if (_step == 4)
            {
                // Preenche célular inferiores do grid
                _simplexData = simplexSteps.FirstStageFillInferiorCells(_simplexData);
                _step++;
            }
            else if (_step == 5)
            {
                // Troca coluna da variáveis básicas com não básicas
                _simplexData = simplexSteps.FirstStageUpdateHeaders(_simplexData);
                _step++;
            }
            else if (_step == 6)
            {
                // Preenche novamente células superiores e volta à verificação do primeiro passo
                _simplexData = simplexSteps.FirstStageReposition(_simplexData);
                _step =1;
                _stage = 1;
            }
            else if (_stage == 2 && _step == 1)
            {
                // Verifica se existe variável com valor de função positiva
                _simplexData.AllowedColumn = simplexSteps.SecondStageGetAllowedColumn(_simplexData.GridArray);
                if (_simplexData.AllowedColumn != 0)
                {
                    // Caso tenha vai para o próximo passo
                    _step++;
                }
                else
                {
                    // Solução ÓTIMA encontrada
                    _simplexData.Status = SimplexStatus.Sucess;
                    _simplexData = simplexSteps.FillSucessData(_simplexData);
                }
            }
            else if (_stage == 2 && _step == 2)
            {
                // Pega coluna permitida
                var positive = simplexSteps.SecondStageCheckIfValid(_simplexData.GridArray);
                if (positive)
                {
                    // Caso tenha vai para o próximo passo
                    _step++;
                }
                else
                {
                    // Se não tem, é um caso de região permissiva impossível
                    _simplexData.Status = SimplexStatus.Infinite;
                }
            }
            else if (_stage == 2 && _step == 3)
            {
                // Pega linha permitida
                _simplexData.AllowedRow = simplexSteps.SecondStageGetAllowedRow(_simplexData);
                _step++;
            }
        }

        private string[] GetColumnsHeaderArray()
        {
            // Monta o cabeçalho das variáveis não básicas
            var columnHeaderArray = new string[_simplexData.Problem.Variables.Count+1];
            columnHeaderArray[0] = "ML";
            for (int i = 1; i <= _simplexData.Problem.Variables.Count; i++)
            {
                columnHeaderArray[i] = _simplexData.Problem.Variables[i - 1].Value;
            }

            return columnHeaderArray;
        }

        private string[] GetRowsHeaderArray()
        {
            // Monta o cabeçalho das variáveis básicas
            var rowsHeaderArray = new string[_simplexData.Problem.Restrictions.Count+1];
            rowsHeaderArray[0] = "f(x)";
            for (int i = 1; i <= _simplexData.Problem.Restrictions.Count; i++)
                rowsHeaderArray[i] = _simplexData.Problem.Restrictions[i - 1].RestrictionLeftOver.LeftOverVariable.Value;

            return rowsHeaderArray;
        }

        private GridCell[][] GetProblemSimplexGrid()
        {
            // Preenche o grid com o valor das variáveis na função
            // E o valor das variáveis na função referente à variável de folga
            var simplexGrid = GridArray(_simplexData.Problem.Variables.Count,
                _simplexData.Problem.Restrictions.Count);

            for (int i = 0; i < _simplexData.Problem.Variables.Count; i++)
            {
                simplexGrid[i + 1][0] = new GridCell
                {
                    Superior = _simplexData.Problem.Variables[i].FunctionValue,
                    Inferior = 0
                };
            }
            for (int i = 0; i < _simplexData.Problem.Restrictions.Count; i++)
            {
                var restr = _simplexData.Problem.Restrictions[i].RestrictionLeftOver;

                simplexGrid[0][i + 1] = new GridCell
                {
                    Superior = restr.FreeMember,
                    Inferior = 0
                };
                for (int j = 0; j < restr.RestrictionVariables.Count; j++)
                {
                    simplexGrid[j + 1][i + 1] = new GridCell
                    {
                        Superior = restr.RestrictionVariables[j].RestrictionValue,
                        Inferior = 0
                    };
                }
            }

            return simplexGrid;
        }

        private GridCell[][] GridArray(int x, int y)
        {
            var grid = new GridCell[x+1][];

            var gridFunctionRow = new GridCell[y+1];
            gridFunctionRow[0] = new GridCell();
            for (int j = 0; j < y; j++)
                gridFunctionRow[j+1] = new GridCell();
            grid[0] = gridFunctionRow;

            for (int i = 0; i < x; i++)
            {
                var gridRow = new GridCell[y+1];
                gridRow[0] = new GridCell();

                for (int j = 0; j < y; j++)
                    gridRow[j+1] = new GridCell();

                grid[i+1] = gridRow;
            }

            return grid;
        }
    }
}