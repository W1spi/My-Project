using Data.NLimit.Common.DataContext.MongoDb;
using Data.NLimit.Common.EntitiesModels.MongoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NLimit.MongoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly MongoDbContext _dbContext;
        public BookController()
        {
            _dbContext = new MongoDbContext("mongodb://127.0.0.1:27017", "local");
        }

        [HttpPost("create-book")]
        [ProducesResponseType(200)]
        public IActionResult AddBook(Book book)
        {
            _dbContext.Books.InsertOne(book);
            return Ok();
        }
    }
}
