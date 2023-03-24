using System.ComponentModel.DataAnnotations;

namespace OrderCollectionApp.Models
{
    public class ItemViewModel
    {
        public int? ID { get; set; }
        [Required]
        public int? OrderID { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public decimal? Quantity { get; set; }
        [Required]
        public string? Unit { get; set; }

        public OrderViewModel? OrderVm { get; set; }

        public string? Method { get; set; }
    }
}
