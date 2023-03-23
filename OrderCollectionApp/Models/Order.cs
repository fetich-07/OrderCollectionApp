using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderCollectionApp.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ID { get; set; }
        [Required]
		[Column(TypeName = "nvarchar(max)")]
		public string? Number { get; set; }
		[Required]
		[Column(TypeName = "datetime2")]
		public DateTime? Date { get; set; }
		[Required]
		public int? ProviderId { get; set; }
		[Required]
		public List<OrderItem>? Items { get; set;}
	}
}