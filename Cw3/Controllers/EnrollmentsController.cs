using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DTOs.Request;
using Cw3.DTOs.Response;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentsDbService _serivce;
        private readonly s18291Context _context;

        public EnrollmentsController(IStudentsDbService serivce)
        {
            _serivce = serivce;
        }
        //ZADANIE 1-------------

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            Enrollment n = _serivce.EnrollStudent(request);
            if (n.Equals(null))
            {
                return BadRequest(500);
            }
            return Created("201", n);


        }
        [HttpPost("{promotions}")]
        [Authorize(Roles = "employee")]
        public IActionResult promoteStudent(Promotion promotion)
        {
            Enrollment n = _serivce.promoteStudent(promotion);
            if (n == null)
            {
                return NotFound(404);
            }
            return Created("201", n);
        }

    }
}
//jwt token