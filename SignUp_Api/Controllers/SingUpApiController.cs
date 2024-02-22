using SignUp_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignUp_Api.Data;
using System.Net;
using Bogus;
using System.Collections;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SignUp_Api.Controllers
{
    [Route("api/v1/SignUpApi")]
    [ApiController]
    public class SingUpApiController: ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ILogger<SingUpApiController> _logger;
        private readonly IConfiguration _configuration;
        public SingUpApiController(ILogger<SingUpApiController> logger,ApplicationDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            _configuration = configuration;
        }





        //[HttpGet("GetAll_SP")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public IActionResult GetUser([FromQuery] string cursor = "")
        //{
        //    /*pagination:
        //     * 1. Retrieve query parameters(cursor) from request url
        //     * 2. Is cursor empty? -> If Yes -> 1st page generate. 
        //     *                                  for that use sql query, returns List of users(Max 20)
        //     *                                  then calculate next page info:  1. start cursor 2. end cursor 3. has_next_page
        //     * 3. Is cursor empty? -> If No ->  2nd page start cursor = 1st page end cursor+1
        //     *                                  again query.
        //     *                                  calculate same 1. start cursor 2. end cursor 3. has_next_page
        //     */
        //    int limit = 20;
        //    int currentCursor = 0;
        //    if (string.IsNullOrEmpty(cursor))
        //    {
        //        var parameters = new[]
        //        {

        //            new SqlParameter("@cursor", currentCursor),
        //            new SqlParameter("@limit", limit)
        //        };

        //        var users = _db.SignUp.FromSqlRaw("EXEC SP_GetUserNoCursor @cursor, @limit", parameters).ToList();



        //        // start cursor = users[0].ID
        //        // len = Length(users)
        //        // end cursor = users[len-1].ID
        //        // hasNextPage = with query
        //        long StartCursor = users[0].Id;
        //        int len = users.Count;
        //        long EndCursor = users[len - 1].Id;
        //        var parameters2 = new[]
        //        {

        //            new SqlParameter("@EndCursor", EndCursor)
        //        };
        //        var NextPage = _db.SignUp.FromSqlRaw("EXEC SP_HasNextPage @EndCursor", parameters2).ToList();
        //        bool HasNextPage = Convert.ToBoolean(NextPage);

        //        var PageInfo2 = new
        //        {
        //            StartCursor = StartCursor,
        //            EndCursor = EndCursor,
        //            HasNextPage = HasNextPage,

        //        };
        //        var res = new
        //        {
        //            PageInfo2 = PageInfo2,
        //            users = users,
        //        };


        //        _logger.LogInformation("getting 20 users from 1st page!");
        //        return Ok(res);
        //    }

        //    else
        //    {
        //        currentCursor = int.Parse(cursor);
        //        var parameters = new[]
        //        {
        //            new SqlParameter("@cursor", currentCursor),
        //            new SqlParameter("@limit", limit)
        //        };

        //        var users = _db.SignUp.FromSqlRaw("EXEC SP_GetUserCursor @cursor, @limit", parameters).ToList();

        //        _logger.LogInformation("getting 20 users from cursored id!");
        //        return Ok(users);
        //    }


        //    var users2 = _db.SignUp.FromSqlRaw("SP_GetAllUser");

        //    _logger.LogInformation("getiing all users!");
        //    return Ok(users2);
        //}


        /// <summary>
        /// /////////
        [HttpGet("GetAll_SP")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetUser([FromQuery] string cursor = "")
        {
            /*pagination:
             * 1. Retrieve query parameters(cursor) from request url
             * 2. Is cursor empty? -> If Yes -> 1st page generate. 
             *                                  for that use sql query, returns List of users(Max 20)
             *                                  then calculate next page info:  1. start cursor 2. end cursor 3. has_next_page
             * 3. Is cursor empty? -> If No ->  2nd page start cursor = 1st page end cursor+1
             *                                  again query.
             *                                  calculate same 1. start cursor 2. end cursor 3. has_next_page
             */
            int currentCursor =0;
            int limit = 20;
            var users = new List<SignUp>();

            if (int.TryParse(cursor, out int givenCursor) && givenCursor > 0)
            {
                currentCursor = givenCursor;
                var parameters = new[]
                {

                    new SqlParameter("@cursor", currentCursor),
                    new SqlParameter("@limit", limit)
                };

                users = _db.SignUp.FromSqlRaw("EXEC SP_GetUserCursor @cursor, @limit", parameters).ToList();
            }

            else
            {
                currentCursor = givenCursor;
                var parameters = new[]
                {

                    new SqlParameter("@cursor", currentCursor),
                    new SqlParameter("@limit", limit)
                };

                users = _db.SignUp.FromSqlRaw("EXEC SP_GetUserNoCursor @cursor, @limit", parameters).ToList();

            }


            
            
            ////if (string.IsNullOrEmpty(cursor))
            //{
                

                string connectionString = _configuration.GetConnectionString("DefaultConnectionStrings");
                

                // start cursor = users[0].ID
                // len = Length(users)
                // end cursor = users[len-1].ID
                // hasNextPage = with query
                long StartCursor = users[0].Id;
                int len = users.Count;
                long EndCursor = users[len - 1].Id;
                bool hasNextPage = false;
                if (users.Any())
                {
                    long lastUserId = users.Last().Id; // Assuming the last user's ID is needed for SP_HasNextPage
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("SP_HasNextPage", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add(new SqlParameter("@EndCursor", SqlDbType.BigInt) { Value = lastUserId });

                            // Assuming SP_HasNextPage returns a bit (0 or 1) indicating if there is a next page
                            hasNextPage = (bool)command.ExecuteScalar();
                        }
                    }
                }

                var pageInfo = new
                {
                    StartCursor = users.Any() ? users.First().Id : 0,
                    EndCursor = users.Any() ? users.Last().Id : 0,
                    HasNextPage = hasNextPage
                };

                var response = new
                {
                    PageInfo = pageInfo,
                    Users = users
                };

                return Ok(response);
            //}


            var users2 = _db.SignUp.FromSqlRaw("SP_GetAllUser");

            _logger.LogInformation("getiing all users!");
            return Ok(users2);
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
