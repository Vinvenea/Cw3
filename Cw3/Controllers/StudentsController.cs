using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.DTOs.Request;
using Cw3.Hashing;
using Cw3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw3.Controllers
{
    [ApiController]
    [Route("api/students")] // atrybuty
    public class StudentsController : ControllerBase
    {
        private readonly s18291Context _context;
        public IConfiguration Configuration { get; set; }
        public StudentsController(s18291Context context, IConfiguration configuration)
        {
            Configuration = configuration;
            _context = context;
        }
        [HttpGet]

        public IActionResult GetStudent(string orderBy)
        {
           
            var students = _context.Student.Select(student => new
            {
                IndexNumber = student.IndexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName
            });

            return Ok(students);

        }
        [HttpPut()]
        public IActionResult PutStudent(ChangeStudentRequest studento)
        {
            var student = _context.Student.Where(stu => stu.IndexNumber == studento.IndexNumber);

            Student studenta = new Student
            {
                IndexNumber = studento.IndexNumber,
                FirstName = studento.FirstName,
                LastName = studento.LastName
            };
            _context.Attach(studenta);
            _context.Entry(studenta).Property("FirstName").IsModified = true;
            _context.Entry(studenta).Property("LastName").IsModified = true;

            _context.SaveChanges();

            return Ok("Aktualizacja dokonczona");
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(String id)
        {
            var studentop = _context.Student.Where(stu => stu.IndexNumber == id);


            _context.Remove(studentop);


            _context.SaveChanges();

            return Ok("Usuwanie ukonczone");
        }
        [HttpPost]
        public IActionResult Login(String IndexNumber, String password)
        {
            LoginRequestDTO login = new LoginRequestDTO(IndexNumber, password);
            Console.WriteLine(IndexNumber + " " + password);
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18291;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                client.Open();

                com.CommandText = "select * from student WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("index", IndexNumber);

                var dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    return Unauthorized("Zle dane");
                    dr.Close();

                }
                string Password = (string)dr["Password"];
                string salt = (string)dr["salt"];

                //Console.WriteLine(salt);
                //Console.WriteLine(Password);

                //Console.WriteLine(MyHashing.Create(password, salt));


                dr.Close();
                if (MyHashing.Create(password, salt).Equals(Password))
                {


                    var claims = new[]
                  {
                new Claim(ClaimTypes.Name, IndexNumber),
                new Claim(ClaimTypes.Role, "student")
            };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token2 = new JwtSecurityToken
                    (
                        issuer: "Admin",
                        audience: "student",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                    );
                    var token = new JwtSecurityTokenHandler().WriteToken(token2);
                    var refreshtoken = Guid.NewGuid();

                    com.CommandText = "UPDATE Student SET RefreshToken = @refreshToken WHERE IndexNumber = @index";
                    com.Parameters.AddWithValue("index", IndexNumber);
                    com.Parameters.AddWithValue("refreshToken", refreshtoken.ToString());
                    dr = com.ExecuteReader();
                    Console.WriteLine(refreshtoken.ToString());
                    return Ok(new
                    {
                        token2,
                        refreshtoken
                    });
                }
                else
                {
                    return Unauthorized("Zle dane");
                }
            }
            }
              
        
        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refreshToken)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18291;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                client.Open();

                com.CommandText = "select IndexNumber, Password from student WHERE refreshtoken = @refreshToken";
                com.Parameters.AddWithValue("refreshToken", refreshToken);
                var dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    return Unauthorized("Sesja wygasla");
                }

                string IndexNumber = (String)dr["IndexNumber"];
                string Password = (String)dr["Password"];

                return Ok(Login(IndexNumber, Password));

            }
                
        }
        // CW 4 ZADANIE 4.2
        
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
      
      

        //ZADANIE 7
       
        //[HttpPut()]
       public IActionResult HashAll(string IndexNumber)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18291;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = "SELECT * FROM STUDENT WHERE IndexNumber = @Index";
                com.Parameters.AddWithValue("Index", IndexNumber);
               client.Open();
                var dr = com.ExecuteReader();
                dr.Read();


                string Password = (string)dr["Password"];

                var salt = MyHashing.CreateSalt();
                string hashedPass = MyHashing.Create(Password, salt);

                dr.Close();
                
                com.CommandText = "UPDATE Student SET Password = @hashed WHERE IndexNumber = @Indexe";
                com.Parameters.AddWithValue("hashed", hashedPass);
                com.Parameters.AddWithValue("Indexe", IndexNumber);
                dr = com.ExecuteReader();
                dr.Close();
                
                com.CommandText = "UPDATE Student SET salt = @salt WHERE IndexNumber = @Indexs";
                com.Parameters.AddWithValue("salt", salt);
                com.Parameters.AddWithValue("Indexs", IndexNumber);
                dr = com.ExecuteReader();
                dr.Close();

            }

            return Ok();
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