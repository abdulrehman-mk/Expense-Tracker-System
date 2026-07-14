using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using technova_ecommerce.Models.Entities;


namespace technova_ecommerce.Controllers
{
    [Authorize]
    public class ExpensesController : Controller
    {
        private readonly DatabaseContext _context;

        public ExpensesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(DateTime? selectedMonth)
        {
            var databaseContext = _context.Expense
                .Include(e => e.Category)
                .AsQueryable();

            // Apply month filter if selected
            if (selectedMonth.HasValue)
            {
                int month = selectedMonth.Value.Month;
                int year = selectedMonth.Value.Year;

                databaseContext = databaseContext.Where(e =>
                    e.ExpenseDate.Month == month &&
                    e.ExpenseDate.Year == year);
            }

            // Calculate total (filtered or full)
            ViewBag.TotalExpenses = await databaseContext.SumAsync(e => e.Amount);

            // Keep selected month in UI
            ViewBag.SelectedMonth = selectedMonth?.ToString("yyyy-MM");

            return View(await databaseContext.ToListAsync());
        }


        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseId,Title,Amount,ExpenseDate,CategoryId")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseId,Title,Amount,ExpenseDate,CategoryId")] Expense expense)
        {
            if (id != expense.ExpenseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.ExpenseId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", expense.CategoryId);
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.ExpenseId == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expense.FindAsync(id);
            if (expense != null)
            {
                _context.Expense.Remove(expense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expense.Any(e => e.ExpenseId == id);
        }


        public IActionResult Chart()
        {
            // Get all categories with total expenses
            var data =  _context.Expense
                .Include(e => e.Category)
                .GroupBy(e => e.Category.CategoryName)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(e => e.Amount)
                })
                .ToList();

            // Ensure we always have arrays, even if empty
            ViewBag.Labels = data.Select(d => d.Category).ToArray();
            ViewBag.Totals = data.Select(d => d.Total).ToArray();

            return View();
        }


    } 
}
