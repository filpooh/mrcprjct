using System.ComponentModel.DataAnnotations.Schema;

namespace TableManager.Models
{
    public class Statistics
    {
        public int Id { get; set; }
        public int MlCsvId { get; set; }
        [ForeignKey(nameof(MlCsvId))]
        public MlCsv MlCsv { get; set; }
        public int RegressionId { get; set; } = 0; //da cancellare
        public string ModelPath { get; set; }
        public string ModelType { get; set; }// "regression" or "classification"
        public string GraphPath { get; set; }
        public float R2 { get; set; } = 0; //misura la validatà del modello
        public float Mse { get; set; } = 0; //errore quadratico medio (deve essere basso per un modello buono)
        public float Rmse { get; set; } = 0; //radice di mse indica di quanto può sbagliare
        public float Coef { get; set; } = 0; //correlazione tra le variabili (per regressione lineare)
        public float Intercept { get; set; } = 0; //valore di y per x=0 (per regressione lineare)
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        public float DurationSeconds { get; set; } = 0;

        public List<string> OtherValues { get; set; } = new List<string>(); //valori dei setting non ancora definiti
    }
}
