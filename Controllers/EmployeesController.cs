using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Authorization; // Add this namespace
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Add this attribute to require authentication
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        // Constructor for dependency injection
        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Get all employees
        [HttpGet]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN},{StaticUserRoles.OWNER}")]
        public IActionResult GetAllEmployees()
        {
            var allEmployees = dbContext.Employees.ToList();
            return Ok(allEmployees);
        }

        // Get employee by ID
        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN},{StaticUserRoles.OWNER},{StaticUserRoles.USER}")]
        public IActionResult GetEmployeeById(Guid id)
        {
            var employee = dbContext.Employees.Find(id);
            if (employee == null) 
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // Add a new employee
        [HttpPost]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN},{StaticUserRoles.OWNER},{StaticUserRoles.USER}")]
        public IActionResult AddEmployee(AddEmployeeDto addEmployeeDto)
        {
            var employeeEntity = new Employee()
            {
                Name = addEmployeeDto.Name,
                Email = addEmployeeDto.Email,
                Phone = addEmployeeDto.Phone,
                Salary = addEmployeeDto.Salary
            };

            dbContext.Employees.Add(employeeEntity);
            dbContext.SaveChanges();
            return Ok(employeeEntity);
        }

        // Update an existing employee
        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN}")]
        public IActionResult UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = dbContext.Employees.Find(id);

            if (employee == null) 
            {
                return NotFound();
            }
            employee.Name = updateEmployeeDto.Name;
            employee.Email = updateEmployeeDto.Email;
            employee.Phone = updateEmployeeDto.Phone;
            employee.Salary = updateEmployeeDto.Salary;
            dbContext.SaveChanges();
            return Ok(employee);
        }

        // Delete an employee
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = $"{StaticUserRoles.ADMIN},{StaticUserRoles.OWNER},{StaticUserRoles.USER}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var employee = dbContext.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            dbContext.Employees.Remove(employee);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
