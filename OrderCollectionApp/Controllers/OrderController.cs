using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderCollectionApp.Data;
using OrderCollectionApp.Models;

namespace OrderCollectionApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Create(int? id)
        {
            ViewBag.Providers = await GetProviders();

            if (id == null)
            {
                return View();
            }

            var order = await _context.Orders
                .AsNoTracking()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(x => x.ID == id);

            var orderVm = new OrderViewModel()
            {
                ID = order!.ID,
                Number = order.Number,
                Date = order.Date,
                ProviderId = order.ProviderId,
                OrderItems = order.Items
            };

            return View(orderVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderViewModel vm)
        {
            ViewBag.Providers = await GetProviders();

            if (!ModelState.IsValid)
            {
                return View();
            }

            //номера заказов от одного и того же поставщика не должны повторяться
            if (_context.Orders.Any(o => o.Number == vm.Number && o.ProviderId == vm.ProviderId))
            {
                ViewBag.ErrorMessage = "Уже существует заказ с подобным номером от данного поставщика!";

                return View();
            }

            var order = new Order()
            {
                Number = vm.Number,
                Date = vm.Date,
                ProviderId = vm.ProviderId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            //Получаем айди заказа с таким же номером и передаем его модели
            var tempOrder = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Number == vm.Number);

            if (tempOrder != null)
                vm.ID = tempOrder.ID;

            return View(vm);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Providers = await GetProviders();

            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .AsNoTracking()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (order == null)
                return NotFound();

            var vm = new OrderViewModel
            {
                ID = order!.ID,
                Number = order.Number,
                Date = order.Date,
                ProviderId = order.ProviderId,
                OrderItems = order.Items
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderViewModel vm)
        {
            ViewBag.Providers = await GetProviders();

            var order = new Order
            {
                ID = vm.ID,
                Number = vm.Number,
                Date = vm.Date,
                ProviderId = vm.ProviderId
            };

            if (_context.Orders.Any(o => o.ID != order.ID
                && o.Number == order.Number
                && o.ProviderId == vm.ProviderId))
            {
                ViewBag.ErrorMessage = "Уже существует заказ с подобным номером от данного поставщика!";
                return View();
            }

            _context.Entry(order).State = EntityState.Modified;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .AsNoTracking()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(x => x.ID == id);

            if (order == null)
                return NotFound();

            var vm = new OrderViewModel
            {
                OrderItems = order.Items,
                ID = order.ID,
                Number = order.Number,
                Date = order.Date,
                ProviderId = order.ProviderId
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(OrderViewModel vm)
        {

            var order = await _context.Orders
                .Include(s => s.Items)
                .FirstOrDefaultAsync(x => x.ID == vm.ID);

            if (order == null)
            {
                return View();
            }

            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    _context.Remove(item);
                }
            }

            _context.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(s => s.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            var vm = new OrderViewModel()
            {
                OrderItems = order.Items,
                ID = order.ID,
                Number = order.Number,
                Date = order.Date,
                ProviderId = order.ProviderId
            };

            return View(vm);
        }

        private async Task<List<SelectListItem>> GetProviders()
        {
            var providers = _context.Providers.AsNoTracking();
            List<SelectListItem> providersList = new();

            providersList = await providers
                .Select(p => new SelectListItem() { Value = p.Id.ToString(), Text = p.Name })
                .ToListAsync();

            providersList.Insert(0, new SelectListItem() { Value = "", Text = "Выберите поставщика" });

            return providersList;
        }
    }
}