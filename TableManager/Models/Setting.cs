using System.ComponentModel.DataAnnotations.Schema;

namespace TableManager.Models
{
    public class Setting
    {
        //questo modello salva i setting 
        public int Id { get; set; }
        public int MlId { get; set; }
        [ForeignKey(nameof(MlId))]
        public MlCsv MlCsv { get; set; }
        //public List<Header> Header { get; set; }//da cancellare
        public string Fill { get; set; } = "";
        public List<int>? NormalizeColumn { get; set; }
        public List<int>? DummyColumn { get; set; }
        public int RegressionType { get; set; }
        public string StatoVerbose { get; set; } = "da iniziare";
        //public List<TableRow> ErrorRow { get; set; }
        //public List<Cell>ErrorCell { get; set; }


    }
}
