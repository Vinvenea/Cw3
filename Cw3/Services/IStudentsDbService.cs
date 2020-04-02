using Cw3.DTOs.Request;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentsDbService
    {
       public Enrollment EnrollStudent(EnrollStudentRequest request);
       public Enrollment promoteStudent(Promotion promotion);
    }
}
