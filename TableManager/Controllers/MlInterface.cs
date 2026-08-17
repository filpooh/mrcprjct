using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;
using TableManager.Classes;
using TableManager.Data;
using TableManager.Models;
//using System.Text.Json;

namespace TableManager.Controllers
{
    [Route("[controller]/[action]")]
    public class MlInterface : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _http;
        private readonly HomeUtilityObject _utilityObject;
        private object _payload { get; set; }
        //crea la classe paylaod per avere un payload globale
        public MlInterface(ApplicationDbContext context)
        {
            _payload = new object();
            _context = context;
            _http = new HttpClient();
            _utilityObject = new HomeUtilityObject(_context);

        }

        [HttpPost]
        public async Task<IActionResult> RequestByCreate(int tableId, int type, string headerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mlCsv = _context.MlCsv.Where(x => x.IdCsv == tableId && userId == x.UserId && (x.Stato == 0 || x.Stato == -1 || x.Stato == 1)).FirstOrDefault();
            if (mlCsv == null)
                mlCsv = _utilityObject.CreateNewMlCsv(tableId, userId, type);
            // mlCsv = _context.MlCsv.Where(x => x.IdCsv == tableId && userId == x.UserId).FirstOrDefault();
            var result = _utilityObject.GetDataFrame(tableId, false, userId);
            var headerJson = JsonConvert.DeserializeObject<List<int>>(headerId);
            DataFrame df = result[0] as DataFrame;
            if (type == 1 && headerJson.Count != 2)
                return BadRequest("devi inserire almeno due colonne");
            var name = result[1] as string;
            var res = await Sendrequest(df, type, mlCsv.Name, userId, headerJson, mlCsv.Id);
            mlCsv.Stato = await GetRequestStatus(mlCsv, userId);
            mlCsv.type = type;
            _context.SaveChanges();
            if (mlCsv.Stato == -1)
                return BadRequest("Errore nell'invio della richiesta");
            return Ok("ricarico la pagina");
        }
        private async Task<int> GetRequestStatus(MlCsv mlCsv, string userId)
        {
            _payload = new
            {
                data = new List<List<string>>(),
                type = -1,
                fileName = mlCsv.Name,
                userId = userId,
                headerId = new List<int>(),
                id = mlCsv.Id,
                operation = 1,
            };
            var jsonPayload = JsonConvert.SerializeObject(_payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {

                var result = await _http.PostAsync("http://127.0.0.1:8000/CheckModelStatus", content);
                var responseContent = await result.Content.ReadAsStringAsync();
                var resposeDeserialized = JObject.Parse(responseContent);

                if (resposeDeserialized["data"] != null)
                    if ((string)resposeDeserialized["data"]["status"] == "not_ready")
                        return -1;
                if (resposeDeserialized["status"] != null && (string)resposeDeserialized["status"] == "not_ready")
                    return 1;
                if (mlCsv.Statistics == null)
                    mlCsv.Statistics = new Statistics();
                //devo cambiare il cast delle proprietà da double a float per adattarsi al modello Statistics
                Statistics stat = new Statistics
                {
                    ModelPath = (string)resposeDeserialized["model_path"],
                    GraphPath = (string)resposeDeserialized["graph_path"],
                    R2 = (float)resposeDeserialized["stats"]["r2"],
                    Mse = (float)resposeDeserialized["stats"]["mse"],
                    Rmse = (float)resposeDeserialized["stats"]["rmse"],
                    Coef = (float)resposeDeserialized["stats"]["coef"],
                    Intercept = (float)resposeDeserialized["stats"]["intercept"],
                    StartTime = (DateTime)resposeDeserialized["stats"]["start_time"],
                    EndTime = (DateTime)resposeDeserialized["stats"]["end_time"],
                    DurationSeconds = (float)resposeDeserialized["stats"]["duration_seconds"],
                    ModelType = "regression"
                };

                mlCsv.Statistics = stat;
                _context.SaveChanges();
                switch ((int)resposeDeserialized["status"])
                {
                    //da definire un flag sulla generazione del modello lato api gestire:
                    /*  
                     *  in attesa di dati
                     *  bloccato
                     *  errore durante la generazione
                     */
                    case 200:
                        if ((DateTime)resposeDeserialized["stats"]["end_time"] == null)
                            return 1;//in corso
                        return 2;//finito
                    default:
                        return -1;//errore generico
                }
            }
            catch (Exception e)
            {
                return -1;
            }

            /*
             * risposta json
            "{\"msg\":\"Data received\",
            \"status\":200,
            \"data\":
                {\"status\":200,\"msg\":\"Modello lineare generato\",
                \"model_path\":\"models/f9822b20-f1f9-4a3a-a477-be5760c3f651/ML_12_7\\\\model.npy\",
                \"graph_path\":\"models/f9822b20-f1f9-4a3a-a477-be5760c3f651/ML_12_7\\\\grafico_regressione.png\",
                \"stats\":{
                    \"r2\":0.8033729209046305,
                    \"mse\":43.98026095684181,
                    \"rmse\":6.63176152744064,
                    \"coef\":2.270491803278689,
                    \"intercept\":-323.1416861826699,
                    \"start_time\":\"2026-08-15T16:53:19.662154+00:00\",
                    \"end_time\":\"2026-08-15T16:53:19.672811+00:00\",
                    \"duration_seconds\":0.010657,\"used_columns\":[\"Height\",\"Weight\"]}}}"  
            */

        }
        public async Task<IActionResult> RequestByModel(int modelId, int type, string headerJson)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Ok("ricarico la pagina");
        }
        [HttpPost]
        public async Task<IActionResult> Index(
            [FromForm] int tableId,
            [FromForm] string Name,
            [FromForm] int type,
            [FromForm] string headerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mlCsv = _context.MlCsv.Where(x => x.IdCsv == tableId && userId == x.UserId).FirstOrDefault();
            if (mlCsv == null)//or userId == null
                //return View("~/Views/Home/Table.cshtml", tableId);
                return BadRequest();
            //get dataframe
            //mlCsv.Stato = await Sendrequest(df,type,mlCsv.Name, userId);
            if (mlCsv.Stato == -1)
                return BadRequest();
            //DA RIATTIVARE PER SALVARE A DB
            //_context.MlCsv.Add(mlCsv);
            //_context.SaveChanges();

            //devo mandare il csv all'api 
            return Ok();
            //return View("~/Views/Home/Index.cshtml");
            //return View("~/Views/Table/ShowCsv.cshtml", dto);
            //return View();
        }

