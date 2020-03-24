using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
       
        // CW 4 ZADANIE 4.2
        [HttpGet]
        public IActionResult GetStudent(string orderBy)
        {
             List<Student> _students = new List<Student>();
            using(var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18291;Integrated Security=True"))
            using(var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "select * from student";

                client.Open();
                var dr = com.ExecuteReader();
                int idStudent = 1;
                while (dr.Read())
                {
                    var st = new Student();
                    st.IdStudent = idStudent++;
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    _students.Add(st);
                }
            }   
            return Ok(_students);
        }
        // CW 4 ZADANIE 4.3-4.5
        [HttpGet("{IndexNumber}")]
        public IActionResult GetStudents(string IndexNumber)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18291;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = $"select Student.IndexNumber,FirstName, LastName, Enrollment.IdEnrollment, " +
                    $"Semester, IdStudy  from Enrollment INNER JOIN Student ON Student.IdEnrollment = Enrollment.IdEnrollment WHERE Student.IndexNumber =  @IndexNumber ";
                com.Parameters.AddWithValue("IndexNumber", IndexNumber);
                client.Open();
                var dr = com.ExecuteReader();
                String result = "";
                while (dr.Read())
                {
                    result += dr["IndexNumber"].ToString() + " " + dr["FirstName"].ToString() + " " + dr["LastName"].ToString() + " " + dr["IdEnrollment"].ToString() +
                        "semester " + dr["Semester"].ToString() + " " + dr["IdStudy"];
                }

                return Ok(result);
            }
        }
        //ZADANIE 8
        private readonly IDbsService _dbService;

        public StudentsController(IDbsService dbService)
        {
            _dbService = dbService;
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