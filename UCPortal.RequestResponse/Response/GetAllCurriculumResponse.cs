using System;
using System.Collections.Generic;
using System.Text;

namespace UCPortal.RequestResponse.Response
{
    public class GetAllCurriculumResponse
    {
        public List<SchoolYear> year { get; set; }
        public List<Courses> course { get; set; }
        public List<Subjects> subjects { get; set; }
        public List<Departments> departments { get; set; }
        public class SchoolYear 
        {
            public int year { get; set; }
            public int isDeployed { get; set; }
        }
        public class Courses
        { 
            public string course_code { get; set; }
            public string course_description { get; set; }
            public string course_abbr { get; set; }
            public int course_year_limit { get; set; }
            public string course_department { get; set; }
            public string course_department_abbr { get; set; }
            public string department { get; set; }
            public int course_active { get; set; }
            public int enrollment_open { get; set; }
        }
        public class Subjects
        { 
            public string internal_code { get; set; }
            public string subject_name { get; set; }
            public string subject_type { get; set; }
            public string descr_1 { get; set; }
            public string descr_2 { get; set; }
            public string units { get; set; }
            public string semester { get; set; }
            public string course_code { get; set; }
            public string year_level { get; set; }
            public string split_type { get; set; }
            public string split_code{ get; set; }
            public string curriculim_year { get; set; }

        }

        public class Departments
        {
            public string course_department { get; set; }
            public string course_department_abbr { get; set; }
            public string department { get; set; }
        }


    }
}
