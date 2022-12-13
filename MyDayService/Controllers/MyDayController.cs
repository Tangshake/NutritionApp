using System;
using System.Threading.Tasks;
using AutoMapper;
using MealsCatalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDayService.AsyncDataServices.RabbitMQ.Log;
using MyDayService.Dtos.RabbitMQ.Request;
using MyDayService.Dtos.Request;
using MyDayService.Dtos.Response;
using MyDayService.Entity;
using MyDayService.Repository;
using MyDayService.SyncDataServices.Grpc;
using ProductsCatalog;

namespace MyDayService.Controllers
{
    [ApiController]
    [Route("/api/myday")]
    public class MyDayController : ControllerBase
    {
        private readonly IDayOfEatingRepository _repository;
        private readonly IProductsDataClient _productsDataClient;
        private readonly IMealsDataClient _mealsDataClient;
        private readonly IMapper _mapper;
        private readonly ILogMessageBusClient _logMessageBusClient;

        public MyDayController(IDayOfEatingRepository repository, IProductsDataClient productsDataClient, IMealsDataClient mealsDataClient, IMapper mapper, ILogMessageBusClient logMessageBusClient)
        {
            _repository = repository;
            _productsDataClient = productsDataClient;
            _mealsDataClient = mealsDataClient;
            _mapper = mapper;
            _logMessageBusClient = logMessageBusClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoeResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("{userId}/{date}", Name = "GetDoeByDateAsync")]
        public async Task<IActionResult> GetDoeByDateAsync(int userId, DateTime date)
        {
            Console.WriteLine($"[GetDoeByDate] User: {userId} requested his doe from date: {date}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "MyDay", UserId = userId, Method = "GetDoeByDateAsync", Message = $"Request for doe at: {date}"};
            
            if (userId < 0)
            {
                 logDto.Error = "User id cannot be less than zero.";
                _logMessageBusClient.PublishNewLog(logDto);
                return BadRequest("Id has to be a positive number!");
            }

            if(date > DateTime.Now)
            {
                logDto.Error = "Cannot ask for doe from the future.";
                _logMessageBusClient.PublishNewLog(logDto);

                return BadRequest("Welcome to the future!");
            }

            Console.WriteLine("[GetDoeByDate] Query database...");
            var doe = await _repository.GetByDateAsync(userId, date);    // MongoDB Doe data retrieve
            
            if(doe is null)
            {
                logDto.Error = "Doe not found.";
                _logMessageBusClient.PublishNewLog(logDto);

                return NotFound();
            }

            if(doe.Does.Count == 0)
            {
                logDto.Error = "Doe contains no items.";
                _logMessageBusClient.PublishNewLog(logDto);

                return NotFound();
            }

            Console.WriteLine("[GetDoeByDate] Query successful!");

            var grpcRequestMealsModel = doe.ExtractGrpcRequestMealModel();
            var grpcRequestProductsModel = doe.ExtractGrpcRequestProductModel();
            
            Console.WriteLine("[GetDoeByDate] Grpc Server call: Products.");
            var grpcProductsResult = await _productsDataClient.ReturnAllProducts(grpcRequestProductsModel);   //Grpc Server call: Products

            Console.WriteLine("[GetDoeByDate] Grpc Server call: Meals.");
            var grpcMealsResult = await _mealsDataClient.ReturnAllMeals(grpcRequestMealsModel);   //Grpc Server call: Meals

            if(grpcProductsResult is null)
                Console.WriteLine("[GetDoeByDate] Grpc products response is null.");
            
            if(grpcMealsResult is null)
                Console.WriteLine("[GetDoeByDate] Grpc meals response is null.");

            
            var doeResponseDto = doe.AsDoeResponseDto(grpcProductsResult, grpcMealsResult);

            logDto.Message = $"Request for doe at: {date} succesfull.";
            _logMessageBusClient.PublishNewLog(logDto);

            return Ok(doeResponseDto);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{userId}/{date}")]
        public async Task<IActionResult> UpdateDoeAsync(int userId,  DateTime date, [FromBody] DoeUpdateRequestDto doeUpdateRequestDto)
        {
            Console.WriteLine($"[UpdateDoeAsync] User: {userId} requested to update or create his doe on date: {date}");
            var logDto = new LogRequestDto(){ Date = DateTime.Now, ServiceName = "MyDay", UserId = userId, Method = "UpdateDoeAsync", Message = $"Request to update doe at: {date}."};

            if(userId < 0)
            {
                logDto.Error = "User id cannot be less than zero.";
                _logMessageBusClient.PublishNewLog(logDto);

                return BadRequest("Id has to be a positive number!");
            }
    
            if(doeUpdateRequestDto is null)
            {
                logDto.Error = "Doe cannot be null.";
                _logMessageBusClient.PublishNewLog(logDto);

                return BadRequest("Doe cannot be null.");
            }

            var doe = _mapper.Map<Doe>(doeUpdateRequestDto);

            var result = await _repository.UpdateAsync(userId, date, doe);
            Console.WriteLine($"[UpdateMealAsync] Updated MC:{result.MatchedCount} MC:{result.ModifiedCount} UId:{result.UpsertedId}");

            if(result.UpsertedId is not null)
            {
                var grpcRequestMealsModel = doe.ExtractGrpcRequestMealModel();
                var grpcRequestProductsModel = doe.ExtractGrpcRequestProductModel();

                var existingDoe = await _repository.GetByDateAsync(userId, date);

                Console.WriteLine("[UpdateDoeAsync] Grpc Server call: Products.");
                var grpcProductsResult = await _productsDataClient.ReturnAllProducts(grpcRequestProductsModel);   //Grpc Server call: Products

                Console.WriteLine("[UpdateDoeAsync] Grpc Server call: Meals.");
                var grpcMealsResult = await _mealsDataClient.ReturnAllMeals(grpcRequestMealsModel);   //Grpc Server call: Meals
                
                var doeResponseDto = doe.AsDoeResponseDto(grpcProductsResult, grpcMealsResult);
                doeResponseDto.Id = Guid.Parse(result.UpsertedId.ToString());

                Console.WriteLine("[UpdateDoeAsync] Created response object.");

                logDto.Message = $"New doe created at date: {date}";

                _logMessageBusClient.PublishNewLog(logDto);
                return CreatedAtRoute(nameof(GetDoeByDateAsync), new { userId = result.UpsertedId.ToString(), date = date}, doeResponseDto);
            }

            
            logDto.Message = $"Doe updated succesfully.";
            _logMessageBusClient.PublishNewLog(logDto);
            return NoContent();
        }
    }
}