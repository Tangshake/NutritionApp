using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PredefinedMeals.Dtos.Response;
using PredefinedMeals.Dtos.Request;
using PredefinedMeals.Entities;
using PredefinedMeals.Repositories;
using Microsoft.AspNetCore.Http;
using PredefinedMeals.Dtos.RabbitMQ.Request;
using PredefinedMeals.AsyncDataServices.RabbitMQ;

namespace PredefinedMeals.Controllers
{
    [ApiController]
    [Route("api/meals")]
    public class MealsController : ControllerBase
    {
        private readonly IMealsRepository _repository;
        private readonly IMessageBusClient _messageBusClient;

        public MealsController(IMealsRepository repository, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [Route("/api/meals/{userId}")]
        public async Task<IActionResult> GetAllAsync(int userId)
        {
            Console.WriteLine($"[GetAllAsync] User:{userId} wants to see all his meals.");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "Meals", UserId = userId, Method = "GetAllAsync", Message = $"Get all meals."};

            if (userId < 0)
            {
                logDto.Error = $"Users {userId} has to be a positive number.";
                _messageBusClient.PublishNewLog(logDto);
                return BadRequest("Id has to be a positive number!");
            }

            var meals = await _repository.GetAllAsync(userId);

            if(meals is null)
            {
                logDto.Error = $"Database contains no meals.";
                _messageBusClient.PublishNewLog(logDto);

                return NotFound();
                //return StatusCode(StatusCodes.Status503ServiceUnavailable, DateTime.Now);
            }

            _messageBusClient.PublishNewLog(logDto);
            var mealResponseDto = meals.Select(meal => meal.AsReadDto());

            return Ok(mealResponseDto);
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MealResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("by/id/{userId}/{id}", Name = "GetMealByIdAsync")]
        public async Task<IActionResult> GetMealByIdAsync(Guid id, int userId)
        {
            Console.WriteLine($"[GetMealByIdAsync] User:{userId} want a meal with Id: {id}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "Meals", UserId = userId, Method = "GetMealByIdAsync", Message = $"Get meal by id."};
            bool isValid = Guid.TryParse(id.ToString(), out _);

            if(!isValid)
            {
                logDto.Error = $"Meal id is not valid GUID.";
                _messageBusClient.PublishNewLog(logDto);

                return BadRequest("Guid is not valid");
            }

            if(userId < 0)
            {
                logDto.Error = $"User id has to be a positive number.";
                _messageBusClient.PublishNewLog(logDto);
                return BadRequest("User id has to be a positive number.");
            }

            var result = await _repository.GetByIdAsync(id, userId);
            
            if(result is null)
            {
                logDto.Error = $"Database has no meal with id: {id}";
                _messageBusClient.PublishNewLog(logDto);
                return NotFound();
            }

            _messageBusClient.PublishNewLog(logDto);
            var mealResponseDto = result.AsReadDto();

            return Ok(mealResponseDto);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MealResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("by/name/{userId}/{name}")]
        public async Task<IActionResult> GetMealByNameAsync(int userId, string name)
        {
            Console.WriteLine($"[GetMealByNameAsync] User:{userId} wants all the meals with the name: {name}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "Meals", UserId = userId, Method = "GetMealByNameAsync", Message = $"Get meal by name."};

            if(string.IsNullOrEmpty(name))
            {
                logDto.Error = $"Name of the meal is null or empty.";
                _messageBusClient.PublishNewLog(logDto);
                return BadRequest("Name of the meal is null or empty.");
            }

            if(userId < 0)
            {
                logDto.Error = $"User id has to be a positive number.";
                _messageBusClient.PublishNewLog(logDto);
                return BadRequest("User id has to be a positive number");
            }

            var result = await _repository.GetByNameAsync(name, userId);
            
            if(result is null)
            {
                logDto.Error = $"Database contains no meal with name: {name}";
                _messageBusClient.PublishNewLog(logDto);
                return NotFound();
            }
            
            _messageBusClient.PublishNewLog(logDto);
            var mealResponseDto = result.Select(x=>x.AsReadDto()).ToList();
            return Ok(mealResponseDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMealAsync([FromBody] MealCreateRequestDto meal)
        {
            Console.WriteLine($"[CreateMealAsync] Request to add user defined meal.");
            Console.WriteLine($"[CreateMealAsync] Name: {meal.Name} with {meal.Ingredients.Count} ingredients by User: {meal.UserId}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "Meals", UserId = meal.UserId, Method = "CreateMealAsync", Message = $"Creating meal."};

            if(meal is null)
            {
                logDto.Error = $"Meal cannot be null.";
                _messageBusClient.PublishNewLog(logDto);

                return BadRequest("Meal cannot be null");
            }

            var predefinedMeal = new Meal()
            {
                Name = meal.Name,
                UserId = meal.UserId
            };

            var ingredients = meal.Ingredients.Select(x=>x.AsEntity());
            
            predefinedMeal.Ingredients = new List<Ingredient>();
            predefinedMeal.Ingredients.AddRange(ingredients);

            var result = await _repository.CreateAsync(predefinedMeal);
            Console.WriteLine($"[CreateMealAsync] Meal has been created with id: {result}");

            if(result == Guid.Empty)
            {
                logDto.Error = $"Could not create meal.";
                _messageBusClient.PublishNewLog(logDto);

                return StatusCode(StatusCodes.Status500InternalServerError, DateTime.Now); 
            }

            _messageBusClient.PublishNewLog(logDto);

            return CreatedAtRoute(
                nameof(GetMealByIdAsync),
                new { Id = predefinedMeal.Id, UserId = predefinedMeal.Name },
                predefinedMeal.AsReadDto());
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}")]
        public async Task<IActionResult> UpdateMealAsync(int userId, [FromBody] MealUpdateRequestDto meal)
        {
            Console.WriteLine($"[UpdateMealAsync] User {userId} request to update his meal with id {meal.Id}. Name {meal.Name}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "Meals", UserId = userId, Method = "UpdateMealAsync", Message = $"Updating meal."};

            if(userId < 0)
            {
                logDto.Error = $"User id has to be a positive number.";
                _messageBusClient.PublishNewLog(logDto);
                return BadRequest("Id has to be a positive number");
            }
    
            if(meal is null)
            {
                logDto.Error = $"Meal cannot be null.";
                _messageBusClient.PublishNewLog(logDto);
                return BadRequest("Meal cannot be null.");
            }

            var existing = await _repository.GetByIdAsync(meal.Id, userId);

            if(existing is null)
            {
                logDto.Error = $"Could not find meal to update.";
                _messageBusClient.PublishNewLog(logDto);
                return NotFound();
            }

            existing.Name = meal.Name;
            existing.Ingredients.Clear();
            var ingredients = meal.Ingredients.Select(x=>x.AsEntity());
            existing.Ingredients.AddRange(ingredients);
            
            var result = await _repository.UpdateAsync(existing);
            Console.WriteLine($"[UpdateMealAsync] Update result: {result}");
            
            _messageBusClient.PublishNewLog(logDto);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{userId}/{id}")]
        public async Task<IActionResult> DeleteMealAsync(Guid id, int userId)
        {
            Console.WriteLine($"[DeleteMealAsync] User {userId} request to remove his meal with id {id}.");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "Meals", UserId = userId, Method = "DeleteMealAsync", Message = $"Deleting meal."};

            bool isValid = Guid.TryParse(id.ToString(), out _);
            
            if(!isValid)
            {
                logDto.Error = $"Meal id is not valid GUID.";
                _messageBusClient.PublishNewLog(logDto);

                return BadRequest("Guid is not valid");
            }

            if(userId < 0)
            {
                logDto.Error = $"User id has to be a positive number.";
                _messageBusClient.PublishNewLog(logDto);

                return BadRequest("Id has to be a positive number");
            }

            var item = await _repository.GetByIdAsync(id, userId);

            if(item is null)
            {
                logDto.Error = $"Could not find meal to delete.";
                _messageBusClient.PublishNewLog(logDto);
                return NotFound();
            }

            var result = await _repository.RemoveAsync(item.Id, userId);

            if(result != 1)
            {
                logDto.Error = $"Could not remove meal.";
                _messageBusClient.PublishNewLog(logDto);
                return StatusCode(StatusCodes.Status500InternalServerError, DateTime.Now); 
            }

            _messageBusClient.PublishNewLog(logDto);
            return NoContent();
        }
    }
}