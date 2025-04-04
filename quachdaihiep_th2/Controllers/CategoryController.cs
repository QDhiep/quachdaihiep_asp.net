using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quachdaihiep_th2.Data;
using quachdaihiep_th2.Models;

namespace quachdaihiep_th2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor injection to get the DbContext
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);  // Return a list of categories
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();  // Return 404 if category not found
            }

            return Ok(category);  // Return the category if found
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest();  // Return 400 if category is invalid
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Cat_Id }, category);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Cat_Id)
            {
                return BadRequest("Category ID does not match the provided category ID.");
            }

            // Kiểm tra xem Category có tồn tại trong cơ sở dữ liệu hay không
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            // Cập nhật thông tin của Category
            existingCategory.Cat_Name = category.Cat_Name;
            existingCategory.Image = category.Image;

            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Entry(existingCategory).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(c => c.Cat_Id == id))
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về HTTP status code 204 khi cập nhật thành công
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();  // Return 404 if the category not found
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();  // Return 204 if deletion is successful
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Cat_Id == id);
        }
    }
}