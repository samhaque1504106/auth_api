using SignUp_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignUp_Api.Data;
using System.Net;
using Bogus;

namespace SignUp_Api.Controllers
{
    [Route("api/v1/SignUpApi")]
    [ApiController]
    public class SingUpApiController: ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ILogger<SingUpApiController> _logger;
        public SingUpApiController(ILogger<SingUpApiController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
       


        

        [HttpGet("GetAll_SP")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SignUp>> GetUser()
        {
            var users = _db.SignUp.FromSqlRaw("SP_GetAllUser");

            //System.Data.DataTable dataTableDetails = _db.CmnUser;
            _logger.LogInformation("getiing all users!");
            return Ok(users);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<SignUp> CreateUser([FromBody]SignUp user)
        {
            if(_db.SignUp.FirstOrDefault( u => u.UserName.ToLower() == user.UserName.ToLower() ) != null) 
            {
                ModelState.AddModelError("CustomError", "UserName already Exists,try using combinations with number and signs!");
                return BadRequest(ModelState);
            }
            else if(user == null)
            {
                return BadRequest(user);
            }
            else if (user.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            _db.SignUp.Add(user);
            _db.SaveChanges();
            _logger.LogInformation("new sign up username: "+user.UserName+" is done!");
            return Ok("Sign-up successful, Welcome!");


        }


    }
}
