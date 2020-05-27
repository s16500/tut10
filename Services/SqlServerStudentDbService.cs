using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tutorial_3._1.DTOs;
using Tutorial_3._1.DTOs.Requests;
using Tutorial_3._1.DTOs.Responses;
using Tutorial10_APBD.Entities;

namespace Tutorial_3._1.Services
{
    public class SqlServerStudentDbService : IStudentServiceDb
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentsRequest request)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
                
            {
                using (var com = new SqlCommand())
                {
                    con.Open();
                    com.Connection = con;
                    var tran = con.BeginTransaction();
                    com.Transaction = tran;

                    // Check if studies exists
                    com.CommandText = "select * FROM Studies WHERE Name=@Name";
                    com.Parameters.AddWithValue("Name", request.Studies);

                    var reader = com.ExecuteReader();
                    if (!reader.Read())
                    {
                        return null;
                    }
                    int idStudies = (int)reader["IdStudy"];

                    reader.Close();

                    // check if enrollment with semster 1 exists
                    com.CommandText = "select * FROM Enrollment WHERE Semester=1 AND IdStudy=@IdStudy";
                    com.Parameters.AddWithValue("IdStudy", idStudies);

                    var idEnrollment = 0;
                    var reader2 = com.ExecuteReader();
                    if (!reader2.Read())
                    {
                        // add new enrollment with semster 1 
                        com.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment = (SELECT MAX(IdEnrollment) FROM Enrollment)";
                        reader2.Close();

                        var reader3 = com.ExecuteReader();
                        reader3.Read();
                        idEnrollment = reader3.GetInt32(0);
                        reader3.Close();
                        com.CommandText = "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES(@IdEnrollment, @Semester, @ids, @StartDate);";
                        com.Parameters.AddWithValue("IdEnrollment", idEnrollment + 1);
                        com.Parameters.AddWithValue("Semester", 1);
                        com.Parameters.AddWithValue("ids", idStudies);
                        com.Parameters.AddWithValue("StartDate", DateTime.Now.ToString());

                        var reader4 = com.ExecuteReader();
                        reader4.Close();
                    }
                    else
                    {
                        idEnrollment = (int)reader2["IdEnrollment"];
                        reader2.Close();
                    }


                    // check if student with index number exists
                    com.CommandText = "select * FROM Student WHERE IndexNumber=@IndexNumber";
                    com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);

                    var reader5 = com.ExecuteReader();
                    if (!reader5.Read())
                    {
                        reader5.Close();
                        // create new student
                        com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, Birthdate, IdEnrollment) VALUES (@IndexNumber2, @FirstName2, @LastName2, @Birthdate2, @IdEnrollment2)";
                        com.Parameters.AddWithValue("IndexNumber2", request.IndexNumber);
                        com.Parameters.AddWithValue("FirstName2", request.FirstName);
                        com.Parameters.AddWithValue("LastName2", request.LastName);
                        com.Parameters.AddWithValue("Birthdate2", request.DateOfBirth.ToString());
                        com.Parameters.AddWithValue("IdEnrollment2", idEnrollment);

                        var reader6 = com.ExecuteReader();
                        reader6.Close();
                    }
                    else
                    {
                        reader5.Close();
                    }


                    tran.Commit();
                }

            }

            var response = new EnrollStudentResponse
            {
                LastName = request.LastName,
                Semester = 1
            };

            return response;
        }

        public PromoteResponse PromoteStudent(int semester, string studies)
        {
            using (var client = new SqlConnection(@"Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            using (var com = new SqlCommand())

            {

                client.Open();
                com.Connection = client;


                com.CommandText = "SELECT * FROM Enrollment, Studies WHERE Enrollment.IdStudy=Studies.IdStudy AND Enrollment.semester=@semester AND Studies.Name=@Studies";
                com.Parameters.AddWithValue("semester", semester);
                com.Parameters.AddWithValue("Studies", studies);

                var dr = com.ExecuteReader();

                if (!dr.Read())
                {
                    return null;
                }

                dr.Close();

                com.CommandType = System.Data.CommandType.StoredProcedure;

                com.CommandText = "Promotion";
                com.ExecuteNonQuery();


                com.CommandType = System.Data.CommandType.Text;


                com.CommandText = "select * from Enrollment,Studies where Enrollment.IdStudy=Studies.IdStudy and Name=@Studies and Semester=@semestern";

                com.Parameters.AddWithValue("semestern", semester + 1);

                var dr2 = com.ExecuteReader();


                dr2.Read();

                var enrollment = new Enrollment();

                enrollment.IdStudy = (int)dr2["IdStudy"];
                enrollment.Semester = (int)dr2["Semester"];
                var StartDate = dr2["StartDate"];
                enrollment.StartDate = DateTime.Now;

                var promotion = new PromoteResponse(enrollment);

                return promotion;
       
            }
        }

        public Student GetStudent(string indexNumber)
        {
            using (var con = new SqlConnection(@"Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True"))
            {
                using (var com = new SqlCommand())
                {
                    con.Open();
                    com.Connection = con;
                    var tran = con.BeginTransaction();
                    com.Transaction = tran;

                    // Check if studies exists
                    com.CommandText = @"select *
                                            from Student s
                                            WHERE s.IndexNumber = @indexNumber;";
                    com.Parameters.AddWithValue("indexNumber", indexNumber);
                    var reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        var st = new Student
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                        };

                        return st;
                      
                    }

                    reader.Close();


                    tran.Commit();
                }

            }

            return null;
        }

        PromoteResponse IStudentServiceDb.PromoteStudent(int semester, string studies)
        {
            throw new NotImplementedException();
        }

        public void SaveLogData(string data)
        {
            throw new NotImplementedException();
        }
    }
}
