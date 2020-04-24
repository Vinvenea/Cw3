using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DAL
{
    public class MockDbService : IDbsService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IdStudent=1, FirstName="Jan", LastName="Kowalski" },
                new Student{IdStudent=2, FirstName="Anna", LastName="Malewski" },
                new Student{IdStudent=3, FirstName="Andrzej", LastName="Andrzejewicz" }
            };

        }
        public IEnumerable<Student> GetStudents()
        {
            _students = new List<Student>();
            return _students;
        }

        public bool CheckIndex(string index)
        {
            _students = new List<Student>();
            return false ;
        }

       
    }
}
