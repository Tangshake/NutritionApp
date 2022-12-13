using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductsCatalog.AsyncDataService.RabbitMQ.Logs;
using ProductsCatalog.AsyncDataService.RabbitMQ.Product;
using ProductsCatalog.Dtos.RabbitMQ;
using ProductsCatalog.Dtos.Request;
using ProductsCatalog.Dtos.Response;
using ProductsCatalog.Repositories;

namespace ProductsCatalog.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogMessageBusClient _logMessageBusClient;
        private readonly IProductMessageBusClient _productMessageBusClient;        

        public ProductsController(IProductRepository repository, ILogMessageBusClient logMessageBusClient, IProductMessageBusClient productMessageBusClient)
        {
            _repository = repository;
            _logMessageBusClient = logMessageBusClient;
            _productMessageBusClient = productMessageBusClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductReadResponseDto>))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [Route("{userId:int}")]
        public async Task<IActionResult> GetAllProductsAsync(int userId)
        {
            Console.WriteLine($"[GetAllProductsAsync]");
            var logDto = new LogDto(){ Date = DateTime.Now, UserId = userId, ServiceName = "ProductsCatalog", Method = "GetAllProductsAsync", Message = $"Get all products."};
            var products = await _repository.GetAllProductsAsync();
            
            if(products is null)
            {
                logDto.Error = "Products not found.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, DateTime.Now);
            }
            
            var productsDto = products.Select(p => p.AsReadDto());
            return Ok(productsDto);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductReadResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("by/id/{userId}/{id}", Name = "GetProductByIdAsync")]
        public async Task<ActionResult<ProductReadResponseDto>> GetProductByIdAsync(int userId, int id)
        {
            Console.WriteLine($"[GetProductByIdAsync]");
            var logDto = new LogDto(){ Date = DateTime.Now, UserId = userId, ServiceName = "ProductsCatalog", Method = "GetProductByIdAsync", Message = $"Get product by id: {id}."};

            if (id < 0) 
            {
                logDto.Error = "Product id less than zero.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return BadRequest("Id has to be a positive number!");
            }

            var product = await _repository.GetProductByIdAsync(id);

            if(product is null)
            {
                logDto.Error = "Product not found.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return NotFound();
            }
            
            _logMessageBusClient.PublishNewLog(log: logDto);
            return Ok(product.AsReadDto());
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProductReadResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("by/name/{userId}/{name}")]
        public async Task<IActionResult> GetProductByNameAsync(int userId, string name)
        {
            Console.WriteLine($"[GetProductByNameAsync]");
            var logDto = new LogDto(){ Date = DateTime.Now, UserId = userId, ServiceName = "ProductsCatalog", Method = "GetProductByNameAsync", Message = $"Get Product by name: {name}"};

            if(string.IsNullOrEmpty(name))
            {
                logDto.Error = "Provided product name is null or empty.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return BadRequest("Name of the product cannot be empty or null.");
            }

            var products = await _repository.GetProductByNameAsync(name);

            if(products is null)
            {
                logDto.Error = "Product not found.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return NotFound();
            }

            var productsDto = products.Select(p => p.AsReadDto());
            _logMessageBusClient.PublishNewLog(log: logDto);

            return Ok(productsDto);
        }

        //[Authorize(Roles = "regular")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("{userId}")]
        public async Task<IActionResult> CreateProductAsync(int userId, [FromBody] ProductCreateRequestDto productCreateDto)
        {
            Console.WriteLine($"[CreateProductAsync]");
            var logDto = new LogDto(){ Date = DateTime.Now, UserId = userId, ServiceName = "ProductsCatalog", Method = "CreateProductAsync", Message = $"Create product."};
            
            if(productCreateDto is null)
            {
                logDto.Error = "Product to create cannot be null.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return BadRequest();
            }

            var product = new Entities.Product()
            {
                Name = productCreateDto.Name,
                Manufacturer = productCreateDto.Manufacturer,
                Carbohydrates = productCreateDto.Carbohydrates,
                Fat = productCreateDto.Fat,
                Kcal = productCreateDto.Kcal,
                Protein = productCreateDto.Protein,
                Roughage = productCreateDto.Roughage
            };

            var result = await _repository.CreateProductAsync(product);

            if(result != 1)
            {
                logDto.Error = "Product could not be created.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return StatusCode(StatusCodes.Status500InternalServerError, DateTime.Now);
            }
            
            _logMessageBusClient.PublishNewLog(log: logDto);
            return CreatedAtRoute(nameof(GetProductByIdAsync), new {UserId = userId,Id = product.AsReadDto().Id}, product.AsReadDto());
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteProductAsync(int userId, int id)
        {
            Console.WriteLine($"[DeleteProductAsync]");
            var logDto = new LogDto(){ Date = DateTime.Now, UserId = userId, ServiceName = "ProductsCatalog", Method = "DeleteProductAsync", Message = $"Delete product."};

            if (id < 0) 
            {
                logDto.Error = "Product id less then zero.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return BadRequest("Id has to be a positive number!");
            }

            var existingProduct = await GetProductByIdAsync(userId, id);

            if(existingProduct is null)
            {
                logDto.Error = "Product to delete could not be found.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return NotFound($"Product with id {id} was not found.");
            }

            var result = await _repository.RemoveProductAsync(id);
            
            if(result != 1)
            {
                logDto.Error = "Product could not be deleted.";
                _logMessageBusClient.PublishNewLog(log: logDto);

                return StatusCode(StatusCodes.Status500InternalServerError, DateTime.Now);
            }
                
            _productMessageBusClient.PublishProductRemoved(id);
            _logMessageBusClient.PublishNewLog(log: logDto);
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{userId}/{id}")]
        public async Task<IActionResult> UpdateProductAsync(int userId, int id, ProductUpdateRequestDto productUpdateDto)
        {
            Console.WriteLine($"[UpdateProductAsync]");
            var logDto = new LogDto(){ Date = DateTime.Now, UserId = userId, ServiceName = "ProductsCatalog", Method = "UpdateProductAsync", Message = $"Update product with id: {id}."};

            if (id < 0) 
            {
                logDto.Error = "Product id is less than zero.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return BadRequest("Id has to be a positive number!");
            }

            var existingProduct = await _repository.GetProductByIdAsync(id);
            
            if(existingProduct is null) 
            {
                logDto.Error = "Product to update was not found.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return NotFound($"Product with id {id} was not found.");
            }
            
            if(productUpdateDto is null)
            {
                logDto.Error = "Product update data cannot be null.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return BadRequest();
            }

            var product = new Entities.Product()
            {
                Id = id,
                Name = productUpdateDto.Name,
                Manufacturer = productUpdateDto.Manufacturer,
                Carbohydrates = productUpdateDto.Carbohydrates,
                Fat = productUpdateDto.Fat,
                Kcal = productUpdateDto.Kcal,
                Protein = productUpdateDto.Protein,
                Roughage = productUpdateDto.Roughage
            };

            var result = await _repository.UpdateProductAsync(product);
            
            if(result != 1)
            {
                logDto.Error = "Product could not be updated.";
                _logMessageBusClient.PublishNewLog(log: logDto);
                return StatusCode(StatusCodes.Status500InternalServerError, DateTime.Now);
            }

            _logMessageBusClient.PublishNewLog(log: logDto);
            return NoContent();
        }
    }
}