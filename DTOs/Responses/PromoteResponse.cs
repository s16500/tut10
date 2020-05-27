using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial10_APBD.Entities;
using Tutorial10_APBD.Models;

namespace Tutorial_3._1.DTOs
{
    public class PromoteResponse
    {

        private Enrollment enrollment;

        public string Studies { get; set; }

        public int Semester { get; set; }


        public PromoteResponse(Enrollment enrollment)
        {


            Studies = enrollment.StartDate.ToString();

            Semester = enrollment.Semester;
        }
    }
}
