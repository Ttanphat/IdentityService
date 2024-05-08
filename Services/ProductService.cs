using App.Models;

namespace App.Services
{
    public class ProductService : List<ProductModel>
    {
        public ProductService()
        {
            this.AddRange(new ProductModel[] {
                new ProductModel() {Id = 1, Name = "Iphone X", Price = 10000},
                new ProductModel() {Id = 2, Name = "Samsung", Price = 8000},
                new ProductModel() {Id = 3, Name = "Nokia", Price = 3000},
                new ProductModel() {Id = 4, Name = "Iphone 14", Price = 20000},
            });
        }
    }
}