using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCPortal.BusinessLogic.Enrollment;
using UCPortal.RequestResponse.Request;
using UCPortal.RequestResponse.Response;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using UCPortal.Authenticator;

namespace UCPortal.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("enrollment")]
    public class EnrollmentController : ControllerBase
    {
        private IEnrollmentManagement _enrollmentManagement;
        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

        public EnrollmentController(IEnrollmentManagement enrollmentManagement, IJwtAuthenticationManager jwtAuthenticationManager)
        {
            _enrollmentManagement = enrollmentManagement;
            _jwtAuthenticationManager = jwtAuthenticationManager;
        }

        /*
         * Endpoint for Registration of new student
         * 
         */
        [HttpPost]
        [Route("users")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegistrationRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new RegistrationResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.RegistrationRequest>(serialized_req);

            //await result from function RegisterStudent
            var result = await Task.FromResult(_enrollmentManagement.RegisterStudent(converted_req));

            //return login reponse
            return Ok(new RegistrationResponse { success = result.success });
        }     

        /*
        * Endpoint for getting the Department
        * 
        */
        [HttpPost]
        [Route("department")]
        public async Task<IActionResult> GetDepartment(GetDepartmentRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetDepartmentResponse { departments = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetDepartmentRequest>(serialized_req);

            //await result from function SaveEnrollmentData
            var result = await Task.FromResult(_enrollmentManagement.GetDepartment(converted_req));

            var response = result.departments.Select(x =>
            {
                var rsRes = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var rRes = Newtonsoft.Json.JsonConvert.DeserializeObject<GetDepartmentResponse.department>(rsRes);
                return rRes;
            }).ToList();

            //return login reponse
            return Ok(new GetDepartmentResponse { departments = response });
        }


        /*
        * Endpoint for getting the Department
        * 
        */
        [HttpPost]
        [Route("course")]
        public async Task<IActionResult> GetCourses([FromBody] GetCoursesRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetCoursesResponse { colleges = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetCoursesRequest>(serialized_req);

            //await result from function SaveEnrollmentData
            var result = await Task.FromResult(_enrollmentManagement.GetCourses(converted_req));

            //convert DTO to response
            var response = result.colleges.Select(x =>
            {
                var rsRes = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var rRes = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCoursesResponse.college>(rsRes);
                return rRes;
            }).ToList();

            //return login reponse
            return Ok(new GetCoursesResponse { colleges = response });
        }

        /*
        * Endpoint for Registration of new student
        * 
        */
        [HttpPost]
        [Route("enroll")]
        public async Task<IActionResult> EnrollStudent([FromBody] SaveEnrollmentRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new RegistrationResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.SaveEnrollmentRequest>(serialized_req);

            //await result from function SaveEnrollmentData
            var result = await Task.FromResult(_enrollmentManagement.SaveEnrollmentData(converted_req));

            //return login reponse
            return Ok(new SaveEnrollmentResponse { success = result.success });
        }

        /*
        * Endpoint for saving payments
        * 
       */
        [HttpPost]
        [Route("enroll/payments")]
        public async Task<IActionResult> SavePayments(SavePaymentRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.SavePaymentRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.SavePayments(converted_req));

            return Ok(new SaveEnrollmentResponse { success = result.succcess });
        }

        /*
        * Endpoint for getting schedule
        * 
        */
        [HttpPost]
        [Route("schedule")]
        public async Task<IActionResult> ViewSchedule([FromBody] ViewScheduleRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new ViewScheduleResponse { schedules = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewScheduleRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.ViewSchedules(converted_req));

            //convert DTO to response
            var response = result.schedules.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewScheduleResponse.schedule>(rSched);
                return cSched;
            }).ToList();

            return Ok(new ViewScheduleResponse { schedules = response.OrderBy(x => x.section).ThenBy(x => x.course_code).ToList(), count = result.count });
        }

        /*
        * Endpoint for getting schedule
        * 
        */
        [HttpPost]
        [Route("schedule/subject")]
        public async Task<IActionResult> ViewSubject([FromBody] SelectSubjectInfoRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(null);
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.SelectSubjectInfoRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.SelectSubject(converted_req));

            //Convert DTO to response result
            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<SelectSubjectInfoResponse>(serialized_result);

            return Ok(converted_result);
        }

        /*
        * Endpoint for getting schedule
        * 
        */
        [HttpPost]
        [Route("schedule/subject/update")]
        public async Task<IActionResult> UpdateSchedules([FromBody] UpdateSubjectRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new UpdateSubjectResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.UpdateSubjectRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.UpdateSubject(converted_req));           

            return Ok(new UpdateSubjectResponse { success = result.success});
        }

        /*
       * Endpoint for getting the active sections per course
       * 
       */
        [HttpPost]
        [Route("schedule/section")]
        public async Task<IActionResult> GetActiveSection([FromBody] GetActiveSectionsRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetActiveSectionsResponse { sections = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetActiveSectionsRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetActiveSections(converted_req));

            //Convert DTO to response result
            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetActiveSectionsResponse>(serialized_result);

            return Ok(converted_result);
        }

        /*
         * Endpoint for viewing classlist
         * 
         */
        [HttpPost]
        [Route("schedule/classlist")]
        public async Task<IActionResult> ViewClasslist(ViewClassListRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewClasslistRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.ViewClasslist(converted_req));

            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewClasslistResponse>(serialized_result);

            return Ok(converted_result);
        }

        /*
        * Endpoint for viewing classlist
        * 
        */
        [HttpPost]
        [Route("schedule/classlist/section")]
        public async Task<IActionResult> ViewClasslistPerSection(ViewClasslistPerSectionRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewClasslistPerSectionRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.ViewClasslistPerSection(converted_req));

            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewClasslistPerSectionResponse>(serialized_result);

            return Ok(converted_result);
        }

        [HttpPost]
        [Route("schedule/section/all")]
        public async Task<IActionResult> GetSections([FromBody] GetSectionRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetSectionResponse { section = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetSectionRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.GetSection(converted_req));

            //convert DTO to response
            var response = result.section.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSectionResponse.sections>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetSectionResponse { section = response });
        }

        [HttpPost]
        [Route("schedule/section/status")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeSchedStatusRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new ChangeSchedStatusResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ChangeSchedStatusRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.ChangeSchedStatus(converted_req));

            return Ok(new ChangeSchedStatusResponse { success = result.success });
        }

        /*
        * Endpoint for getting the studyload
        * 
        */
        [HttpPost]
        [Route("student/studyload")]
        public async Task<IActionResult> ViewStudyLoad([FromBody] GetStudyLoadRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetStudyLoadResponse { schedules = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetStudyLoadRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetStudyLoad(converted_req));

            //convert DTO to response
            var response = result.schedules.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetStudyLoadResponse.Schedules>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetStudyLoadResponse { schedules = response, has_nstp = result.has_nstp, has_pe = result.has_pe });
        }


        /*
        * Endpoint for getting the student status
        * 
        */
        [HttpPost]
        [Route("student/status")]
        public async Task<IActionResult> GetStudentStatus([FromBody] GetStudentStatusRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetStudentStatusResponse { status = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetStudentStatusRequest>(serialized_req);
            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetStudentStatus(converted_req));

            //convert DTO to response
            var response = result.status.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetStudentStatusResponse.Status>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetStudentStatusResponse { status = response, classification = result.classification, is_cancelled = result.is_cancelled, needed_payment = result.needed_payment, pending_promissory = result.pending_promissory, promi_pay = result.promi_pay,  adjustment_open = result.adjustment_open, enrollment_open = result.enrollment_open});
        }

        /*
        * Endpoint for getting the student status
        * 
        */
        [HttpPost]
        [Route("student/promissory/view")]
        public async Task<IActionResult> ViewPromissoryList([FromBody] GetPromissoryListRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetPromissoryListResponse { students = null, count = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetPromissoryListRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetPromissoryList(converted_req));

            //convert DTO to response
            var response = result.students.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetPromissoryListResponse.Student>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetPromissoryListResponse { students = response, count = result.count });
        }

        /*
        * Endpoint for getting all the student per status
        * 
        */
        [HttpPost]
        [Route("student/status/all")]
        public async Task<IActionResult> ViewStudentPerStatus([FromBody] ViewStudentPerStatusRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new ViewStudentPerStatusResponse { students = null });
            }

            //Convert response object to DTO Objectscourses
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewStudentPerStatusRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.ViewStudentStatus(converted_req));

            //convert DTO to response

            List<ViewStudentPerStatusResponse.Student> final = new List<ViewStudentPerStatusResponse.Student>();

            foreach (DTO.Response.ViewStudentPerStatusResponse.Student stud in result.students.ToList())
            {
                String dateF = String.Empty;

                if (stud.status.ToString().Equals("0") || stud.status.ToString().Equals("1") || stud.status.ToString().Equals("2") || stud.status.ToString().Equals("3") || stud.status.ToString().Equals("4") || stud.status.ToString().Equals("5"))
                {
                    dateF = stud.registered_on.Value.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    dateF = stud.enrollmentDate.Value.ToString("yyyy/MM/dd HH:mm:ss");
                }

                ViewStudentPerStatusResponse.Student newStudent = new ViewStudentPerStatusResponse.Student
                {
                    classification = stud.classification,
                    course_code = stud.course_code,
                    course_year = stud.course_year,
                    firstname = stud.firstname,
                    lastname = stud.lastname,
                    id_number = stud.id_number,
                    status = stud.status,
                    suffix = stud.suffix,
                    mi = stud.mi,
                    date = dateF,
                    request_deblock = stud.request_deblock,
                    request_overload = stud.request_overload,
                    has_payment = stud.has_payment,
                    profile = stud.profile,
                    needed_payment = stud.needed_payment.ToString(),
                    has_promissory = stud.has_promissory,
                    promi_pay = stud.promi_pay
                };

                final.Add(newStudent);
            }

            return Ok(new ViewStudentPerStatusResponse { students = final.OrderBy(x => DateTime.Parse(x.date)).ToList(), count = result.count });
        }

        /*
        * Endpoint for the registration info
        * 
        */
        [HttpPost]
        [Route("student/registration")]
        public async Task<IActionResult> ViewStudentRegistraton([FromBody] ViewStudentRegistrationRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(null);
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewStudentRegistrationRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.ViewRegistration(converted_req));

            //Convert DTO to response result
            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewStudentRegistrationResponse>(serialized_result);

            return Ok(converted_result);
        }

        /*
        * Endpoint for the registration info
        * 
        */
        [HttpPost]
        [Route("student/info")]
        public async Task<IActionResult> ViewOldStudentInfo([FromBody] ViewOldStudentInfoRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(null);
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewOldStudentInfoRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.ViewOldStudentInfo(converted_req));

            //Convert DTO to response result
            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewOldStudentInfoResponse>(serialized_result);

            return Ok(converted_result);
        }

        /*
        * Endpoint for updating the status
        * 
        */
        [HttpPost]
        [Route("student/status/update")]
        public async Task<IActionResult> SetApproveOrDissaprove([FromBody] SetApproveOrDisapprovedRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new SetApproveOrDisapprovedResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.SetApproveOrDisapprovedRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.SetApproveOrDisapprove(converted_req));

            //Convert DTO to response result
            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<SetApproveOrDisapprovedResponse>(serialized_result);

            return Ok(converted_result);
        }


        /*
        * Endpoint for uploading images
        * 
        */
        [HttpPost]
        [Route("student/image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new UploadImageResponse { success = 0 });
            }

            string path = "C:\\Users\\james\\Documents\\My Web Sites\\ucportal\\src";
            foreach (var formFile in request.formFiles)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine( /*Environment.CurrentDirectory*/path, "storage", formFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new UploadImageResponse { success = 1, count = request.formFiles.Count });
        }

        /*
        * Endpoint for getting the request
        * 
        */
        [HttpPost]
        [Route("student/status/request")]
        public async Task<IActionResult> GetStudentRequest([FromBody] StudentReqRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new StudentReqResponse { students = null, count = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.StudentReqRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.GetStudentRequest(converted_req));

            //convert DTO to response
            var response = result.students.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<StudentReqResponse.Student>(rSched);
                return cSched;
            }).ToList();

            return Ok(new StudentReqResponse { students = response, count = result.count });
        }


        /*
       * Endpoint for applying student request
       * 
       */
        [HttpPost]
        [Route("student/request")]
        public async Task<IActionResult> ApplyStudentRequest([FromBody] ApplyReqRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new ApplyReqResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ApplyReqRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.ApplyRequest(converted_req));

            return Ok(new ApplyReqResponse { success = result.success });
        }

        /*
      * Endpoint for applying student request
      * 
      */
        [HttpPost]
        [Route("student/request/approve")]
        public async Task<IActionResult> ApproveStudentRequest([FromBody] ApproveReqRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new ApproveReqResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ApproveReqRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.ApproveRequest(converted_req));

            return Ok(new ApproveReqResponse { success = result.success });
        }

        /*
      * Endpoint for applying student request
      * 
      */
        [HttpPost]
        [Route("student/request/promissory")]
        public async Task<IActionResult> RequestPromissory([FromBody] RequestPromissoryRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new RequestPromissoryResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.RequestPromissoryRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.RequestPromissory(converted_req));

            return Ok(new RequestPromissoryResponse { success = result.success });
        }

        [HttpPost]
        [Route("student/cancel")]
        public async Task<IActionResult> CancelEnrollment([FromBody] CancelEnrollmentRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new CancelEnrollmentResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.CancelEnrollmentRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.CancelEnrollment(converted_req));

            return Ok(new CancelEnrollmentResponse { success = result.success });
        }

        [HttpPost]
        [Route("student/update")]
        public async Task<IActionResult> UpdateLoginInfo([FromBody] UpdateStudentInfoRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new UpdateStudentInfoResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.UpdateStudentInfoRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.UpdateStudentInfo(converted_req));

            return Ok(new CancelEnrollmentResponse { success = result.success });
        }


        [HttpPost]
        [Route("student/evaluation")]
        public async Task<IActionResult> ViewStudentEvaluation([FromBody] ViewStudentEvaluationRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new ViewStudentEvaluationResponse { studentGrades = null});
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ViewStudentEvaluationRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.ViewEvaluation(converted_req));

            var response = result.studentGrades.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewStudentEvaluationResponse.Grades>(rSched);
                return cSched;
            }).ToList();

            return Ok(new ViewStudentEvaluationResponse { studentGrades = response });
        }

        [HttpPost]
        [Route("adjustment")]
        public async Task<IActionResult> SaveAdjustment([FromBody] SaveAdjustmentRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new SaveAdjustmentResponse { success = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.SaveAdjustmentRequest>(serialized_req);

            //await result from function GetStudentRequest
            var result = await Task.FromResult(_enrollmentManagement.SaveAdjustment(converted_req));

            return Ok(new SaveAdjustmentResponse { success = result.success });
        }

        /*
        * Endpoint for getting the student status
        * 
        */
        [HttpPost]
        [Route("adjustment/view")]
        public async Task<IActionResult> ViewAdjustmentList([FromBody] GetAdjustmentListRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetAdjustmentListResponse { students = null, count = 0 });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetAdjustmentListRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetAdjustmentlist(converted_req));

            //convert DTO to response
            var response = result.students.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAdjustmentListResponse.Student>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetAdjustmentListResponse { students = response, count = result.count });
        }

        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("status/count")]
        public async Task<IActionResult> GetStatusCount(GetStatusCountRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetStatusCountRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.GetStatusCount(converted_req));

            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetStatusCountResponse>(serialized_result);

            return Ok(converted_result);
        }


        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("status/enrollmentStatus")]
        public async Task<IActionResult> GetEnrollmentStatus(GetEnrollmentStatusRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetEnrollmentStatusRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.GetEnrollmentStatus(converted_req));
            //convert DTO to response
            var response = result.courseStat.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetEnrollmentStatusResponse.courseStatus>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetEnrollmentStatusResponse { courseStat = response});
        }

        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("notification/add")]
        public async Task<IActionResult> AddNotification(AddNotificationRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.AddNotificationRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.AddNotification(converted_req));

            return Ok(new AddNotificationResponse { success = result.success });
        }

        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("teachers/list")]
        public async Task<IActionResult> GetTeachersList(GetTeachersListRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetTeachersListRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.GetTeachersList(converted_req));

            var response = result.teacherList.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetTeachersListResponse.Teachers>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetTeachersListResponse { teacherList = response });
        }

        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("teachers/teachersLoad")]
        public async Task<IActionResult> SaveTeachersLoad(SaveTeachersLoadRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.SaveTeachersLoadRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.SaveTeachersLoad(converted_req));;

            return Ok(new SaveTeachersLoadResponse { success = result.success });
        }

        /*
        * Endpoint for getting the studyload
        * 
        */
        [HttpPost]
        [Route("teachers/getteachersload")]
        public async Task<IActionResult> ViewTeachersLoad([FromBody] GetStudyLoadRequest request)
        {
            //Check if required fields are present
            if (!ModelState.IsValid)
            {
                return Ok(new GetStudyLoadResponse { schedules = null });
            }

            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetTeachersLoadRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetTeachersLoad(converted_req));

            //convert DTO to response
            var response = result.schedules.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetTeachersLoadResponse.Schedules>(rSched);
                return cSched;
            }).ToList();

            return Ok(new GetTeachersLoadResponse { schedules = response });
        }

        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("tools/cleanseostsp")]
        public async Task<IActionResult> REmoveDuplicateOstsp()
        {          
            var result = await Task.FromResult(_enrollmentManagement.RemoveDuplicateOstsp());

            return Ok(new RemoveDuplicateOtspResponse { success = result.success });
        }

        /*
      * Endpoint for uploading images
      * 
      */
        [HttpPost]
        [Route("tools/forceupdatestatus")]
        public async Task<IActionResult> ForceUpdateStatus(UpdateStudentStatusRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.UpdateStudentStatusRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.UpdateStudentStatus(converted_req));

            return Ok(new UpdateStudentStatusResponse { success = result.success });
        }

        /*
     * Endpoint for uploading images
     * 
     */
        [HttpPost]
        [Route("tools/manualenrollment")]
        public async Task<IActionResult> ManualEnrollment(ManualEnrollmentRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.ManualEnrollmentRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.ManualEnrollment(converted_req));

            return Ok(new UpdateStudentStatusResponse { success = result.success });
        }

        /*
      * Endpoint for uploading images
      * 
      */
        [HttpPost]
        [Route("tools/correcttotalunits")]
        public async Task<IActionResult> CorrectTotalUnits()
        {
            var result = await Task.FromResult(_enrollmentManagement.CorrectTotalUnits());

            return Ok(new RemoveDuplicateOtspResponse { success = result.success });
        }


        /*
        * Endpoint for uploading images
        * 
        */
        [HttpPost]
        [Route("tools/updateInfo/view")]
        public async Task<IActionResult> UpdateInfoView(UpdateInfoRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.UpdateInfoRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.GetInfoUpdate(converted_req));

            var serialized_result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            var converted_result = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateInfoResponse>(serialized_result);

            return Ok(converted_result);
        }

        /*
       * Endpoint for uploading images
       * 
       */
        [HttpPost]
        [Route("tools/updateInfo")]
        public async Task<IActionResult> UpdateInfo(UpdateInforRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.UpdateInforRequest>(serialized_req);

            var result = await Task.FromResult(_enrollmentManagement.UpdateInfor(converted_req));

            return Ok(new UpdateInforResponse { success = result.success });
        }

        [HttpPost]
        [Route("tools/setclosedsubject")]
        public async Task<IActionResult> SetClosedSubject()
        {
            var result = await Task.FromResult(_enrollmentManagement.SetClosed());

            return Ok(new SetClosedSubjectResponse { success = result.success });
        }


        [HttpPost]
        [Route("student/prospectus")]
        public async Task<IActionResult> GetCurriculum([FromBody] GetCurriculumRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetCurriculumRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetCurriculum(converted_req));

            // convert DTO to response
            // convert DTO to response


            var response = result.subjects.Select(x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCurriculumResponse.Subjects>(rSched);
                return cSched;
            }).ToList();

            var remark = result.prerequisites.Select( x =>
            {
                var rSched = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSched = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCurriculumResponse.Prerequisites>(rSched);
                return cSched;
            }).ToList();

            var grades = result.grades.Select(x =>
            {
                var rGrade = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cGrade = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCurriculumResponse.Grades>(rGrade);
                return cGrade;
            }).ToList();

            var schedules = result.schedules.Select(x =>
            {
                var rSchedule = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSchedule = Newtonsoft.Json.JsonConvert.DeserializeObject<GetCurriculumResponse.Schedules>(rSchedule);
                return cSchedule;
            }).ToList();

            return Ok(new GetCurriculumResponse {subjects = response, course_code = result.course_code, prerequisites = remark, grades = grades, schedules = schedules,units = result.units});
        }
        [HttpPost]
        [Route("student/requestsubject")]
        public async Task<IActionResult> RequestSubject([FromBody] StudentSubjectRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.StudentSubjectRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.RequestSubject(converted_req));

            // convert DTO to response
            // convert DTO to response
            
            return Ok(new StudentSubjectResponse { success=result.success });
        }

        [HttpPost]
        [Route("student/getrequestsubject")]
        public async Task<IActionResult> GetRequestSubject([FromBody] GetSubjectReqRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetSubjectReqRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetRequestSubject(converted_req));

            var requestSubjects = result.request.Select(x =>
            {
                var rSchedule = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSchedule = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSubjectReqResponse.RequestedSubject>(rSchedule);
                return cSchedule;
            }).ToList();
            // convert DTO to response

            return Ok(new GetSubjectReqResponse { request = requestSubjects });
        }


        [HttpPost]
        [Route("student/getstudentrequest")]
        public async Task<IActionResult> GetStudentRequest([FromBody] GetStudentReqRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.GetStudentReqRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetStudentSubjectRequest(converted_req));

            var requestSubjects = result.request.Select(x =>
            {
                var rSchedule = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSchedule = Newtonsoft.Json.JsonConvert.DeserializeObject<GetStudentReqResponse.RequestedSubject>(rSchedule);
                return cSchedule;
            }).ToList();
            var filteredSubjects = result.filtered.Select(x =>
            {
                var rSchedule = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSchedule = Newtonsoft.Json.JsonConvert.DeserializeObject<GetStudentReqResponse.FilteredSubject>(rSchedule);
                return cSchedule;
            }).ToList();
            // convert DTO to response

            return Ok(new GetStudentReqResponse { request = requestSubjects, filtered= filteredSubjects});
        }

        [HttpPost]
        [Route("student/addstudentrequest")]
        public async Task<IActionResult> AddStudentRequest([FromBody] AddStudentReqRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.AddStudentReqRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.AddSubjectRequest(converted_req));

            return Ok(new AddStudentReqResponse { success= result.success });
        }

        [HttpPost]
        [Route("student/cancelstudentrequest")]
        public async Task<IActionResult> CancelSubjectRequest([FromBody] CancelSubjectReqRequest request)
        {
            //Convert response object to DTO Objects
            var serialized_req = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var converted_req = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Request.CancelSubjectReqRequest>(serialized_req);

            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.CancelSubjectRequest(converted_req));

            return Ok(new CancelEnrollmentResponse { success = result.success });
        }

        [HttpPost]
        [Route("student/allcurriculum")]
        public async Task<IActionResult> GetAllCurriclum()
        {
           
            //await result from function ChangePassword
            var result = await Task.FromResult(_enrollmentManagement.GetAllCurriculum());


            var getYears = result.year.Select(x =>
            {
                var rYears = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cYears = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAllCurriculumResponse.SchoolYear>(rYears);
                return cYears;

            }).ToList();
            var getCourses = result.courses.Select(x =>
            {
                var rCourse = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cCourse = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAllCurriculumResponse.Courses>(rCourse);
                return cCourse;

            }).ToList();
            var getSubjects = result.subjects.Select(x =>
            {
                var rSubjects = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cSubjects = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAllCurriculumResponse.Subjects>(rSubjects);
                return cSubjects;

            }).ToList();
            var getDepartments = result.departments.Select(x =>
            {
                var rCourse = Newtonsoft.Json.JsonConvert.SerializeObject(x);
                var cCourse = Newtonsoft.Json.JsonConvert.DeserializeObject<GetAllCurriculumResponse.Departments>(rCourse);
                return cCourse;

            }).ToList();
            return Ok(new GetAllCurriculumResponse { year = getYears, course = getCourses,  subjects = getSubjects,departments = getDepartments});
        }
    }
}
