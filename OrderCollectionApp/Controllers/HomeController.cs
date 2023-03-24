using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderCollectionApp.Data;
using OrderCollectionApp.Models;
using System.Collections;
using System.Diagnostics;

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
            var units = GetDataForFiltering()["units"] as List<string>;
            var names = GetDataForFiltering()["names"] as List<string>;


            ViewBag.Numbers = new SelectList(numbers);
            ViewBag.Providers = new SelectList(providers, "Id", "Name");
            ViewBag.Units = new SelectList(units);
            ViewBag.Names = new SelectList(names);

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

            if (vm.DateFrom != null
                && vm.DateTo != null)
            {
                if (!(vm.DateFrom <= vm.DateTo))
                {
                    return new List<Order>();
                }

                orders = orders.Where(o => o.Date >= vm.DateFrom && o.Date <= vm.DateTo);
            }

            else if (vm.DateFrom != null)
            {
                orders = orders.Where(o => o.Date >= vm.DateFrom);
            }
            else if (vm.DateTo != null)
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

            if (vm.ItemUnits != null && !vm.ItemUnits[0].Contains("Выбрать все"))
            {
                orders = orders.Where(o => o.Items != null
                    && o.Items.Any(i => i.Unit != null && vm.ItemUnits.Contains(i.Unit)));
            }

            if (vm.ItemNames != null && !vm.ItemNames[0].Contains("Выбрать все"))
            {
                orders = orders.Where(o => o.Items != null
                    && o.Items.Any(i => i.Name != null && vm.ItemNames.Contains(i.Name)));
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

            var units = _context.OrderItems
                .AsNoTracking()
                .Select(x => x.Unit)
                .Distinct()
                .ToList();

            var names = _context.OrderItems
                .AsNoTracking()
                .Select(x => x.Name)
                .Distinct()
                .ToList();

            data.Add("providers", providers);
            data.Add("numbers", numbers);
            data.Add("units", units);
            data.Add("names", names);

            return data;
        }

        #endregion
    }
}