using Cw3.DTOs.Request;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SqlServerStudentDbService : IStudentsDbService
    {
        public Enrollment EnrollStudent(EnrollStudentRequest request)
        {
            var st = new Enrollment();
            st.Semester = 1;
            st.StartDate = DateTime.Today.ToShortDateString();

            using (var con = new SqlConnection("Data Source = db-mssql; Initial Catalog = s18291; Integrated Security = True"))
            using (var com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();

                var tran = con.BeginTransaction();
                com.Transaction = tran;
                try
                {
                    //1 Czy studia istnieja? 
                    com.CommandText = "select IdStudy from studies where name = @name";
                    com.Parameters.AddWithValue("name", request.Studies);
                    var dr = com.ExecuteReader();


                    if (!dr.Read())
                    {
                        tran.Rollback();
                        //return BadRequest("Studia nie istnieja");
                        throw new Exception();
                        //Zglloszenie bledu
                    }

                    int idstudies = (int)dr["IdStudy"];
                    dr.Close();
                    st.IdStudy = idstudies;

                    com.CommandText = "Select IdEnrollment From Enrollment WHERE IdStudy = @IdStudy AND SEMESTER = 1 AND StartDAte = (SELECT MAX(StartDate) FROM Enrollment WHERE @IdStudy = @IdStudy)";
                    com.Parameters.AddWithValue("IdStudy", idstudies);
                    dr = com.ExecuteReader();



                    if (!dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "Select IndexNumber From Student Where IndexNumber = @index";
                        com.Parameters.AddWithValue("index", request.IndexNumber);
                        dr = com.ExecuteReader();
                        if (dr.Read())
                        {
                            throw new Exception("NIEUNIKALNY STUDENT");

                        }
                        dr.Close();


                        com.CommandText = "SELECT MAX(IdEnrollment) as 'Maximum'  from Enrollment";
                        dr = com.ExecuteReader();
                        dr.Read();

                        var nextValue = (int)dr["Maximum"] + 1;
                        st.IdEnrollment = nextValue;

                        dr.Close();
                        com.CommandText = "Insert into Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@next, 1, @IdStudy, GETDATE())";
                        com.Parameters.AddWithValue("next", nextValue);
                        //com.Parameters.AddWithValue("IdStudy", idstudies);
                        Console.WriteLine(nextValue);
                        com.ExecuteNonQuery();


                        Console.WriteLine("CO?");
                        //x dodanie studenta

                        com.CommandText = "Insert Into Student(IndexNumber, FirstName, LastName, BirthDate,IdEnrollment) VALUES (@Index, @FName, @LastName, @BirthDate,@nexte)";

                        com.Parameters.AddWithValue("Index", request.IndexNumber);
                        com.Parameters.AddWithValue("Fname", request.FirstName);
                        com.Parameters.AddWithValue("LastName", request.LastName);
                        com.Parameters.AddWithValue("BirthDate", request.Birthdate);
                        com.Parameters.AddWithValue("nexte", nextValue);



                        com.ExecuteNonQuery();
                        tran.Commit();
                    }
                    else
                    {
                        var nexte = (int)dr["IdEnrollment"];
                        st.IdEnrollment = nexte;
                        dr.Close();
                        com.CommandText = "Insert Into Student(IndexNumber, FirstName, LastName, BirthDate,IdEnrollment) VALUES (@Index, @FName, @LastName, @BirthDate,@nexte)";

                        com.Parameters.AddWithValue("Index", request.IndexNumber);
                        com.Parameters.AddWithValue("Fname", request.FirstName);
                        com.Parameters.AddWithValue("LastName", request.LastName);
                        com.Parameters.AddWithValue("BirthDate", request.Birthdate); // CZY ENROLLMENTS TEZ?
                        com.Parameters.AddWithValue("nexte", nexte);




                        com.ExecuteNonQuery();
                        tran.Commit();
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    tran.Rollback();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                   // return BadRequest(400);

                }



                return  (st);
            }
        }

        public bool ifExists(string index)
        {
            using (var con = new SqlConnection("Data Source = db-mssql; Initial Catalog = s18291; Integrated Security = True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "Select IndexNumber From Student Where IndexNumber = @index";
                com.Parameters.AddWithValue("index", index);
                var dr = com.ExecuteReader();
               
                if (dr.Read())
                {
                    return true;

                }

                return false;
            }
        }

        public Enrollment promoteStudent(Promotion promotion)
        {
            using (var con = new SqlConnection("Data Source = db-mssql; Initial Catalog = s18291; Integrated Security = True"))
            using (var com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();

                var tran = con.BeginTransaction();
                com.Transaction = tran;
                try
                {
                    com.CommandText = "select IdStudy from studies where name = @name";
                    com.Parameters.AddWithValue("name", promotion.Studies);
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        tran.Rollback();
                        //return BadRequest("Studia nie istnieja");
                        throw new Exception();
                    }
                    else
                    {

                        int idstudies = (int)dr["IdStudy"];
                        dr.Close();
                        com.CommandText = "SELECT IdEnrollment from Enrollment WHERE IdStudy = @IdStudy AND Semester = @semester";

                        com.Parameters.AddWithValue("IdStudy", idstudies);
                        com.Parameters.AddWithValue("semester", promotion.Semester);
                        dr = com.ExecuteReader();

                        if (dr.Read())
                        {

                            dr.Close();
                            com.CommandText = "exec PromoteStudents @k1, @ol";
                            com.Parameters.AddWithValue("k1", promotion.Studies);
                            com.Parameters.AddWithValue("ol", promotion.Semester);
                            com.ExecuteNonQuery();
                            dr.Close();
                            com.CommandText = "SELECT * FROM Enrollment WHERE IdStudy = @IdStudy AND Semester = @sem";
                            com.Parameters.AddWithValue("IdStudy", idstudies);
                            com.Parameters.AddWithValue("sem", promotion.Semester + 1);
                            dr = com.ExecuteReader();
                            Enrollment result = new Enrollment();
                            result.IdEnrollment = (int)dr["IdEnrollment"];
                            result.Semester = (int)dr["Semester"];
                            result.IdStudy = (int)dr["IdStudy"];
                            result.StartDate = (string)dr["StartDate"];
                            dr.Close();
                            tran.Commit();
                            return result;




                        }
                        else
                        {
                            return null;
                        }

                    }
                   

                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    tran.Rollback();
                }
            }
            return null;
        }

        
    }
}
