using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quachdaihiep_b2.Data;
using quachdaihiep_b2.DTO;
using quachdaihiep_b2.Model;
//using Microsoft.AspNetCore.Authorization;


namespace quachdaihiep_b2.Controllers
{
    //[Authorize]
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
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int page = 1, int pageSize = 10)
        {
            var categories = await _context.Categories
                                           .Skip((page - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();

            return Ok(categories);
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
        public async Task<ActionResult<Category>> PostCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest();
            }

            var category = new Category
            {
                Cat_Name = categoryDto.Cat_Name,
                Image = categoryDto.Image
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Cat_Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryDTO categoryDto)
        {
            if (id != categoryDto.Cat_Id)
            {
                return BadRequest("Category ID does not match the provided category ID.");
            }

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            existingCategory.Cat_Name = categoryDto.Cat_Name;
            existingCategory.Image = categoryDto.Image;

            try
            {
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

            return NoContent();
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
        /// GET: api/Product/category/2
        [HttpGet("category/{Cat_Id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int Cat_Id)
        {
            var products = await _context.Products
                                         .Where(p => p.Cat_Id == Cat_Id)
                                         .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy sản phẩm
            }

            return Ok(products);  // Trả về các sản phẩm tìm thấy
        }
    }
}