using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreAngular.Data;
using AspNetCoreAngular.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreAngular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {


        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }


        // GET: api/Product
        [HttpGet("[action]")]

        [Authorize(Policy ="RequiredLoogedIn")]
        public IActionResult GetProduct()
        {
            return Ok(_db.Products.ToList());
        }

     

        // POST: api/Product
        [HttpPost("[action]")]
        [Authorize(Policy = "RequredAdministratorRole")]
        public async  Task<IActionResult> AddProduct([FromBody] ProductModel formdata)
        {
            var newproduct = new ProductModel
            {
                Name = formdata.Name,
                ImageUrl = formdata.ImageUrl,
                Description = formdata.Description,
                OutOfStock = formdata.OutOfStock,
                Price = formdata.Price
            };

            await _db.Products.AddAsync(newproduct);

            await _db.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/Product/5
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateProduct([FromRoute]int id, [FromBody] ProductModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProduct = _db.Products.FirstOrDefault(p => p.ProductId == id);
            if(findProduct == null)
            {
                return NotFound();

            }
            findProduct.Name = formdata.Name;
            findProduct.Description = formdata.Description;
            findProduct.ImageUrl = formdata.ImageUrl;
            findProduct.OutOfStock = formdata.OutOfStock;
            findProduct.Price = formdata.Price;

            _db.Entry(findProduct).State = EntityState.Modified;

            await _db.SaveChangesAsync();
            return Ok(new JsonResult("the product with id " + id + "was update"));
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("[action]")]
        public async Task<IActionResult>DeleteProduct([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

                var findProduct = await _db.Products.FindAsync(id);

                if(findProduct == null)
                {
                    return NotFound();

                }

                _db.Products.Remove(findProduct);

                await _db.SaveChangesAsync();

                return Ok(new JsonResult("the product with id " + id + "was delete"));

            
        }
    }
}
