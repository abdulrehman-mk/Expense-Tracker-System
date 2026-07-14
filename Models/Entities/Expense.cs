using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace technova_ecommerce.Models.Entities
{

    [Table("expenses")]
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("expense_id")]
        public int ExpenseId { get; set; }

        [Required]
        [Column("title")]
        [StringLength(150)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Title can only contain letters.")]

        public string Title { get; set; }

        [Required]
        [Column("amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }

        [Required]
        [Column("expense_date")]
        public DateTime ExpenseDate { get; set; }

        // Foreign Key
        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        // Navigation Property
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
