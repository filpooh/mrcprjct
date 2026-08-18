using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Analysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using TableManager.Classes;
using TableManager.Data;
using TableManager.Models;

namespace TableManager.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HomeUtilityObject _utilityObject;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            _utilityObject = new HomeUtilityObject(_context);

        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AddTable()
        {
            return View();
        }
        public IActionResult Tablecopy()
        {
            return View();
        }
        public IActionResult ModelList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return PartialView("_ModelListPartial", _utilityObject.GetModelList(userId));
        }
        public IActionResult LoadUserTables()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tables = _context.FileCsvs
                .Where(t => t.UserId == userId)
                .ToList();

            return PartialView("_UserTablesPartial", tables);
        }
        public IActionResult LoadTableSettings(int id)
        {
            var model = GetTableHeader(id);

            return PartialView("_TableSettingsPartial", model);
        }
        public IActionResult LoadTableData(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _utilityObject.GetTableData(id, userId);
            return PartialView("_TableDataPartial", model);
        }
        public IActionResult Table(int id)
        {
            //guarda come ho fatto con i model per snellire le partial
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var table = _utilityObject.GetTableData(id, userId);
            if (table == null)
                return BadRequest("tabella non trovata");
            return View(table);
        }
        public async Task<IActionResult> Models(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = _utilityObject.GetModel(id, userId);
            model.ImgDict = await _utilityObject.DownloadImg(id, userId, model.Name);

            return View(model); // la view usa il modello completo
        }

        [HttpPost]
        public IActionResult UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File mancante");

            using var reader = new StreamReader(file.OpenReadStream());

            var firstLine = reader.ReadLine();
            var headers = firstLine.Split(',').ToList();

            return Ok(headers);
        }
        [HttpPost]
        public IActionResult SubmitTable(IFormFile file, string name, string headers)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return BadRequest();
            var selectedHeaders = JsonSerializer.Deserialize<List<string>>(headers);
            using var reader = new StreamReader(file.OpenReadStream());
            var firstLine = reader.ReadLine();
            var originalHeaders = firstLine.Split(',');
            var indicesToKeep = originalHeaders
                .Select((h, i) => new { h, i })
                .Where(x => selectedHeaders.Contains(x.h))
                .Select(x => x.i)
                .ToList();
            //filecsv
            try
            {
                var filcsv = new FileCsv
                {
                    UserId = userId,
                    FileName = name,
                    HeaderJson = JsonSerializer.Serialize(selectedHeaders),//or indicesToKeep per il numero
                };
                _context.FileCsvs.Add(filcsv);
                _context.SaveChanges();
                //rows
                int rowNumber = 1;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var parts = line.Split(',');
                    var filtered = indicesToKeep.Select(i => parts[i]).ToList();
                    var row = new CsvRow
                    {
                        FileId = filcsv.Id,
                        NumeroRiga = rowNumber++,
                        DataJson = JsonSerializer.Serialize(filtered)
                    };
                    _context.CsvRows.Add(row);
                }
                _context.SaveChanges();
                return Ok(new { message = "done", redirect = "/Home/Index" });//return json
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "fail", redirect = "", error = e });
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult FillEmptyCell(int id, int type, string value)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _utilityObject.GetDataFrame(id, false, userId);
            var df = result[0] as DataFrame;
            var name = result[1].ToString();
            if (type != 3 && (value != null || value != ""))
                //posso non considerarlo tanto non lo vado ad utilizzare
                switch (type)
                {
                    case 1://media
                        value = "media";
                        df = _utilityObject.FillMean(df);
                        break;
                    case 2://mediana
                        value = "mediana";
                        df = _utilityObject.FillMedian(df);
                        break;
                    case 3:
                        df = _utilityObject.FillValue(df, value);
                        break;
                }
            _utilityObject.SaveDataFrame(id, df, userId, 2, [], value);
            return Ok(new { message = "done" });
        }
        public IActionResult GetEmptyRow(int id)
        {
            //ritorna le righe in formato json con uno o più valori vuoti
            List<string> rowJson = new List<string>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var table = _context.FileCsvs.Where(k => k.Id == id && k.UserId == userId).FirstOrDefault();
            var rows = _context.CsvRows.Where(c => c.FileId == table.Id).ToList();
            var headerJson = JsonSerializer.Deserialize<List<string>>(table.HeaderJson);
            foreach (var row in rows)
            {
                var cells = JsonSerializer.Deserialize<List<string>>(row.DataJson);

                // Se almeno una cella è vuota, aggiungi la riga
                if (cells.Any(c => string.IsNullOrEmpty(c)))
                {
                    rowJson.Add(row.DataJson);
                }
            }
            return Ok(new { message = "done", rows = rowJson, header = headerJson });
        }
        //metodi normalizzazione non usati
        public IActionResult NormalizeColumns(int id, List<int> headerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //posso aggiungere anche il tipo di normalizzazione
            var result = _utilityObject.GetDataFrame(id, false, userId);
            var df = result[0] as DataFrame;
            var name = result[1].ToString();
            foreach (var h in headerId)
                df = _utilityObject.NormalizeMinMax(df, h);
            //non fa la normalizzazione
            _utilityObject.SaveDataFrame(id, df, userId, 1, headerId, "");
            return Ok(new { message = "done" });
        }
        public IActionResult DummyColumn(int id, List<int> headerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = _utilityObject.GetDataFrame(id, false, userId);
            var df = result[0] as DataFrame;
            var name = result[1].ToString();
            df = _utilityObject.CreateDummyColumns(df, headerId);
            _utilityObject.SaveDataFrame(id, df, userId, 0, headerId, "");
            return Ok(new { message = "done" });
        }
        public CsvDto GetTableHeader(int id)
        {
            var csvdto = new CsvDto();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var t = _context.FileCsvs.Where(k => k.Id == id && k.UserId == userId).FirstOrDefault();
            var HeaderJson = JsonSerializer.Deserialize<List<string>>(t.HeaderJson);
            if (HeaderJson == null)
                HeaderJson = new List<string>();
            else
                csvdto.HeaderJson = HeaderJson;
            return csvdto;
        }
        //deve venir fatta la richiesta tramite js se eliminare tutti i modelli generati dal file
        [HttpPost]
        public IActionResult DeleteFile(int id, bool model)//agiunta anche la parte per eliminare anche i Models e tutti i dati collegati
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var table = _context.FileCsvs.Where(k => k.Id == id && k.UserId == userId).FirstOrDefault();
            //string message = "File eliminato con successo";
            if (table == null)//mettere null solo per debug
                return BadRequest(new { message = "File non trovato" });
            if (model)
            {
                var models = _context.MlCsv.Where(m => m.IdCsv == table.Id).ToList();
                var modelrows = _context.MlCsvRows.Where(m => models.Select(x => x.Id).Contains(m.MlCsvId)).ToList();
                var settings = _context.Settings.Where(m => models.Select(x => x.Id).Contains(m.MlId)).ToList();
                _context.MlCsvRows.RemoveRange(modelrows);
                _context.MlCsv.RemoveRange(models);
                _context.Settings.RemoveRange(settings);
                _context.SaveChanges();
                //message= "File e modelli collegati eliminati con successo";

            }
            var rows = _context.CsvRows.Where(c => c.FileId == table.Id).ToList();
            _context.CsvRows.RemoveRange(rows);
            _context.FileCsvs.RemoveRange(table);
            _context.SaveChanges();

            return Ok(new { redirect = "/Home/Index" });
        }
        public IActionResult DeleteModel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _context.MlCsv.Where(m => m.Id == id && m.UserId == userId).FirstOrDefault();
            if (model == null)
                return BadRequest(new { message = "Modello non trovato" });
            var modelrows = _context.MlCsvRows.Where(m => m.MlCsvId == model.Id).ToList();
            var stat = _context.Statistics.Where(m => m.MlCsvId == model.Id);
            _context.Statistics.RemoveRange(stat);
            _context.MlCsvRows.RemoveRange(modelrows);
            _context.MlCsv.RemoveRange(model);
            _context.SaveChanges();
            return Ok(new { redirect = "/Home/Index" });
        }
    }
}