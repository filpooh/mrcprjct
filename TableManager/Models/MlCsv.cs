using System.ComponentModel.DataAnnotations.Schema;
using TableManager.Data;

namespace TableManager.Models
{
    public class MlCsv
    {
        //bisogna aggiungere un riferimento user
        //modello per lo stato dei modelli 
        public int Id { get; set; }
        public int IdCsv { get; set; } // riferimento al FileCsv originale
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
        public string Name { get; set; }
        public int Stato { get; set; } = 0;
        public int? SettingId { get; set; }
        public Setting? Setting { get; set; }
        public string HeaderJson { get; set; } = ""; // header del dataset trasformato
        public ICollection<MlCsvRow> Rows { get; set; }
        public Statistics Statistics { get; set; }
        public int type { get; set; } = 0;
        //public string Type { get; set; }
        //public string Url { get; set; }
    }
}
