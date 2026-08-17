namespace TableManager.Models
{
    public class CsvDto
    {
        //modello per ritornare il csv al front end
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> HeaderJson { get; set; }
        public List<List<string>> Values { get; set; }
        public string UserId { get; set; }
        public Setting Setting { get; set; }
    }
}
