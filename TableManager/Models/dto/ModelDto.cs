namespace TableManager.Models.dto
{
    public class ModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int stato { get; set; }
        public List<string> HeaderJson { get; set; }
        public List<List<string>> DataRow { get; set; }
        public int IdCsv { get; set; }
        public Setting? Setting { get; set; }
        public Dictionary<string, string> Statistic { get; set; }
        public Dictionary<string, string> ImgDict { get; set; }
    }
}
