using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tutorial_3._1.DTOs.Requests;
using Tutorial_3._1.DTOs.Responses;
using Tutorial_3._1.Services;

namespace Tutorial_3._1.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        readonly IStudentServiceDb _service;
        //Constructor injection (SOLID - D - Dependency Injection)
        public EnrollmentsController(IStudentServiceDb service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentsRequest request)
        {
            // Check if data was delivered corectly
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not delivered");
            }

            var enrollment = _service.EnrollStudent(request);


            if (enrollment != null)
            {
                return Ok(enrollment);

            }
            else

            {
                return BadRequest("bad request");
            }


            
        }

        [HttpPost("promote")]
        public IActionResult PromoteStudents(PromoteRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Data not delivered");
            }

            var result = _service.PromoteStudent(request.Semester, request.Studies);


            if (result != null)
            {
                return Ok(result);

            }
            else

            {
                return BadRequest("bad request");
            }
        }
    }
}