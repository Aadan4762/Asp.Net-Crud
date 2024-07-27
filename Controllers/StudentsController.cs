using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Add this attribute to require authentication
public class StudentsController : Controller
{
    
    private readonly ApplicationDbContext dbContext;
    
    public StudentsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }


        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var allStudents = dbContext.Students.ToList();
            return Ok(allStudents);
        }

        // Get students by ID
        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetStudentById(Guid id)
        {
            var student = dbContext.Students.Find(id);
            if (student == null) 
            {
                return NotFound();
            }
            return Ok(student);
        }

        // Add a new student
        [HttpPost]
        public IActionResult AddStudent(AddStudentDto addStudentDto)
        {
            var studentEntity = new Student()
            {
                Name = addStudentDto.Name,
                School = addStudentDto.School,
                Grade = addStudentDto.Grade,
            };

            dbContext.Students.Add(studentEntity);
            dbContext.SaveChanges();
            return Ok(studentEntity);
        }

        // Update an existing student
        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateStudent(Guid id, UpdateStudentDto updateStudentDto)
        {
            var student = dbContext.Students.Find(id);

            if (student == null) 
            {
                return NotFound();
            }
            student.Name = updateStudentDto.Name;
            student.School = updateStudentDto.School;
            student.Grade = updateStudentDto.Grade;
            dbContext.SaveChanges();
            return Ok(student);
        }

        // Delete an student
        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteStudent(Guid id)
        {
            var student = dbContext.Students.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            dbContext.Students.Remove(student);
            dbContext.SaveChanges();
            return Ok();
        }
    }
