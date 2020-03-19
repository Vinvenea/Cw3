using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")] // atrybuty
    public class StudentsController : ControllerBase
    {
        //ZADANIE 8
        private readonly IDbsService _dbService;

        public StudentsController(IDbsService dbService)
        {
            _dbService = dbService;
        }
        [HttpGet]
        public IActionResult GetStudent(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }
      
      
        //ZADANIE 7
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, Student student)
        {

            return Ok("Aktualizacja dokonczona");
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id, Student student)
        {
            return Ok("Usuwanie ukonczone");
        }
        //-------------------------------------------------
        //ZADANIE 5
        //[HttpGet]
        //public string GetStudents(string id)//[FromBody] Student student do HttpPost // atrybut do upewnienia sie ze bedzie brane z Body
        //{
        //    return $"Chce info o danym studencie o id:{id}";
        //}
        //ZADANIE 4
        //[HttpGet("{id}")]
        //public IActionResult GetStudents(int id)
        //{
        //    if(id == 1)
        //    {
        //        return Ok("Kowalski");
        //    }else if(id == 2)
        //    {
        //        return Ok("Malewski");
        //    }

        //    return NotFound("Nie znaleziono studenta");
        //}
  // przetwarzanie zadan endpoints
        //  [HttpPost] // w poscie mozna cialo zadania
        //[HttpGet("{id}")]
        //public string GetStudents(string id)//[FromBody] Student student do HttpPost // atrybut do upewnienia sie ze bedzie brane z Body
        //{
        //    return $"Chce info o danym studencie o id:{id}";
        //}
  //[HttpPost]
  //      public IActionResult CreateStudent(Student student)
  //      {
  //          student.IndexNumber = $"s{new Random().Next(1, 20000)}";
  //          return Ok(student);
  //      }
    }

}