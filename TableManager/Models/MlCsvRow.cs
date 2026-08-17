namespace TableManager.Models
{
    public class MlCsvRow
    {

        public long Id { get; set; }
        public int MlCsvId { get; set; }
        public MlCsv MlCsv { get; set; }
        public int NumeroRiga { get; set; }
        public string DataJson { get; set; } = "";
    }
}
