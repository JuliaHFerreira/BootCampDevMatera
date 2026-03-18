using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BootCampDevMatera.Data;

namespace BootCampDevMatera.Controllers
{
    public class OrderItensController : Controller
    {
        private readonly BdContext _context;

        public OrderItensController(BdContext context)
        {
            _context = context;
        }

        // GET: OrderItens
        public async Task<IActionResult> Index()
        {
            var bdContext = _context.OrderItens.Include(o => o.Order).Include(o => o.ProductCodeNavigation);
            return View(await bdContext.ToListAsync());
        }

        // GET: OrderItens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderIten = await _context.OrderItens
                .Include(o => o.Order)
                .Include(o => o.ProductCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderIten == null)
            {
                return NotFound();
            }

            return View(orderIten);
        }

        // GET: OrderItens/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["ProductCode"] = new SelectList(_context.Products, "Code", "Code");
            return View();
        }

        // POST: OrderItens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,IdProduct,Quantity,Value")] OrderIten OrderIten)
        {
            if (ModelState.IsValid)
            {
                _context.Add(OrderIten);
                await _context.SaveChangesAsync();

                await AtualizarValorPedido(OrderIten.OrderId);

                return RedirectToAction(nameof(Index));
            }

            return View(OrderIten);
        }

        // GET: OrderItens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderIten = await _context.OrderItens.FindAsync(id);
            if (orderIten == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderIten.OrderId);
            ViewData["ProductCode"] = new SelectList(_context.Products, "Code", "Code", orderIten.ProductCode);
            return View(orderIten);
        }

        // POST: OrderItens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,ProductCode,Quantity,Value")] OrderIten orderIten)
        {
            if (id != orderIten.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderIten);
                    await _context.SaveChangesAsync();
                    await AtualizarValorPedido(orderIten.OrderId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItenExists(orderIten.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderIten.OrderId);
            ViewData["ProductCode"] = new SelectList(_context.Products, "Code", "Code", orderIten.ProductCode);
            return View(orderIten);
        }

        // GET: OrderItens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderIten = await _context.OrderItens
                .Include(o => o.Order)
                .Include(o => o.ProductCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderIten == null)
            {
                return NotFound();
            }

            return View(orderIten);
        }

        // POST: OrderItens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItens.FindAsync(id);

            if (orderItem != null)
            {
                int orderId = orderItem.OrderId;

                _context.OrderItens.Remove(orderItem);
                await _context.SaveChangesAsync();

                await AtualizarValorPedido(orderId);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderItenExists(int id)
        {
            return _context.OrderItens.Any(e => e.Id == id);
        }

        private async Task AtualizarValorPedido(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItens)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                order.Value = order.OrderItens.Sum(oi => oi.Value);
                await _context.SaveChangesAsync();
            }
        }
    }
}
