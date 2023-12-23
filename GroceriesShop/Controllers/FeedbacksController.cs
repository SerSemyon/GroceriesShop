using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroceriesShop;
using GroceriesShop.Models;

namespace GroceriesShop.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly GroceriesContext _context;

        public FeedbacksController(GroceriesContext context)
        {
            _context = context;
        }

        [Route("api/Feedbacks/{productId?}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks(int? productId)
        {
            return _context.Feedbacks.Where(f => f.ProductId == productId).ToList();
        }

        [Route("api/Feedbacks/{productId?}")]
        [HttpPost]
        public async Task<ActionResult<Feedback>> PostFeedback(int? productId, [FromBody]Feedback feedback)
        {
            feedback.ProductId = productId;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeedback", new { id = feedback.Id }, feedback);
        }

        [Route("Feedbacks")]
        public async Task<IActionResult> Index()
        {
            var groceriesContext = _context.Feedbacks.Include(f => f.Account).Include(f => f.Product);
            return View(await groceriesContext.ToListAsync());
        }

        [Route("oldFeedbacks")]
        public async Task<IActionResult> OldFeedbacks()
        {
            var groceriesContext = _context.Feedbacks
                .Include(f => f.Account)
                .Include(f => f.Product)
                .Where(a => a.Account.Age > 30)
                .Where(a => a.Product.Name == "Шаурма классическая");
            return View("Index",await groceriesContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Account)
                .Include(f => f.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id,Rating,TextFeedback,Id")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", feedback.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", feedback.ProductId);
            return View(feedback);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", feedback.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", feedback.ProductId);
            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Id,Rating,TextFeedback,Id")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Id", feedback.AccountId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", feedback.ProductId);
            return View(feedback);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Account)
                .Include(f => f.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }
    }
}

