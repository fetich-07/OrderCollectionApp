using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderCollectionApp.Data;
using OrderCollectionApp.Models;

namespace OrderCollectionApp.Controllers
{
    public class ItemController : Controller
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create(int? id, string? method)
        {
            if (id == null)
                return NotFound();

            var itemVm = new ItemViewModel()
            {
                OrderID = id,
                Method = method
            };

            return View(itemVm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ItemViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            if (_context.Orders.Any(o => o.ID == vm.OrderID && o.Number == vm.Name))
            {
                ViewBag.ErrorMessage = "Название элемента заказа не может быть равным номеру заказа!";
                return View();
            }

            var order = await _context.Orders.
            Include(s => s.Items)
            .FirstOrDefaultAsync(x => x.ID == vm.OrderID);

            if (order!.Items == null)
            {
                order.Items = new List<OrderItem>();
            }

            order.Items.Add(new OrderItem()
            {
                OrderID = vm.OrderID, 
                Name = vm.Name,
                Quantity = vm.Quantity,
                Unit = vm.Unit
            });
            await _context.SaveChangesAsync();

            var orderVm = new OrderViewModel()
            {
                ID = order.ID,
                Number = order.Number!,
                Date = order.Date,
                ProviderId = order.ProviderId,
                OrderItems = order.Items
            };

            return RedirectToAction(vm.Method, "Order", new { id = orderVm.ID });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View();
            }

            var orderItem = await _context.OrderItems.FirstOrDefaultAsync(o => o.ID == id);
            if (orderItem != null)
            {
                var vm = new ItemViewModel()
                {
                    ID = orderItem.ID,
                    OrderID = orderItem.OrderID,
                    Name = orderItem.Name,
                    Quantity = orderItem.Quantity,
                    Unit = orderItem.Unit
                };

                return View(vm);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel vm)
        {
            if (_context.Orders.Any(o => o.ID == vm.OrderID && o.Number == vm.Name))
            {
                ViewBag.ErrorMessage = "Название элемента заказа не может быть равным номеру заказа!";
                return View();
            }

            var item = new OrderItem()
            {
                ID = vm.ID,
                OrderID = vm.OrderID,
                Name = vm.Name,
                Quantity = vm.Quantity,
                Unit = vm.Unit
            };

            _context.Entry(item).State = EntityState.Modified;
            _context.OrderItems.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Order", new { id = vm.OrderID });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.ID == id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Order", new { id = orderItem.ID });
            }
            return View(orderItem);
        }
    }
}
