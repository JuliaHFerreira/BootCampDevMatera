using BootCampDevMatera.Data;
using BootCampDevMatera.Views.OrderItens;
using BootCampDevMatera.Views.Orders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var orders = await _context.Orders
                .Include(o => o.IdClientNavigation)
                .Include(o => o.IdSellerNavigation)
                .Include(o => o.OrderItens)
                    .ThenInclude(oi => oi.ProductCodeNavigation)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Value = order.OrderItens.Sum(oi => oi.Value);
            }

            return View(orders);
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
            var model = new OrderFormViewModel
            {
                DateOrder = DateTime.Today,
                Itens = new List<OrderItenFormViewModel>
        {
            new OrderItenFormViewModel()
        }
            };

            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name");
            ViewBag.Products = _context.Products
                .Select(p => new
                {
                    p.Code,
                    p.Description,
                    p.Price,
                    Display = p.Code + " - " + p.Description
                })
                .ToList();

            return View(model);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderFormViewModel model)
        {
            if (model.Itens == null || !model.Itens.Any())
            {
                ModelState.AddModelError("", "Adicione pelo menos um item.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name", model.IdClient);
                ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name", model.IdSeller);
                ViewBag.Products = _context.Products
                    .Select(p => new
                    {
                        p.Code,
                        p.Description,
                        p.Price
                    })
                    .ToList();
                return View(model);
            }

            var order = new Order
            {
                DateOrder = model.DateOrder,
                IdClient = model.IdClient,
                IdSeller = model.IdSeller,
                Value = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            Decimal totalOrder = 0;

            foreach (var item in model.Itens)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Code == item.ProductCode);

                if (product == null)
                {
                    ModelState.AddModelError("", $"Produto {item.ProductCode} não encontrado.");
                    continue;
                }

                item.Value = product.Price * item.Quantity;
                totalOrder += item.Value;

                var orderIten = new OrderIten
                {
                    OrderId = order.Id,
                    ProductCode = item.ProductCode,
                    Quantity = item.Quantity,
                    Value = item.Value
                };

                _context.OrderItens.Add(orderIten);
            }

            order.Value = totalOrder;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItens)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var model = new OrderFormViewModel
            {
                Id = order.Id,
                DateOrder = order.DateOrder,
                IdClient = order.IdClient,
                IdSeller = order.IdSeller,
                Value = order.Value,
                Itens = order.OrderItens.Select(i => new OrderItenFormViewModel
                {
                    Id = i.Id,
                    ProductCode = i.ProductCode,
                    Quantity = i.Quantity,
                    Value = i.Value
                }).ToList()
            };

            if (!model.Itens.Any())
            {
                model.Itens.Add(new OrderItenFormViewModel());
            }

            ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name");
            ViewBag.Products = _context.Products
                .Select(p => new
                {
                    p.Code,
                    p.Description,
                    p.Price,
                    Display = p.Code + " - " + p.Description
                })
                .ToList();

            return View(model);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderFormViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderItens)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (model.Itens == null || !model.Itens.Any())
            {
                ModelState.AddModelError("", "Adicione pelo menos um item.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["IdClient"] = new SelectList(_context.Clients, "Id", "Name", model.IdClient);
                ViewData["IdSeller"] = new SelectList(_context.Sellers, "Id", "Name", model.IdSeller);
                ViewBag.Products = _context.Products
                    .Select(p => new
                    {
                        p.Code,
                        p.Description,
                        p.Price,
                        Display = p.Code + " - " + p.Description
                    })
                    .ToList();
                return View(model);
            }

            order.DateOrder = model.DateOrder;
            order.IdClient = model.IdClient;
            order.IdSeller = model.IdSeller;

            _context.OrderItens.RemoveRange(order.OrderItens);

            Decimal totalOrder = 0;

            foreach (var item in model.Itens)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Code == item.ProductCode);

                if (product == null)
                    continue;

                Decimal itemValue = product.Price * item.Quantity;

                var orderItem = new OrderIten
                {
                    OrderId = order.Id,
                    ProductCode = item.ProductCode,
                    Quantity = item.Quantity,
                    Value = itemValue
                };

                _context.OrderItens.Add(orderItem);
                totalOrder += itemValue;
            }

            order.Value = totalOrder;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
