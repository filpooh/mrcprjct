using System.ComponentModel.DataAnnotations;
using TableManager.Data;
namespace TableManager.Models
{
    public class FileCsv
    {
        [Key]
        public int Id { get; set; }

        // FK verso ApplicationUser (string!)
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string FileName { get; set; } = "";
        public string HeaderJson { get; set; } = "";
        public bool OriginalFile { get; set; } = true;
        // Relazione 1:N
        public ICollection<CsvRow> Rows { get; set; } = new List<CsvRow>();
        //public string FilePath {get;set;};
    }
}
