using System.ComponentModel.DataAnnotations;

namespace quachdaihiep_b2.DTO
{
    public class OrderDTO
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // FK to User
        public int UserId { get; set; }

    }
}
