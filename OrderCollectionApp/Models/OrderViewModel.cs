using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OrderCollectionApp.Models
{
    public class OrderViewModel
    {
        //Данные конкретного заказа
        public int? ID { get; set; }
        [Required]
        public string? Number { get; set; }
        [Required]
        public DateTime? Date { get; set; }
        [Required]
        public int? ProviderId { get; set; }

        //Заказы для отображения
        public List<Order>? Orders { get; set; }
        public List<OrderItem>? OrderItems { get; set; }

        //Списки для фильтрации
        public List<int>? Numbers { get; set; }
        public List<int>? ProviderIDs { get; set; }
        public List<string>? ItemNames { get; set; }
        public List<string>? ItemUnits { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }
    }
}
