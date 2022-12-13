using ProductsCatalog.Dtos.Response;
using ProductsCatalog.Dtos.Request;
using ProductsCatalog.Dtos.RabbitMQ;
using ProductsCatalog.Entities;
using System;

namespace ProductsCatalog
{
    public static class Extensions
    {
        public static ProductReadResponseDto AsReadDto(this Product product)
        {
            return new ProductReadResponseDto() 
            {
                Id = product.Id,
                Name = product.Name,
                Manufacturer = product.Manufacturer,
                Kcal = product.Kcal,
                Protein = product.Protein,
                Fat = product.Fat,
                Carbohydrates = product.Carbohydrates,
                Roughage = product.Roughage
            };
        }

        public static ProductPublishedDto AsPublishDto(this Product product)
        {
            return new ProductPublishedDto() 
            {
                Id = product.Id,
                Name = product.Name,
                Manufacturer = product.Manufacturer,
                Kcal = product.Kcal,
                Protein = product.Protein,
                Fat = product.Fat,
                Carbohydrates = product.Carbohydrates,
                Roughage = product.Roughage
            };
        }

        public static ProductModel AsGprcProductModel(this Product product)
        {
            if(product is not null)
            {
                return new ProductModel() 
                {
                    Id = product.Id,
                    Name = product.Name,
                    Manufacturer = product.Manufacturer ?? "N/A",
                    Kcal = product.Kcal,
                    Protein = product.Protein,
                    Fat = product.Fat,
                    Carbohydrates = product.Carbohydrates,
                    Roughage = product.Roughage
                };
            }
            else
            {
                Console.WriteLine("[AsGprcProductModel] Product is null!");

                return null;
            }
        }
    }
}