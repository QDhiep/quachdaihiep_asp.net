using System.ComponentModel.DataAnnotations;

namespace quachdaihiep_th2.Models
{
    public class Category
    {
        [Key]
        public int Cat_Id { get; set; }
        public string Cat_Name { get; set; }
        public string Image { get; set; }
    }
}
