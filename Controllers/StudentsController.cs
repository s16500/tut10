using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tutorial10_APBD.Entities;
using Tutorial10_APBD.Models;

namespace Tutorial10_APBD.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentContext _studentContext;

        public StudentsController(StudentContext studentContext)
        {
            _studentContext = studentContext;
        }

        [HttpGet]

        public IActionResult GetStudents()
        {
            var students = _studentContext.Student
                                            .Include(s => s.IdEnrollmentNavigation)
                                            .ThenInclude(s => s.IdStudyNavigation)
                                            .Select(st => new GetStudentsResponse
                                            {
                                                IndexNumber = st.IndexNumber,
                                                FirstName = st.FirstName,
                                                LastName = st.LastName, 
                                                BirthDate = st.BirthDate.ToShortDateString(),
                                                Semester = st.IdEnrollmentNavigation.Semester,
                                                Studies = st.IdEnrollmentNavigation.IdStudyNavigation.Name,

                                            }).ToList();
            return Ok(students);
        }


        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {

            // Check if data was delivered corectly
            if (!ModelState.IsValid)
            {
                return BadRequest("Data not delivered");
            }


            if (student != null)
            {
                _studentContext.Add<Student>(student);
                _studentContext.SaveChanges();

                return Ok(student);

            } else

            {
                return BadRequest("bad request");
            }

            
        }

        [HttpDelete("deleteStudent{id}")]
        public IActionResult GetStudents(string id)
        {
            var index = id;
            var student = (from c in _studentContext.Student
                           where c.IndexNumber == id
                           select c).First();

            if (student != null)
            {
                _studentContext.Student.Remove(student);
                _studentContext.SaveChanges();
                return Ok("Succesfully deleted");
            }
            else
            {
                return BadRequest("Succesfully deleted");

            }

        }

        [HttpPut("updateStudent")]
        public IActionResult UpdateStudent(Student s)
        {
            var student = (from c in
                               _studentContext.Student
                           where c.IndexNumber == s.IndexNumber
                           select c).First();

            if (student != null)
            {
                _studentContext.Update<Student>(s);
                _studentContext.SaveChanges();
                return Ok("Succesuflly Updated");
            }
            else
            {
                return BadRequest("Failed to Update");

            }
        }
    }
}