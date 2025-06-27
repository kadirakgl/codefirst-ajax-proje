using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogContext _context;
        public PostController(BlogContext context)
        {
            _context = context;
        }

        // GET: /Post
        public async Task<IActionResult> Index(string search = null)
        {
            var posts = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                posts = posts.Where(p => p.Title.Contains(search));
            }
            return View(await posts.OrderByDescending(p => p.CreatedAt).ToListAsync());
        }

        // GET: /Post/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts.Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();
            return View(post);
        }

        // GET: /Post/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Post/Create
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState geçersiz!");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"{key}: {error.ErrorMessage}");
                    }
                }
                return View(post);
            }
            Console.WriteLine($"Post Title: {post.Title}, Content: {post.Content}");
            _context.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Post/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Post/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        // POST: /Post/Edit/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return NotFound();
            if (!ModelState.IsValid) return View(post);
            var existing = await _context.Posts.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Title = post.Title;
            existing.Content = post.Content;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Post/GetPostCount
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult GetPostCount()
        {
            var count = _context.Posts.Count();
            return Json(new { count });
        }
    }
} 