using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BlogApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly BlogContext _context;
        public CommentController(BlogContext context)
        {
            _context = context;
        }

        // POST: /Comment/Add
        [HttpPost]
        public async Task<IActionResult> Add(int postId, string author, string text)
        {
            if (string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(text))
                return BadRequest();

            var comment = new Comment { PostId = postId, Author = author, Text = text };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var comments = await _context.Comments.Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt).ToListAsync();
            return PartialView("_CommentListPartial", comments);
        }

        // POST: /Comment/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return NotFound();
            int postId = comment.PostId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            var comments = await _context.Comments.Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt).ToListAsync();
            return PartialView("_CommentListPartial", comments);
        }
    }
} 