        private async Task<int> Sendrequest(DataFrame data, int type, string fileName, string userId, List<int> headerId, int id)
        {
            _payload = new
            {
                data = ConvertDataFrame(data),
                type = type,
                fileName = fileName,
                userId = userId,
                headerId = headerId,
                id = id,
                operation = 0,
            };
            //prendo i dati da mandare
            var jsonPayload = JsonConvert.SerializeObject(_payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {

                var response = await _http.PostAsync("http://127.0.0.1:8000/start-task", content);
                var json = await response.Content.ReadAsStringAsync();
                ViewBag.PythonResponse = json;
                if ((int)response.StatusCode != 200)
                    return -1;
            }
            catch (Exception e)
            {
                return -1;
            }
            return 0;
        }
        public static List<List<string>> ConvertDataFrame(DataFrame df)
        {
            var rows = new List<List<string>>();
            rows.Add(df.Columns.Select(c => c.Name).ToList());
            foreach (var row in df.Rows)
            {
                var list = new List<string>();
                foreach (var value in row)
                    list.Add(value?.ToString() ?? "");
                rows.Add(list);
            }

            return rows;
        }
        public IActionResult SendAgain()
        {
            return Ok();
        }
        public IActionResult CheckStatus()
        {
            return Ok();
        }
        public IActionResult GetAvaliable()
        {
            return Ok();
        }
        public IActionResult DownloadModel()
        {
            return Ok();
        }
    }
}
