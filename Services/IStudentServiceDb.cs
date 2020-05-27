using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial_3._1.DTOs;
using Tutorial_3._1.DTOs.Requests;
using Tutorial_3._1.DTOs.Responses;
using Tutorial10_APBD.Entities;

namespace Tutorial_3._1.Services
{
    public interface IStudentServiceDb
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentsRequest req);
        PromoteResponse PromoteStudent(int semester, string studies);
        Student GetStudent(string indexNumber);
        public void SaveLogData(string data);


    }
}
