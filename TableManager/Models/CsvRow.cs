namespace TableManager.Models
{
    public class CsvRow
    {
        public long Id { get; set; }

        // FK verso FileCsv e model csv
        public int FileId { get; set; }
        //public int ModelId { get; set; } = 0;//non associato a nessun modello di ML quindi righe originali

        public int NumeroRiga { get; set; }

        public string DataJson { get; set; } = "";

        // Navigation property
        public FileCsv File { get; set; } = null!;
    }
}
