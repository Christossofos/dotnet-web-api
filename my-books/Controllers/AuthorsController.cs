using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using my_books.ActionResults;
using my_books.Data.Services;
using my_books.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private AuthorsService _authorsService;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(AuthorsService authorsService, ILogger<AuthorsController> logger)
        {
            _authorsService = authorsService;
            _logger = logger;
        }

        [HttpGet("get-all-authors")]
        public IActionResult GetAllAuthors(string orderBy, string searchString, int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("This is a log in GetAllAuthors().");
                var _result = _authorsService.GetAllAuthors(orderBy, searchString, pageNumber, pageSize);
                if (searchString != null && !_result.Any())
                {
                    return NotFound();
                }
                return Ok(_result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-author")]
        public IActionResult AddAuthor([FromBody]AuthorVM author)
        {
            _authorsService.AddAuthor(author);
            return Ok();
        }

        [HttpGet("get-author-with-books-by-id/{id}")]
        public IActionResult GetAuthorWithBooks(int id)
        {
            var _response = _authorsService.GetAuthorWithBooks(id);
            return Ok(_response);
        }

        /* Custom ActionResult expirements */
        [HttpGet("get-author-by-id/{id}")]
        public CustomActionResult GetAuthorById(int id)
        {
            var _response = _authorsService.GetAuthorById(id);
            if (_response != null)
            {
                var _responseObject = new CustomActionResultVM()
                {
                    Data = _response
                };

                return new CustomActionResult(_responseObject);
            }
            else
            {
                var _responseObject = new CustomActionResultVM()
                {
                    Exception = new Exception("This is coming from authors controller.")
                };
                return new CustomActionResult(_responseObject);
            }
        }
    }
}
