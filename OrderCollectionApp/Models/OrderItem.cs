using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderCollectionApp.Models
{
	public class OrderItem
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int? ID { get; set; }
        public int? OrderID { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(max)")]
		public string? Name { get; set; }
		[Required]
		[Column(TypeName = "decimal(18, 3)")]
		public decimal? Quantity { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(max)")]
		public string? Unit { get; set; }
	}
}