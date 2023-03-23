using System.Collections;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderCollectionApp.Data;
using OrderCollectionApp.Models;

namespace OrderCollectionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;


        }

        public IActionResult Index(OrderViewModel vm)
        {
            var orders = GetFilteredOrdersList(vm);

            var numbers = GetDataForFiltering()["numbers"] as List<string>;
            var providers = GetDataForFiltering()["providers"] as List<Provider>;

            ViewBag.Numbers = new SelectList(numbers);
            ViewBag.Providers = new SelectList(providers, "Id", "Name");


            return View(new OrderViewModel { Orders = orders });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Private Methods

        private List<Order> GetFilteredOrdersList(OrderViewModel vm)
        {
            IEnumerable<Order> orders = _context.Orders
               .Include(o => o.Items).ToList();

            if (vm.DateFrom != null)
            {
                orders = orders.Where(o => o.Date >= vm.DateFrom);
            }

            if (vm.DateTo != null)
            {
                orders = orders.Where(o => o.Date <= vm.DateTo);
            }

            if (vm.Numbers != null && vm.Numbers.Count > 0)
            {
                orders = orders.Where(o => vm.Numbers.Any(n => n != 0 && n.ToString() == o.Number));
            }

            if (vm.ProviderIDs != null && vm.ProviderIDs.Count > 0)
            {
                orders = orders.Where(o => vm.ProviderIDs.Any(p => p == o.ProviderId));
            }

            return orders.ToList();
        }

        private Hashtable GetDataForFiltering()
        {
            Hashtable data = new();
            var providers = _context.Providers
                .AsNoTracking()
                .Select(x => x)
                .Distinct()
                .ToList();

            var numbers = _context.Orders
                .AsNoTracking()
                .Select(x => x.Number)
                .Distinct()
                .ToList();

            data.Add("providers", providers);
            data.Add("numbers", numbers);

            return data;       
        }

        #endregion
    }
}