using Microsoft.Data.Analysis;
using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using TableManager.Data;
using TableManager.Models;
using TableManager.Models.dto;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TableManager.Classes
{
    public class HomeUtilityObject
    {
        private readonly ApplicationDbContext _context;

        public HomeUtilityObject(ApplicationDbContext context)
        {
            _context = context;
        }
        public CsvDto GetTableData(int id, string userId)
        {
            var result = GetDataFrame(id, true, userId);
            var df = result[0] as DataFrame;
            var name = result[1].ToString();
            if (df == null)
                return new CsvDto();

            // 2. Converto il DataFrame in DTO
            var dto = ConvertDataFrameToDto(id, userId, df, name);

            // 3. Applico la logica dell'ID (come nel tuo metodo originale)
            FixIdColumn(dto, id, userId);

            return dto;
        }
        private CsvDto ConvertDataFrameToDto(int id, string userId, DataFrame df, string name)
        {
            var header = df.Columns.Select(c => c.Name).ToList();
            var rows = new List<List<string>>();

            // Ogni colonna è un DataFrameColumn
            // Ogni riga va ricostruita leggendo tutte le colonne
            int rowCount = df.Rows.Count();

            for (int r = 0; r < rowCount; r++)
            {
                var rowValues = new List<string>();

                foreach (var col in df.Columns)
                {
                    rowValues.Add(col[r]?.ToString() ?? "");
                }

                rows.Add(rowValues);
            }

            return new CsvDto
            {
                Id = id,
                Name = name,
                UserId = userId,
                HeaderJson = header,
                Values = rows
            };
        }
        private void FixIdColumn(CsvDto table, int id, string userId)
        {
            if (!table.HeaderJson.Contains("Id") && !table.HeaderJson.Contains("id"))
            {
                // Se la prima colonna è una progressione numerica → sostituisco "" con "Id"
                var firstColumn = table.Values.Select(r => r[0]).ToList();

                bool isNumericProgression =
                    firstColumn.All(x => int.TryParse(x, out _)) &&
                    firstColumn.Select(int.Parse).SequenceEqual(
                        Enumerable.Range(0, firstColumn.Count)
                    );

                if (isNumericProgression)
                {
                    int emptyIndex = table.HeaderJson.IndexOf("");
                    if (emptyIndex >= 0)
                    {
                        table.HeaderJson[emptyIndex] = "Id";

                        var editTable = _context.FileCsvs
                            .FirstOrDefault(t => t.Id == id && t.UserId == userId);

                        editTable.HeaderJson = JsonSerializer.Serialize(table.HeaderJson);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    // Inserisco "Id" come prima colonna
                    table.HeaderJson.Insert(0, "Id");

                    foreach (var r in table.Values)
                    {
                        r.Insert(0, r[0]); // NumeroRiga
                    }
                }
            }
        }
        public List<object> GetDataFrame(int id, bool original, string userId)
        {
            //non include la prop di navigazione
            object mlCsv;
            if (!original)
                mlCsv = _context.MlCsv.Include(x => x.Rows).FirstOrDefault(x => x.IdCsv == id && x.UserId == userId && x.Stato == 0);
            else
                mlCsv = _context.FileCsvs.Include(x => x.Rows).FirstOrDefault(x => x.Id == id);
            List<string> header = new List<string>();
            List<List<string>> rows = new List<List<string>>();
            string name = "";
            if (mlCsv == null)
            {
                //dataframe
                header = _context.FileCsvs
                   .Where(k => k.Id == id && k.UserId == userId)
                   .Select(t => JsonSerializer.Deserialize<List<string>>(t.HeaderJson))
                   .FirstOrDefault();

                rows = _context.CsvRows
                   .Where(c => c.FileId == id)
                   .Select(r => JsonSerializer.Deserialize<List<string>>(r.DataJson))
                   .ToList();
            }
            else
            {
                if (mlCsv is MlCsv m)
                {
                    header = JsonSerializer.Deserialize<List<string>>(m.HeaderJson);

                    rows = m.Rows
                        .Select(r => JsonSerializer.Deserialize<List<string>>(r.DataJson))
                        .ToList();
                    name = m.Name;
                }
                else if (mlCsv is FileCsv f)
                {
                    header = JsonSerializer.Deserialize<List<string>>(f.HeaderJson);

                    rows = f.Rows
                        .Select(r => JsonSerializer.Deserialize<List<string>>(r.DataJson))
                        .ToList();
                    name = f.FileName;
                }
            }
            var columns = new List<DataFrameColumn>();
            if (header == null || header.Count() == 0)
                return new List<object>();

            foreach (var columnName in header)
            {
                columns.Add(new StringDataFrameColumn(columnName));
            }

            // Popolamento colonne
            foreach (var row in rows)
            {
                for (int i = 0; i < header.Count; i++)
                {
                    ((StringDataFrameColumn)columns[i]).Append(row[i]);
                }
            }

            return new List<object> { new DataFrame(columns), name };
        }
        public DataFrame CreateDummyColumns(DataFrame df, List<int> columnIds)
        {
            foreach (int columnId in columnIds)
            {
                var original = df.Columns[columnId];

                // 1. Trova tutte le categorie uniche
                var categories = new HashSet<string>();

                foreach (var cell in original)
                {
                    var value = cell?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        categories.Add(value);
                }

                // 2. Crea una colonna dummy per ogni categoria
                var dummyColumns = new Dictionary<string, Int32DataFrameColumn>();

                foreach (var cat in categories)
                {
                    dummyColumns[cat] = new Int32DataFrameColumn($"{original.Name}_{cat}");
                }

                // 3. Popola le colonne dummy
                foreach (var cell in original)
                {
                    var value = cell?.ToString();

                    foreach (var cat in categories)
                    {
                        dummyColumns[cat].Append(value == cat ? 1 : 0);
                    }
                }

                // 4. Aggiungi le colonne al DataFrame
                foreach (var col in dummyColumns.Values)
                {
                    if (!df.Columns.Contains(col))//entra a quanto pare
                        df.Columns.Add(col);
                }
            }
            return df;
        }
        public void SaveDataFrame(int fileId, DataFrame df, string userId, int type, List<int> headersId, string fill)
        {
            /*
                type:
            0->normalizzazione
            1->dummy
            2->fill
             */
            // Serializzo l'header
            var header = df.Columns.Select(c => c.Name).ToList();
            string headerJson = JsonSerializer.Serialize(header);

            // Cerco il modello esistente
            var mlCsv = _context.MlCsv
                .Include(x => x.Rows)
                .Include(x => x.Setting)
                .FirstOrDefault(x => x.IdCsv == fileId && x.UserId == userId && (x.Stato == -1 || x.Stato == 0));

            Setting setting = null;

            if (mlCsv != null)
            {
                // Setting collegato correttamente tramite MlId
                setting = _context.Settings.FirstOrDefault(s => s.MlId == mlCsv.Id);
            }

            // Preparo le nuove righe
            var newRows = df.Rows
                .Select((row, index) => new MlCsvRow
                {
                    NumeroRiga = index,
                    DataJson = JsonSerializer.Serialize(row.Select(c => c?.ToString() ?? "").ToList())
                })
                .ToList();

            // ---------------------------------------------------------
            // CASO 1: NON ESISTE MLCSV → CREO TUTTO DA ZERO
            // ---------------------------------------------------------
            if (mlCsv == null)
            {
                // 1. Creo MlCsv
                mlCsv = new MlCsv
                {
                    UserId = userId,
                    IdCsv = fileId,
                    Name = "ML_" + fileId,
                    Stato = 0,
                    HeaderJson = headerJson
                };

                _context.MlCsv.Add(mlCsv);
                _context.SaveChanges(); // mlCsv.Id ora esiste

                // 2. Creo Setting collegato
                setting = new Setting
                {
                    MlId = mlCsv.Id,
                    Fill = ""
                };
                switch (type)
                {
                    case 0:
                        setting.DummyColumn = headersId;
                        break;
                    case 1:
                        setting.NormalizeColumn = headersId;
                        break;
                    case 2:
                        setting.Fill = fill;
                        break;
                }
                _context.Settings.Add(setting);
                _context.SaveChanges();

                // 3. Aggancio Setting a MlCsv
                mlCsv.SettingId = setting.Id;
                _context.SaveChanges();

                // 4. Inserisco tutte le righe
                foreach (var r in newRows)
                {
                    r.MlCsvId = mlCsv.Id;
                    _context.MlCsvRows.Add(r);
                }

                _context.SaveChanges();
                return;
            }

            // ---------------------------------------------------------
            // CASO 2: MLCSV ESISTE → AGGIORNO
            // ---------------------------------------------------------

            // Aggiorno header
            mlCsv.HeaderJson = headerJson;

            // Aggiorno o creo righe
            foreach (var newRow in newRows)
            {
                var existingRow = mlCsv.Rows.FirstOrDefault(r => r.NumeroRiga == newRow.NumeroRiga);

                if (existingRow != null)
                {
                    existingRow.DataJson = newRow.DataJson;
                }
                else
                {
                    newRow.MlCsvId = mlCsv.Id;
                    _context.MlCsvRows.Add(newRow);
                }
            }

            // Elimino righe in eccesso
            var extraRows = mlCsv.Rows.Where(r => r.NumeroRiga >= newRows.Count).ToList();
            _context.MlCsvRows.RemoveRange(extraRows);

            _context.SaveChanges();
        }
        public MlCsv CreateNewMlCsv(int tableId, string userId, int type)
        {
            var table = _context.FileCsvs.First(x => x.UserId == userId);
            var df = GetDataFrame(tableId, true, userId)[0] as DataFrame;

            var mlCsv = new MlCsv
            {
                UserId = userId,
                IdCsv = table.Id,
                Stato = 0,
                HeaderJson = table.HeaderJson,

            };
            mlCsv.Name = "ML_" + tableId + "_" + mlCsv.Id;
            _context.MlCsv.Add(mlCsv);
            _context.SaveChanges();
            // 2. Creo Setting collegato
            var setting = new Setting
            {
                MlId = mlCsv.Id,
                Fill = ""
            };
            _context.Settings.Add(setting);
            _context.SaveChanges();
            mlCsv.SettingId = setting.Id;
            mlCsv.Setting = setting;
            var newRows = df.Rows.Select((row, index) => new MlCsvRow
            {
                NumeroRiga = index,
                DataJson = JsonSerializer.Serialize(row.Select(c => c?.ToString() ?? "").ToList())
            }).ToList();
            foreach (var r in newRows)
            {
                r.MlCsvId = mlCsv.Id;
                _context.MlCsvRows.Add(r);
            }

            _context.SaveChanges();
            return mlCsv;
        }
        public ModelDto GetModel(int id, string userId)
        {
            var modelCsv = _context.MlCsv.Include(x => x.Rows).Include(x => x.Setting).Include(x => x.Statistics).FirstOrDefault(x => x.Id == id);
            var rows = modelCsv.Rows.ToList();
            List<List<string>> listRows = new List<List<string>>();
            foreach (var r in rows)
                listRows.Add(JsonSerializer.Deserialize<List<string>>(r.DataJson));
            var stat = ConvertToDict(modelCsv.Statistics);
            ModelDto modelDto = new ModelDto
            {
                Id = modelCsv.Id,
                Name = modelCsv.Name,
                stato = modelCsv.Stato,
                HeaderJson = JsonSerializer.Deserialize<List<string>>(modelCsv.HeaderJson),
                IdCsv = modelCsv.IdCsv,
                DataRow = listRows,
                Setting = modelCsv.Setting,
                Statistic = stat,
            };
            return modelDto;
        }
        private Dictionary<string, string> ConvertToDict(Statistics stat)
        {
            if (stat != null)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>
                {
                    ["R2"] = stat.R2.ToString(),
                    ["Mse"] = stat.Mse.ToString(),
                    ["Rmse"] = stat.Rmse.ToString(),
                    ["Coef"] = stat.Coef.ToString(),
                    ["Intercept"] = stat.Intercept.ToString(),
                    ["ModelPath"] = stat.ModelPath,
                    ["ModelType"] = stat.ModelType,
                    ["GraphPath"] = stat.GraphPath,
                    ["DurationSeconds"] = stat.DurationSeconds.ToString(),
                    ["StartTime"] = stat.StartTime.ToString(),
                    ["endTime"] = stat.EndTime.ToString(),
                };
                return dict;
            }
            return null;
        }
        public async Task<Dictionary<string, string>> DownloadImg(int id, string userId, string fileName)
        {
            var payload = new
            {
                data = new List<List<string>>(),
                type = -1,
                fileName = fileName,
                userId = userId,
                headerId = new List<int>(),
                id = id,
                operation = 2,
            };
            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


                using var client = new HttpClient();

                var response = await client.PostAsync("http://localhost:8000/GetModelImgs", content);

                if (!response.IsSuccessStatusCode)
                    return new Dictionary<string, string>();

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;



                var images = new Dictionary<string, string>();

                var imagesElement = root.GetProperty("images");

                foreach (var image in imagesElement.EnumerateArray())
                {
                    var name = image.GetProperty("name").GetString();
                    var base64 = image.GetProperty("data").GetString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(base64))
                        images[name] = base64;
                }

                return images;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<ModelDto> GetModelList(string userId)
        {
            var models = _context.MlCsv.Where(x => x.UserId == userId).ToList();
            List<ModelDto> modelDto = new List<ModelDto>();
            foreach (var m in models)
            {
                var newM = new ModelDto
                {
                    IdCsv = m.IdCsv,
                    Name = m.Name,
                    Id = m.Id
                };
                modelDto.Add(newM);
            }
            return modelDto;
        }
        public DataFrame NormalizeMinMax(DataFrame df, int columnIndex)
        {
            var original = df.Columns[columnIndex];
            string columnName = original.Name;

            // Converto la colonna in double
            var numeric = new DoubleDataFrameColumn(columnName);

            foreach (var cell in original)
            {
                if (double.TryParse(cell?.ToString(), out double d))
                    numeric.Append(d);
                else
                    numeric.Append(double.NaN);
            }

            // Calcolo min e max
            double min = Convert.ToDouble(numeric.Min());
            double max = Convert.ToDouble(numeric.Max());

            // Creo la colonna normalizzata
            var normalized = new DoubleDataFrameColumn(columnName);

            foreach (var v in numeric)
            {
                double val = v.HasValue ? v.Value : double.NaN;
                double norm = (val - min) / (max - min);
                normalized.Append(norm);
            }

            // Sostituisco la colonna originale con quella normalizzata
            df.Columns.Remove(columnName);
            df.Columns.Insert(columnIndex, normalized);

            return df;
        }
        public DataFrame FillMean(DataFrame df)
        {
            foreach (var column in df.Columns)
            {
                // Gestiamo solo colonne numeriche
                if (column.DataType == typeof(double) ||
                    column.DataType == typeof(float) ||
                    column.DataType == typeof(int) ||
                    column.DataType == typeof(long))
                {
                    double sum = 0;
                    int count = 0;

                    // Calcolo media ignorando i null
                    for (int i = 0; i < column.Length; i++)
                    {
                        var value = column[i];

                        if (value != null)
                        {
                            sum += Convert.ToDouble(value);
                            count++;
                        }
                    }

                    if (count == 0)
                        continue; // tutta la colonna è vuota → non facciamo nulla

                    double mean = sum / count;

                    // Riempiamo i null con la media
                    for (int i = 0; i < column.Length; i++)
                    {
                        if (column[i] == null)
                        {
                            column[i] = mean;
                        }
                    }
                }
            }
            return df;
        }
        public DataFrame FillMedian(DataFrame df)
        {
            foreach (var column in df.Columns)
            {
                // Consideriamo solo colonne numeriche
                if (column.DataType == typeof(double) ||
                    column.DataType == typeof(float) ||
                    column.DataType == typeof(int) ||
                    column.DataType == typeof(long))
                {
                    List<double> values = new();

                    // Raccogliamo i valori non nulli
                    for (int i = 0; i < column.Length; i++)
                    {
                        var value = column[i];
                        if (value != null)
                        {
                            values.Add(Convert.ToDouble(value));
                        }
                    }

                    if (values.Count == 0)
                        continue; // colonna completamente vuota → non facciamo nulla

                    // Ordiniamo per calcolare la mediana
                    values.Sort();

                    double median;
                    int n = values.Count;

                    if (n % 2 == 1)
                    {
                        // dispari → valore centrale
                        median = values[n / 2];
                    }
                    else
                    {
                        // pari → media dei due centrali
                        median = (values[(n / 2) - 1] + values[n / 2]) / 2.0;
                    }

                    // Riempiamo i null con la mediana
                    for (int i = 0; i < column.Length; i++)
                    {
                        if (column[i] == null)
                        {
                            column[i] = median;
                        }
                    }
                }
            }
            return df;
        }
        public DataFrame FillValue(DataFrame df, string value)
        {
            bool isNumeric = double.TryParse(value, out double numericValue);

            foreach (var column in df.Columns)
            {
                // Se value è numerico → riempi solo colonne numeriche
                if (isNumeric)
                {

                    for (int i = 0; i < column.Length; i++)
                    {
                        if (column[i] == null)
                        {
                            column[i] = numericValue;
                        }
                    }

                }
                else
                {
                    // Se value NON è numerico → riempi solo colonne NON numeriche
                    if (column.DataType == typeof(string))
                    {
                        for (int i = 0; i < column.Length; i++)
                        {
                            if (column[i] == null)
                            {
                                column[i] = value;
                            }
                        }
                    }
                }
            }
            return df;
        }
    }
}
