using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TableManager.Models
{
    public class TableProp
    {
        [Key]
        public int Id { get; set; }


        // FK verso FileCsv
        public int FileId { get; set; }

        [ForeignKey(nameof(FileId))]
        public FileCsv File { get; set; }

        //public string Name { get; set; }
    }
}
