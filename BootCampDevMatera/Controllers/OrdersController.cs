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
    public class OrdersController : Controller
    {
        private readonly BdContext _context;

        public OrdersController(BdContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var bdContext = _context.Orders.Include(o => o.IdClientNavigation).Include(o => o.IdProductNavigation).Include(o => o.IdSellerNavigation);
            return View(await bdContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdClientNavigation)
                .Include(o => o.IdProductNavigation)
                .Include(o => o.IdSellerNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["IdProduct"] = new SelectList(_context.Products, "Id", "Code");
            ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdProduct,Quantity,DateOrder,IdClient,IdSeller")] Order order)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == order.IdProduct);

            if (product == null)
            {
                ModelState.AddModelError("IdProduct", "Produto não encontrado.");
            }

            if (order.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "A quantidade deve ser maior que zero.");
            }

            order.Value = product.Price * order.Quantity;

                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            // preenche o Value novamente caso volte para a tela com erro
            if (product != null)
            {
                order.Value = product.Price * order.Quantity;
            }

            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name", order.IdClient);
            ViewData["IdProduct"] = new SelectList(_context.Products, "Id", "Code", order.IdProduct);
            ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name", order.IdSeller);

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name", order.IdClient);
            ViewData["IdProduct"] = new SelectList(_context.Products, "Id", "Code", order.IdProduct);
            ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name", order.IdSeller);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdProduct,Quantity,Value,DateOrder,IdClient,IdSeller")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name", order.IdClient);
            ViewData["IdProduct"] = new SelectList(_context.Products, "Id", "Code", order.IdProduct);
            ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name", order.IdSeller);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.IdClientNavigation)
                .Include(o => o.IdProductNavigation)
                .Include(o => o.IdSellerNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
