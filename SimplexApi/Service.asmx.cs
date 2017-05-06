using System.Linq;
using System.Web.Services;
using SimplexApi.Classes;
using SimplexApi.Models;

namespace SimplexApi
{
    /// <summary>
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : WebService
    {
        [WebMethod]
        public SimplexData ExecuteSimplex(SimplexData simplexData)
        {
            var simplexMethod = new SimplexMethod(simplexData);
            return simplexMethod.ExecuteSimplexUntilEnd();
        }

        [WebMethod]
        public ProblemData CreateRestrictionLeftover(ProblemData problem)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.CreateRestrictionLeftover(problem);
        }

        [WebMethod]
        public ProblemData IsolateTheLeftOver(ProblemData problem)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.IsolateTheLeftOver(problem);
        }

        [WebMethod]
        public int FirstStageCheckForTheEnd(GridCell[][] simplexGrid)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FirstStageCheckForTheEnd(simplexGrid);
        }

        [WebMethod]
        public int FirstStageGetAllowedColumn(GridCell[][] simplexGrid)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FirstStageGetAllowedColumn(simplexGrid);
        }

        [WebMethod]
        public int FirstStageGetAllowedRow(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FirstStageGetAllowedRow(simplexData);
        }

        [WebMethod]
        public SimplexData FirstStageFillInferiorCells(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FirstStageFillInferiorCells(simplexData);
        }

        [WebMethod]
        public SimplexData FirstStageUpdateHeaders(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FirstStageUpdateHeaders(simplexData);
        }

        [WebMethod]
        public SimplexData FirstStageReposition(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FirstStageReposition(simplexData);
        }

        [WebMethod]
        public int SecondStageGetAllowedColumn(GridCell[][] simplexGrid)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.SecondStageGetAllowedColumn(simplexGrid);
        }

        [WebMethod]
        public bool SecondStageCheckIfValid(GridCell[][] simplexGrid)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.SecondStageCheckIfValid(simplexGrid);
        }

        [WebMethod]
        public int SecondStageGetAllowedRow(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.SecondStageGetAllowedRow(simplexData);
        }

        [WebMethod]
        public SimplexData TransformFunction(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.TransformFunction(simplexData);
        }

        [WebMethod]
        public SimplexData FillSucessData(SimplexData simplexData)
        {
            var simplexSteps = new SimplexSteps();
            return simplexSteps.FillSucessData(simplexData);
        }

        [WebMethod]
        public GridCell GetGridCell(int x, int y, SimplexData simplexData)
        {
            // Esse método só serve para que o tipo GridCell possa ser vizualizado.
            return simplexData.GridArray[x][y];
        }
    }
}
