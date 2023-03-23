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

        public async Task<IActionResult> Details(int? id)
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

        [HttpGet]
        public IActionResult Create(int? id)
        {
            if (id == null)
                return NotFound();

            var itemVm = new ItemViewModel()
            {
                OrderID = id
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

            var order = await _context.Orders.
                Include(s => s.Items)
                .FirstOrDefaultAsync(x => x.ID == vm.OrderID);

            if (order!.Items == null)
            {
                order.Items = new List<OrderItem>();
            }

            order.Items.Add(new OrderItem()
            {
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
            
            return RedirectToAction("Create", "Order", new { id = orderVm.ID });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            return View(orderItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ID,Name,Quantity,Unit")] OrderItem orderItem)
        {
            if (id != orderItem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!OrderItemExists(orderItem.ID))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderItem);
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

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'AppDbContext.OrderItems'  is null.");
            }
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return (_context.OrderItems?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
