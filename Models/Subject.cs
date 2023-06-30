using System.ComponentModel.DataAnnotations;

namespace MagistriMVC.Models {
    public class Subject {
        public int Id { get; set; }
        [StringLength(35)]
        public string Name { get; set; }     
    }
}
