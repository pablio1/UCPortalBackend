using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCPortal.DatabaseEntities.Models;
using UCPortal.DTO.Request;
using UCPortal.DTO.Response;
using UCPortal.EmailHandler;
using UCPortal.EmailHandler.Handlers;
using UCPortal.RequestResponse.Enums;
using UCPortal.RequestResponse.Literals;
using UCPortal.Utils;

namespace UCPortal.BusinessLogic.Enrollment
{
    public class EnrollmentManagement : IEnrollmentManagement
    {
        private UCOnlinePortalContext _ucOnlinePortalContext;
        private ISMTPHandler _emailHandler;

        public EnrollmentManagement(UCOnlinePortalContext ucOnlinePortalContext, ISMTPHandler emailHandler)
        {
            _ucOnlinePortalContext = ucOnlinePortalContext;
            _emailHandler = emailHandler;
        }

        /*
         * Method to add new student record
        */
        public RegistrationResponse RegisterStudent(RegistrationRequest registrationRequest)
        {
            //Generate random number for Token
            Random generator = new Random();
            String token = generator.Next(0, 1000000).ToString("D6");

            bool hasError = false;

            try
            {
                //Create new student object
                _212studentInfo newStudent = new _212studentInfo
                {
                    CourseCode = registrationRequest.student_info.course,
                    YearLevel = registrationRequest.student_info.year_level,
                    Mdn = registrationRequest.student_info.mdn,
                    FirstName = registrationRequest.student_info.first_name,
                    LastName = registrationRequest.student_info.last_name,
                    MiddleName = registrationRequest.student_info.middle_name,
                    Suffix = registrationRequest.student_info.suffix,
                    Gender = registrationRequest.student_info.gender,
                    Status = registrationRequest.student_info.status,
                    Nationality = registrationRequest.student_info.nationality,
                    BirthDate = DateTime.ParseExact(registrationRequest.student_info.birthdate, "yyyy-MM-dd", null),
                    BirthPlace = registrationRequest.student_info.birthplace,
                    Religion = registrationRequest.student_info.religion,
                    DateCreated = DateTime.Now.Date,
                    DateUpdated = DateTime.Now.Date,
                    StartTerm = (short)registrationRequest.student_info.start_term,
                    IsVerified = 0,
                    Token = token,
                    Classification = registrationRequest.student_info.classification,
                    Dept = registrationRequest.student_info.dept
                };

                //Insert to table
                _ucOnlinePortalContext._212studentInfos.Add(newStudent);
                _ucOnlinePortalContext.SaveChanges();

                //Save primary id which will be used in linking to different tables
                int stud_info_id = newStudent.StudInfoId;

                //Update id number to stud_info_id for linking
                var newStudentRecord = _ucOnlinePortalContext._212studentInfos.Where(x => x.StudInfoId == stud_info_id).FirstOrDefault();
                newStudentRecord.StudId = stud_info_id.ToString();

                //Temporarily use primary id for student id
                _ucOnlinePortalContext._212studentInfos.Update(newStudentRecord);
                _ucOnlinePortalContext.SaveChanges();

                //Create new contact & address object
                _212contactAddress newContactAddress = new _212contactAddress
                {
                    StudInfoId = (short)stud_info_id,
                    PCountry = registrationRequest.address_contact.pcountry,
                    PProvince = registrationRequest.address_contact.pprovince,
                    PCity = registrationRequest.address_contact.pcity,
                    PBarangay = registrationRequest.address_contact.pbarangay,
                    PStreet = registrationRequest.address_contact.pstreet,
                    PZip = registrationRequest.address_contact.pzip,
                    CProvince = registrationRequest.address_contact.cprovince,
                    CCity = registrationRequest.address_contact.ccity,
                    CBarangay = registrationRequest.address_contact.cbarangay,
                    CStreet = registrationRequest.address_contact.cstreet,
                    Mobile = registrationRequest.address_contact.mobile,
                    Landline = registrationRequest.address_contact.landline,
                    Email = registrationRequest.address_contact.email,
                    Facebook = registrationRequest.address_contact.facebook
                };

                //Create new family info object
                _212familyInfo newFamilyInfo = new _212familyInfo
                {
                    StudInfoId = (short)stud_info_id,
                    FatherName = registrationRequest.family_info.father_name,
                    FatherContact = registrationRequest.family_info.father_contact,
                    FatherOccupation = registrationRequest.family_info.father_occupation,
                    MotherName = registrationRequest.family_info.mother_name,
                    MotherContact = registrationRequest.family_info.mother_contact,
                    MotherOccupation = registrationRequest.family_info.mother_occupation,
                    GuardianName = registrationRequest.family_info.guardian_name,
                    GuardianContact = registrationRequest.family_info.guardian_contact,
                    GuardianOccupation = registrationRequest.family_info.guardian_occupation
                };

                //Create new school info object
                _212schoolInfo newSchoolInfo = new _212schoolInfo
                {
                    StudInfoId = (short)stud_info_id,
                    ElemName = registrationRequest.school_info.elem_name,
                    ElemYear = registrationRequest.school_info.elem_year,
                    ElemLastYear = (short)registrationRequest.school_info.elem_last_year,
                    ElemType = registrationRequest.school_info.elem_type,
                    ElemLrnNo = registrationRequest.school_info.elem_lrn_number,
                    ElemEscSchoolId = registrationRequest.school_info.elem_esc_school_id,
                    ElemEscStudentId = registrationRequest.school_info.elem_esc_student_id,
                    SecName = registrationRequest.school_info.sec_name,
                    SecYear = registrationRequest.school_info.sec_year,
                    SecLastYear = (short)registrationRequest.school_info.sec_last_year,
                    SecLastStrand = registrationRequest.school_info.sec_last_strand,
                    SecType = registrationRequest.school_info.sec_type,
                    SecLrnNo = registrationRequest.school_info.sec_lrn_number,
                    SecEscSchoolId = registrationRequest.school_info.sec_esc_school_id,
                    SecEscStudentId = registrationRequest.school_info.sec_esc_student_id,
                    ColName = registrationRequest.school_info.col_name,
                    ColYear = registrationRequest.school_info.col_year,
                    ColCourse = registrationRequest.school_info.col_course,
                    ColLastYear = (short)registrationRequest.school_info.col_last_year
                };


                foreach (RegistrationRequest.Attachment attachment in registrationRequest.attachment)
                {
                    _212attachment newAttachment = new _212attachment
                    {
                        StudId = stud_info_id.ToString(),
                        Email = attachment.email,
                        Filename = attachment.filename,
                        Type = attachment.type
                    };

                    _ucOnlinePortalContext._212attachments.Add(newAttachment);
                }
                //Add to tables
                _ucOnlinePortalContext._212contactAddresses.Add(newContactAddress);
                _ucOnlinePortalContext._212familyInfos.Add(newFamilyInfo);
                _ucOnlinePortalContext._212schoolInfos.Add(newSchoolInfo);

                //Save Changes
                _ucOnlinePortalContext.SaveChanges();


                //Add OENRP
                _212oenrp newStudentOenrp = new _212oenrp
                {
                    StudId = stud_info_id.ToString(),
                    YearLevel = (short)registrationRequest.student_info.year_level,
                    CourseCode = registrationRequest.student_info.course,
                    RegisteredOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Units = 0,
                    Classification = registrationRequest.student_info.classification,
                    Dept = registrationRequest.student_info.dept,
                    Status = (short)EnrollmentStatus.REGISTERED,
                    AdjustmentCount = 1,
                    RequestDeblock = 0,
                    RequestOverload = 0,
                    NeededPayment = 0,
                    RequestPromissory = 0,
                    PromiPay = 0
                };

                //Save OENRP
                _ucOnlinePortalContext._212oenrps.Add(newStudentOenrp);
                _ucOnlinePortalContext.SaveChanges();

                //Checkfirst if name has similarity
                String sourceName = registrationRequest.student_info.first_name.Trim() + registrationRequest.student_info.last_name.Trim() + (registrationRequest.student_info.middle_name.Equals(String.Empty) ? "" : registrationRequest.student_info.middle_name.Substring(0, 1));
                //Compare and find 
                var checkIfNameExistOrlike = (from loginInfo in _ucOnlinePortalContext.LoginInfos.AsEnumerable()
                                              where Utils.Function.CalculateSimilarity(loginInfo.FirstName.Trim() + loginInfo.LastName.Trim() + loginInfo.Mi.Trim(), sourceName) > 0.75
                                              select loginInfo);


                var result = checkIfNameExistOrlike.ToList();

                if (result == null || result.Count == 0)
                {
                    newStudentOenrp.ApprovedRegRegistrar = "AUTO-APPROVE";
                    newStudentOenrp.ApprovedRegRegistrarOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    newStudentOenrp.Status = 1;
                }


                //Insert new record to loginInfo but with No Id Numbers first
                LoginInfo newLogin = new LoginInfo
                {
                    StudId = stud_info_id.ToString(),
                    LastName = registrationRequest.student_info.last_name,
                    FirstName = registrationRequest.student_info.first_name,
                    Mi = registrationRequest.student_info.middle_name.Equals(String.Empty) ? "" : registrationRequest.student_info.middle_name.Substring(0, 1),
                    Suffix = registrationRequest.student_info.suffix,
                    StartTerm = registrationRequest.student_info.start_term.ToString(),
                    Password = Utils.Function.EncodeBase64(registrationRequest.student_info.password),
                    Dept = registrationRequest.student_info.dept,
                    Year = (short)registrationRequest.student_info.year_level,
                    CourseCode = registrationRequest.student_info.course,
                    Sex = registrationRequest.student_info.gender,
                    MobileNumber = registrationRequest.address_contact.mobile,
                    Email = registrationRequest.address_contact.email,
                    Birthdate = DateTime.ParseExact(registrationRequest.student_info.birthdate, "yyyy-MM-dd", null),
                    Facebook = registrationRequest.address_contact.facebook,
                    IsVerified = 1,
                    IsBlocked = 0,
                    UserType = "STUDENT",
                    Token = token
                };

                //Save Login Info
                _ucOnlinePortalContext.LoginInfos.Add(newLogin);
                _ucOnlinePortalContext.SaveChanges();

                //Create Notification
                _212notification newNotification = new _212notification
                {
                    StudId = stud_info_id.ToString(),
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.REGISTERED,
                    NotifRead = 0
                };

                //Save Notification
                _ucOnlinePortalContext._212notifications.Add(newNotification);
                _ucOnlinePortalContext.SaveChanges();

                if (result == null || result.Count == 0)
                {
                    //Create Notification
                    newNotification = new _212notification
                    {
                        StudId = stud_info_id.ToString(),
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.APPROVED_REGISTRATION_REGISTRAR,
                        NotifRead = 0
                    };

                    //Save Notification

                    _ucOnlinePortalContext._212notifications.Add(newNotification);
                }

                //Delete old tmp login
                var findTmpLogin = _ucOnlinePortalContext.TmpLogins.Where(x => x.Email == registrationRequest.address_contact.email).FirstOrDefault();

                if (findTmpLogin != null)
                {
                    _ucOnlinePortalContext.TmpLogins.Attach(findTmpLogin);
                    _ucOnlinePortalContext.TmpLogins.Remove(findTmpLogin);
                }

                _ucOnlinePortalContext.SaveChanges();
            }
            catch (Exception ex)
            {
                String message = ex.InnerException.ToString();
                hasError = true;
            }


            if (hasError)
            {
                return new RegistrationResponse { success = 0 };
            }
            else
            {
                return new RegistrationResponse { success = 1 };
            }
        }


        /*
         * Method to save enrollment data
        */
        public SaveEnrollmentResponse SaveEnrollmentData(SaveEnrollmentRequest saveEnrollRequest)
        {
            //Get data from Login Info
            var student = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == saveEnrollRequest.id_number).FirstOrDefault();
            List<_212schedule> schedules = new List<_212schedule>();

            bool hasError = false;
            bool isFull = false;

            try
            {
                var enrollmentData = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == saveEnrollRequest.id_number).FirstOrDefault();

                enrollmentData.YearLevel = (short)saveEnrollRequest.year_level;
                enrollmentData.CourseCode = student.CourseCode;
                enrollmentData.EnrollmentDate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                enrollmentData.SubmittedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                enrollmentData.Units = (short)saveEnrollRequest.total_units;
                enrollmentData.Classification = saveEnrollRequest.classification;
                enrollmentData.Dept = student.Dept;
                enrollmentData.Status = (short)EnrollmentStatus.SELECTING_SUBJECTS;
                enrollmentData.AdjustmentCount = 1;
                enrollmentData.DisapprovedDeanOn = null;

                if (enrollmentData.DisapprovedDean != null && enrollmentData.DisapprovedDean != "")
                {
                    enrollmentData.DisapprovedDean = null;
                    enrollmentData.DisapprovedDeanOn = null;
                }

                _212notification newNotif = new _212notification
                {
                    StudId = saveEnrollRequest.id_number,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.SELECTING_SUBJECTS
                };

                _ucOnlinePortalContext._212notifications.Add(newNotif);
                _ucOnlinePortalContext.SaveChanges();

                if (saveEnrollRequest.accept_section == 1)
                {
                    enrollmentData.Status = (short)EnrollmentStatus.APPROVED_BY_DEAN;
                    enrollmentData.ApprovedDean = "AUTO-APPROVE";
                    enrollmentData.ApprovedDeanOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    enrollmentData.Status = (short)EnrollmentStatus.APPROVED_BY_DEAN;

                    //Insert OSTSP if section is set by dean
                    var ostsp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == saveEnrollRequest.id_number && x.Status != 2).Select(x => x.EdpCode).ToList();

                    schedules = _ucOnlinePortalContext._212schedules.Where(x => ostsp.Contains(x.EdpCode) && x.Size == x.MaxSize).ToList();

                    if (schedules.Count > 0 && saveEnrollRequest.accept_section != 1)
                    {
                        isFull = true;
                    }
                    else
                    {
                        //Insert OSTSP if section is set by dean
                        var schedulesOstp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == saveEnrollRequest.id_number && x.Status != 2).ToList();

                        var selected = schedulesOstp.Select(x => x.EdpCode).ToList();
                        var addPeOrNstp = saveEnrollRequest.schedules.Except(selected).ToList();

                        //Iterate Schedules to save individual EDP codes to OSTSP
                        for (int index = 0; index < addPeOrNstp.Count; index++)
                        {
                            _212ostsp newStudentOstsp = new _212ostsp
                            {
                                StudId = saveEnrollRequest.id_number,
                                EdpCode = addPeOrNstp[index],
                                Status = 1,
                                Remarks = null,
                                AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                            };

                            var scheduleAdd = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == addPeOrNstp[index]).FirstOrDefault();
                            scheduleAdd.Size += 1;
                            scheduleAdd.PendingEnrolled += 1;
                            _ucOnlinePortalContext._212schedules.Update(scheduleAdd);

                            //Save OSTSP
                            _ucOnlinePortalContext._212ostsps.Add(newStudentOstsp);
                            _ucOnlinePortalContext.SaveChanges();
                        }

                        schedulesOstp.ToList().ForEach(x => x.Status = 1);
                        schedulesOstp.ToList().ForEach(x => x.AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

                        newNotif = new _212notification
                        {
                            StudId = saveEnrollRequest.id_number,
                            NotifRead = 0,
                            Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                            Message = Literals.APPROVED_BY_DEAN
                        };

                        _ucOnlinePortalContext._212notifications.Add(newNotif);
                        _ucOnlinePortalContext.SaveChanges();

                        //If student is Accounting, Auto Approve!
                        if (!saveEnrollRequest.classification.Equals(String.Empty))
                        {
                            if (saveEnrollRequest.classification.Equals("H"))
                            {
                                enrollmentData.ApprovedAcctg = "AUTO-APPROVE";
                                enrollmentData.ApprovedAcctgOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                enrollmentData.NeededPayment = 500;

                                newNotif = new _212notification
                                {
                                    StudId = saveEnrollRequest.id_number,
                                    NotifRead = 0,
                                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                                    Message = Literals.APPROVED_BY_ACCOUNTING
                                };

                                enrollmentData.Status = (short)EnrollmentStatus.APPROVED_BY_ACCOUNTING;

                                _ucOnlinePortalContext._212notifications.Add(newNotif);
                                _ucOnlinePortalContext.SaveChanges();
                            }

                        }

                    }
                }

                var ostspSelected = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == saveEnrollRequest.id_number && x.Status != 2).Select(x => x.EdpCode).ToList();

                if (ostspSelected.Count > 0)
                {
                    if (saveEnrollRequest.schedules.Count > 0)
                    {
                        var toDelete = ostspSelected.Except(saveEnrollRequest.schedules).ToList();
                        var toAdd = saveEnrollRequest.schedules.Except(ostspSelected).ToList();

                        if (toDelete.Count > 0)
                        {
                            var schedule = _ucOnlinePortalContext._212ostsps.Where(x => toDelete.Contains(x.EdpCode) && x.StudId == saveEnrollRequest.id_number);

                            schedule.ToList().ForEach(x => x.Status = 2);
                            schedule.ToList().ForEach(x => x.AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                            _ucOnlinePortalContext.SaveChanges();
                        }

                        //Iterate Schedules to save individual EDP codes to OSTSP
                        for (int index = 0; index < toAdd.Count; index++)
                        {
                            var otspSched = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == saveEnrollRequest.id_number).Count();

                            if (otspSched == 0)
                            {
                                _212ostsp newStudentOstsp = new _212ostsp
                                {
                                    StudId = saveEnrollRequest.id_number,
                                    EdpCode = toAdd[index],
                                    Status = 0,
                                    Remarks = null,
                                    AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                                };

                                //Save OSTSP
                                _ucOnlinePortalContext._212ostsps.Add(newStudentOstsp);
                                _ucOnlinePortalContext.SaveChanges();
                            }
                        }
                    }
                }

                if (ostspSelected.Count == 0)
                {
                    //Iterate Schedules to save individual EDP codes to OSTSP
                    for (int index = 0; index < saveEnrollRequest.schedules.Count; index++)
                    {
                        var otspSched = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == saveEnrollRequest.id_number).Count();

                        if (otspSched == 0)
                        {
                            _212ostsp newStudentOstsp = new _212ostsp
                            {
                                StudId = saveEnrollRequest.id_number,
                                EdpCode = saveEnrollRequest.schedules[index],
                                Status = 0,
                                Remarks = null,
                                AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                            };

                            //Save OSTSP
                            _ucOnlinePortalContext._212ostsps.Add(newStudentOstsp);
                            _ucOnlinePortalContext.SaveChanges();
                        }
                    }
                }
            }
            catch
            {
                hasError = true;
            }

            if (hasError)
            {
                return new SaveEnrollmentResponse { success = 0 };
            }
            else
            {
                return new SaveEnrollmentResponse { success = 1 };
            }
        }

        /*
         * Method to get all departments
        */
        public GetDepartmentResponse GetDepartment(GetDepartmentRequest getDepartmentRequest)
        {
            //get department names for college
            var departments = _ucOnlinePortalContext.CourseLists.Where(x => x.Department == getDepartmentRequest.department && x.CourseActive == 1).ToList();

            var departmentResult = departments.Select(x => new GetDepartmentResponse.department
            {
                dept_abbr = x.CourseDepartmentAbbr,
                dept_name = x.CourseDepartment
            }).Distinct().ToList();

            var departmentList = departmentResult.GroupBy(x => x.dept_abbr).Select(grp => grp.First());

            return new GetDepartmentResponse { departments = departmentList.ToList() };
        }

        /*
       * Method to get all colleges from department
       */
        public GetCoursesResponse GetCourses(GetCoursesRequest getCollegeRequest)
        {
            var colleges = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseActive == 1).ToList();

            if (!getCollegeRequest.department.Equals(String.Empty))
            {
                colleges = _ucOnlinePortalContext.CourseLists.Where(x => (x.Department == getCollegeRequest.department && x.CourseActive == 1)).ToList();
            }
            else if (!getCollegeRequest.course_department.Equals(String.Empty) || !getCollegeRequest.department_abbr.Equals(String.Empty))
            {
                colleges = _ucOnlinePortalContext.CourseLists.Where(x => (x.CourseDepartment == getCollegeRequest.course_department || x.CourseDepartmentAbbr == getCollegeRequest.department_abbr) && x.CourseActive == 1).ToList();
            }

            var collegeResult = colleges.Select(x => new GetCoursesResponse.college
            {
                college_id = x.CourseId,
                college_code = x.CourseCode,
                college_name = x.CourseAbbr + " - " + x.CourseDescription,
                year_limit = x.CourseYearLimit,
                department = x.Department
            }).ToList();

            return new GetCoursesResponse { colleges = collegeResult };
        }

        /*
        * Method to view schedules
        */
        public ViewScheduleResponse ViewSchedules(ViewScheduleRequest viewScheduleRequest)
        {
            int take = (int)viewScheduleRequest.limit;
            int skip = (int)viewScheduleRequest.limit * ((int)viewScheduleRequest.page - 1);

            var result = (from _schedules in _ucOnlinePortalContext._212schedules
                          join _subject_info in _ucOnlinePortalContext.SubjectInfos
                          on _schedules.InternalCode equals _subject_info.InternalCode into sched
                          from _subject_info in sched.DefaultIfEmpty()
                          join _courselist in _ucOnlinePortalContext.CourseLists
                          on _schedules.CourseCode equals _courselist.CourseCode into course
                          from _courselist in course.DefaultIfEmpty()
                          join _instructor in _ucOnlinePortalContext.LoginInfos
                          on _schedules.Instructor equals _instructor.StudId into instructor
                          from _instructor in instructor.DefaultIfEmpty()
                          select new ViewScheduleResponse.schedule
                          {
                              edpcode = _schedules.EdpCode,
                              subject_name = _schedules.Description,
                              subject_type = _schedules.SubType,
                              days = _schedules.Days,
                              begin_time = _schedules.TimeStart,
                              end_time = _schedules.TimeEnd,
                              mdn = _schedules.Mdn,
                              m_begin_time = _schedules.MTimeStart,
                              m_end_time = _schedules.MTimeEnd,
                              m_days = _schedules.MDays,
                              units = _schedules.Units.ToString(),
                              room = _schedules.Room,
                              size = _schedules.Size.ToString(),
                              pending_enrolled = (int)_schedules.PendingEnrolled,
                              official_enrolled = (int)_schedules.OfficialEnrolled,
                              max_size = _schedules.MaxSize.ToString(),
                              status = _schedules.Status,
                              section = _schedules.Section,
                              split_code = _schedules.SplitCode,
                              split_type = _schedules.SplitType,
                              descriptive_title = _subject_info.Descr1 + _subject_info.Descr2,
                              course_code = _schedules.CourseCode,
                              course_abbr = _courselist.CourseAbbr,
                              gened = (short)_schedules.IsGened,
                              instructor = _instructor.LastName + " , " + _instructor.FirstName
                          });

            if (viewScheduleRequest.gen_ed != null && !viewScheduleRequest.gen_ed.Equals(String.Empty))
            {
                var splitGen = viewScheduleRequest.gen_ed.Split(",").Select(Int32.Parse).ToList();
                if (splitGen.Count == 0)
                {
                    result = result.Where(x => x.gened == short.Parse(viewScheduleRequest.gen_ed));
                }
                else
                {
                    result = result.Where(x => splitGen.Contains(x.gened));
                }
            }

            if (!viewScheduleRequest.department_abbr.Equals(String.Empty))
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == viewScheduleRequest.department_abbr).ToList();

                var courses = courseList.Select(x => x.CourseCode).ToList();
                result = result.Where(x => courses.Contains(x.course_code));
            }

            if (!viewScheduleRequest.course_code.Equals(String.Empty))
            {
                String[] courseCodeArray = { "PN", "MT", "CC", "BN", "MM", "MI", "MV", "MR" };

                if (viewScheduleRequest.no_nstp > 0 && viewScheduleRequest.no_pe > 0)
                {
                    if (viewScheduleRequest.course_code.Contains("PN"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("PE"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MT"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("PE"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("CC"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("DEFTACT"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("BN"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("PE"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MI"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("P.E"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MR"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("P.E"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MM") || viewScheduleRequest.course_code.Contains("MV"))
                    {
                        result = result.Where(x => (x.course_code.Contains("MM") || x.course_code.Contains("MV")) && x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("P.E"));
                    }
                    else
                    {
                        result = result.Where(x => !courseCodeArray.Contains(x.course_code) && (x.subject_name.StartsWith("NSTP") || x.subject_name.StartsWith("PE")));
                    }
                }
                else if (viewScheduleRequest.no_nstp > 0)
                {
                    if (viewScheduleRequest.course_code.Contains("PN"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MT"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("CC"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("BN"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MI"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MR"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("NSTP"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MM") || viewScheduleRequest.course_code.Contains("MV"))
                    {
                        result = result.Where(x => (x.course_code.Contains("MM") || x.course_code.Contains("MV")) && x.subject_name.StartsWith("NSTP"));
                    }
                    else
                    {
                        result = result.Where(x => !courseCodeArray.Contains(x.course_code) && x.subject_name.StartsWith("NSTP"));
                    }
                }
                else if (viewScheduleRequest.no_pe > 0)
                {
                    if (viewScheduleRequest.course_code.Contains("PN"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("PE"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MT"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("PE"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("CC"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("DEFTACT"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("BN"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("PE"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MI"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("P.E"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MR"))
                    {
                        result = result.Where(x => x.course_code == viewScheduleRequest.course_code && x.subject_name.StartsWith("P.E"));
                    }
                    else if (viewScheduleRequest.course_code.Contains("MM") || viewScheduleRequest.course_code.Contains("MV"))
                    {
                        result = result.Where(x => (x.course_code.Contains("MM") || x.course_code.Contains("MV")) && x.subject_name.StartsWith("P.E"));
                    }
                    else
                    {
                        result = result.Where(x => !courseCodeArray.Contains(x.course_code) && x.subject_name.StartsWith("PE"));
                    }
                }
                else
                {
                    result = result.Where(x => x.course_code == viewScheduleRequest.course_code);
                }
            }

            if (!viewScheduleRequest.year_level.Equals(String.Empty) && viewScheduleRequest.year_level > 0)
            {
                if (viewScheduleRequest.year_level != 9)
                {
                    var mdnS = viewScheduleRequest.year_level % 10;
                    var ban = false;
                    var norm = false;
                    var yearEleven = false;
                    var nstpPeFilter = false;

                    if (viewScheduleRequest.no_nstp > 0 && viewScheduleRequest.no_pe > 0)
                    {
                        nstpPeFilter = true;
                    }

                    if (viewScheduleRequest.year_level / 10 == 111)
                    {
                        result = result.Where(x => x.section.Substring(2, 1).Equals("1"));
                    }
                    else if (viewScheduleRequest.year_level / 10 == 121)
                    {
                        result = result.Where(x => x.section.Substring(2, 1).Equals("2"));
                    }
                    else if (viewScheduleRequest.year_level / 10 == 112)
                    {
                        yearEleven = true;
                        ban = true;
                        result = result.Where(x => x.section.Substring(1, 1).Equals("-"));
                    }
                    else if (viewScheduleRequest.year_level / 10 == 122)
                    {
                        ban = true;
                        result = result.Where(x => x.section.Substring(2, 1).Equals("-"));
                    }
                    else if (viewScheduleRequest.no_nstp > 0 && viewScheduleRequest.no_pe > 0)
                    {
                        result = result.Where(x => x.subject_name.Contains("PE 101") || x.subject_name.Contains("P.E 111") || x.subject_name.Contains("P.E. 111 L") || x.subject_name.Contains("NSTP 101"));
                    }
                    else if (viewScheduleRequest.no_nstp > 0)
                    {
                        result = result.Where(x => x.subject_name.Contains("NSTP 101"));
                    }
                    else if (viewScheduleRequest.no_pe > 0)
                    {
                        result = result.Where(x => x.subject_name.Contains("PE 101") || x.subject_name.Contains("P.E 111") || x.subject_name.Contains("P.E. 111 L"));
                    }
                    else
                    {
                        norm = true;
                        result = result.Where(x => x.section.Contains(viewScheduleRequest.year_level.ToString()));
                    }

                    if (!norm && !nstpPeFilter)
                    {
                        //mdn lookup
                        if (mdnS == 8)
                        {
                            if (ban)
                            {
                                if (yearEleven)
                                {
                                    result = result.Where(x => x.section.Substring(2, 1).Equals("A"));
                                }
                                else
                                {
                                    result = result.Where(x => x.section.Substring(3, 1).Equals("A"));
                                }
                            }
                            else
                            {
                                result = result.Where(x => x.section.Substring(3, 1).Equals("A"));
                            }
                        }
                        else
                        {
                            if (ban)
                            {
                                if (yearEleven)
                                {
                                    result = result.Where(x => x.section.Substring(2, 1).Equals("P"));
                                }
                                else
                                {
                                    result = result.Where(x => x.section.Substring(3, 1).Equals("P"));
                                }
                            }
                            else
                            {
                                result = result.Where(x => x.section.Substring(3, 1).Equals("P"));
                            }
                        }
                    }
                }
            }

            if (viewScheduleRequest.edp_codes.Count > 0)
            {
                result = result.Where(x => viewScheduleRequest.edp_codes.Contains(x.edpcode));
            }

            if (!viewScheduleRequest.subject_name.Equals(String.Empty))
            {
                result = result.Where(x => x.subject_name.Contains(viewScheduleRequest.subject_name));
            }

            if (!viewScheduleRequest.section.Equals(String.Empty))
            {
                result = result.Where(x => x.section == viewScheduleRequest.section);
            }

            if (!viewScheduleRequest.status.Equals(String.Empty) && viewScheduleRequest.status != 9)
            {
                result = result.Where(x => x.status == viewScheduleRequest.status);
            }

            var count = result.Count();

            if (viewScheduleRequest.page != 0 && viewScheduleRequest.limit != 0)
            {
                result = result.OrderBy(x => x.section).ThenBy(x => x.course_code).ThenBy(x => x.edpcode).Skip(skip).Take(take);
            }

            return new ViewScheduleResponse { schedules = result.ToList(), count = count };
        }

        /*
         * Method to View Studyload
        */
        public GetStudyLoadResponse GetStudyLoad(GetStudyLoadRequest getRequest)
        {
            //Get user data
            var studyLoad = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == getRequest.id_number);

            //check if the the data exist
            if (studyLoad == null)
            {
                //return empty data
                return new GetStudyLoadResponse { };
            }
            else
            {
                //Get data from _212ostsp and _212schedules
                var result = (from _212ostsp in _ucOnlinePortalContext._212ostsps
                              join _212schedules in _ucOnlinePortalContext._212schedules
                              on _212ostsp.EdpCode equals _212schedules.EdpCode
                              join _subject_info in _ucOnlinePortalContext.SubjectInfos
                              on _212schedules.InternalCode equals _subject_info.InternalCode into sched
                              from _subject_info in sched.DefaultIfEmpty()
                              join _courselist in _ucOnlinePortalContext.CourseLists
                              on _212schedules.CourseCode equals _courselist.CourseCode into course
                              from _courselist in course.DefaultIfEmpty()
                              where _212ostsp.StudId == getRequest.id_number
                              && _212ostsp.Status != 2
                              select new GetStudyLoadResponse.Schedules
                              {
                                  edp_code = _212schedules.EdpCode,
                                  subject_name = _212schedules.Description,
                                  subject_type = _212schedules.SubType,
                                  days = _212schedules.Days,
                                  begin_time = _212schedules.TimeStart,
                                  end_time = _212schedules.TimeEnd,
                                  mdn = _212schedules.Mdn,
                                  m_begin_time = _212schedules.MTimeStart,
                                  m_end_time = _212schedules.MTimeEnd,
                                  m_days = _212schedules.MDays,
                                  size = _212schedules.Size,
                                  max_size = _212schedules.MaxSize,
                                  units = _212schedules.Units,
                                  room = _212schedules.Room,
                                  descriptive_title = _subject_info.Descr1 + _subject_info.Descr2,
                                  split_code = _212schedules.SplitCode,
                                  split_type = _212schedules.SplitType,
                                  section = _212schedules.Section,
                                  course_abbr = _courselist.CourseAbbr
                              }).ToList();


                var has_pe_v = result.Where(x => x.subject_name.StartsWith("PE") || x.subject_name.StartsWith("P.E") || x.subject_name.StartsWith("DEFTACT")).Count() > 0 ? 1 : 0;
                var has_nstp_v = result.Where(x => x.subject_name.StartsWith("NSTP")).Count() > 0 ? 1 : 0;

                var getDept = studyLoad.FirstOrDefault();

                string[] exempted = { "JD", "JT", "PD" };

                if (getDept != null)
                {
                    if (getDept.Dept.Equals("SH"))
                    {
                        has_nstp_v = 1;
                        has_pe_v = 1;
                    }
                    if (getDept.YearLevel == 1 && exempted.Contains(getDept.CourseCode))
                    {
                        has_nstp_v = 1;
                        has_pe_v = 1;
                    }
                    if (getDept.YearLevel > 1)
                    {
                        has_nstp_v = 1;
                    }
                    if (getDept.YearLevel > 2)
                    {
                        has_nstp_v = 1;
                        has_pe_v = 1;
                    }
                    if (getDept.YearLevel == 1 && getDept.CourseCode.Equals("PN"))
                    {
                        has_nstp_v = 1;
                    }
                    if (getDept.YearLevel == 2 && getDept.CourseCode.Equals("HM"))
                    {
                        has_nstp_v = 1;
                        has_pe_v = 1;
                    }
                }


                //return studyload response
                return new GetStudyLoadResponse { schedules = result, has_pe = has_pe_v, has_nstp = has_nstp_v };
            }
        }

        /*
         * Method to Get Student Status
        */
        public GetStudentStatusResponse GetStudentStatus(GetStudentStatusRequest getStudentStatusRequest)
        {
            //Get student enrollment data
            var studentStatus = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == getStudentStatusRequest.id_number).FirstOrDefault();

            //create list holder
            List<GetStudentStatusResponse.Status> status = new List<GetStudentStatusResponse.Status>();

            if (studentStatus != null)
            {
                //Loop to 8 steps and build each step's result
                for (int counter = 0; counter < 8; counter++)
                {
                    GetStudentStatusResponse.Status stat = new GetStudentStatusResponse.Status();

                    stat.done = 0;
                    stat.step = counter + 1;

                    switch (counter)
                    {
                        case 0:
                            {
                                if (studentStatus.Status >= 0)
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.RegisteredOn.ToString();
                                    stat.approved = 1;
                                }
                            }
                            break;
                        case 1:
                            {
                                if ((studentStatus.ApprovedRegRegistrar != null && studentStatus.ApprovedRegRegistrar.Trim().Length > 0) || (studentStatus.DisapprovedRegRegistrar != null && studentStatus.DisapprovedRegRegistrar.Trim().Length > 0))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.DisapprovedRegRegistrar != null ? studentStatus.DisapprovedRegRegistrarOn.ToString() : studentStatus.ApprovedRegRegistrarOn.ToString();
                                    stat.approved = studentStatus.DisapprovedRegRegistrar != null ? 0 : 1;
                                }
                            }
                            break;
                        case 2:
                            {
                                if ((studentStatus.ApprovedRegDean != null && studentStatus.ApprovedRegDean.Trim().Length > 0) || (studentStatus.DisapprovedRegDean != null && studentStatus.DisapprovedRegDean.Trim().Length > 0))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.DisapprovedRegDean != null ? studentStatus.DisapprovedRegDeanOn.ToString() : studentStatus.ApprovedRegDeanOn.ToString();
                                    stat.approved = studentStatus.DisapprovedRegDean != null ? 0 : 1;
                                }
                            }
                            break;
                        case 3:
                            {
                                if (!studentStatus.SubmittedOn.ToString().Equals(String.Empty))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.SubmittedOn.ToString();
                                    stat.approved = 1;
                                }
                            }
                            break;
                        case 4:
                            {
                                if ((studentStatus.ApprovedDean != null && studentStatus.ApprovedDean.Trim().Length > 0) || (studentStatus.DisapprovedDean != null && studentStatus.DisapprovedDean.Trim().Length > 0))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.DisapprovedDean != null ? studentStatus.DisapprovedDeanOn.ToString() : studentStatus.ApprovedDeanOn.ToString();
                                    stat.approved = studentStatus.DisapprovedDean != null ? 0 : 1;
                                }
                            }
                            break;
                        case 5:
                            {
                                if (!studentStatus.ApprovedAcctgOn.ToString().Equals(String.Empty))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.ApprovedAcctgOn.ToString();
                                    stat.approved = 1;
                                }
                            }
                            break;
                        case 6:
                            {
                                if (!studentStatus.ApprovedCashierOn.ToString().Equals(String.Empty))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.ApprovedCashierOn.ToString();
                                    stat.approved = 1;
                                }
                            }
                            break;
                        case 7:
                            {
                                if (!studentStatus.ApprovedCashierOn.ToString().Equals(String.Empty))
                                {
                                    stat.done = 1;
                                    stat.date = studentStatus.ApprovedCashierOn.ToString();
                                    stat.approved = 1;
                                }
                            }
                            break;
                    }
                    status.Add(stat);
                }
            }

            var clasify = studentStatus == null ? "" : studentStatus.Classification;

            int is_cancelled = 0;
            string neededPayment = "0";
            int pending_promissory = 0;
            int promise_pay = 0;

            var openAdjustment = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == studentStatus.CourseCode && x.AdjustmentStart >= DateTime.Now && x.AdjustmentStart <= DateTime.Now).Count();
            short? enrollmentOpen = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == studentStatus.CourseCode).Select(x => x.EnrollmentOpen).FirstOrDefault();

            if (studentStatus != null)
            {
                is_cancelled = studentStatus.Status == 13 ? 1 : 0;
                neededPayment = studentStatus.NeededPayment == null ? "0" : studentStatus.NeededPayment.ToString();
                pending_promissory = studentStatus.RequestPromissory != 0 && studentStatus.RequestPromissory != 3 ? 1 : 0;
                promise_pay = studentStatus.PromiPay.Value;
            }

            return new GetStudentStatusResponse { status = status, classification = clasify, is_cancelled = is_cancelled, needed_payment = neededPayment, pending_promissory = pending_promissory, promi_pay = promise_pay, adjustment_open = openAdjustment, enrollment_open = enrollmentOpen.Value };
        }

        /*
        * Method to View List
        */

        public ViewStudentPerStatusResponse ViewStudentStatus(ViewStudentPerStatusRequest viewStudentPerStatusRequest)
        {
            //settings for pagination
            int take = (int)viewStudentPerStatusRequest.limit;
            int skip = (int)viewStudentPerStatusRequest.limit * ((int)viewStudentPerStatusRequest.page - 1);

            //populate initial response object
            var result = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                          join _loginInfo in _ucOnlinePortalContext.LoginInfos
                          on _212oenrp.StudId equals _loginInfo.StudId
                          join _courseList in _ucOnlinePortalContext.CourseLists
                          on _212oenrp.CourseCode equals _courseList.CourseCode
                          //join _attach in _ucOnlinePortalContext._212attachments
                          //on _212oenrp.StudId equals _attach.StudId into gattach
                          //from _attach in gattach.DefaultIfEmpty()
                          //where _212oenrp.Status == viewStudentPerStatusRequest.status
                          select new ViewStudentPerStatusResponse.Student
                          {
                              id_number = _212oenrp.StudId,
                              lastname = _loginInfo.LastName,
                              firstname = _loginInfo.FirstName,
                              mi = _loginInfo.Mi,
                              suffix = _loginInfo.Suffix,
                              classification = Utils.Function.getClassification(_212oenrp.Classification),
                              classification_abbr = _212oenrp.Classification,
                              course_year = _courseList.CourseAbbr + " " + _212oenrp.YearLevel,
                              course_code = _212oenrp.CourseCode,
                              status = _212oenrp.Status,
                              year_level = _212oenrp.YearLevel,
                              submitted_on = _212oenrp.SubmittedOn,
                              registered_on = _212oenrp.RegisteredOn,
                              approved_reg_registrar = _212oenrp.ApprovedRegRegistrar,
                              approved_reg_registrar_on = _212oenrp.ApprovedRegRegistrarOn,
                              disapproved_reg_registrar = _212oenrp.DisapprovedRegRegistrar,
                              disaproved_reg_registrar_on = _212oenrp.DisapprovedRegRegistrarOn,
                              approved_dean_reg = _212oenrp.ApprovedRegDean,
                              approved_dean_reg_on = _212oenrp.ApprovedRegDeanOn,
                              disapproved_reg_dean = _212oenrp.DisapprovedRegDean,
                              disapproved_reg_dean_on = _212oenrp.DisapprovedRegDeanOn,
                              approved_dean = _212oenrp.ApprovedDean,
                              approved_dean_on = _212oenrp.ApprovedDeanOn,
                              disapproved_dean = _212oenrp.DisapprovedDean,
                              disapproved_dean_on = _212oenrp.DisapprovedDeanOn,
                              approved_accounting = _212oenrp.ApprovedAcctg,
                              approved_accounting_on = _212oenrp.ApprovedAcctgOn,
                              approved_cashier = _212oenrp.ApprovedCashier,
                              approved_cashier_on = _212oenrp.ApprovedCashierOn,
                              request_deblock = (short)_212oenrp.RequestDeblock,
                              request_overload = (short)_212oenrp.RequestOverload,
                              needed_payment = (int)_212oenrp.NeededPayment,
                              promi_pay = (int)_212oenrp.PromiPay,
                              has_payment = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == _212oenrp.StudId && x.Type.Equals("Payment")).Take(1).Count(),
                              has_promissory = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == _212oenrp.StudId && x.RequestPromissory == 3).Count(),
                              profile = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == _212oenrp.StudId && x.Type == "2x2 ID Picture").Select(x => x.Filename).FirstOrDefault(),
                              enrollmentDate = _212oenrp.EnrollmentDate
                          });

            if (viewStudentPerStatusRequest.status == 99)
            {
                //do nothing
            }
            else if (viewStudentPerStatusRequest.status == 98)
            {
                int[] statStudyload = { 6, 8, 9, 10 };

                result = result.Where(x => statStudyload.Contains((int)x.status));
            }
            else
            {
                if (viewStudentPerStatusRequest.status == 6)
                {
                    if (viewStudentPerStatusRequest.course_department.Equals(String.Empty))
                    {
                        result = result.Where(x => (int)x.status == viewStudentPerStatusRequest.status);
                    }
                    else
                    {
                        result = result.Where(x => x.approved_dean_on != null);
                    }
                }
                else if (viewStudentPerStatusRequest.status == 7)
                {
                    result = result.Where(x => x.disapproved_dean_on != null);
                }
                else if (viewStudentPerStatusRequest.status == 8)
                {
                    if (viewStudentPerStatusRequest.is_cashier != null && viewStudentPerStatusRequest.is_cashier == 1)
                    {
                        result = result.Where(x => (int)x.status == 8);

                        var countCashier = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                                            join _attachment in _ucOnlinePortalContext._212attachments
                                            on _212oenrp.StudId equals _attachment.StudId
                                            where _attachment.AttachmentId == (from _attach in _ucOnlinePortalContext._212attachments
                                                                               where _attach.StudId == _attachment.StudId
                                                                               && _attach.Type.Equals("Payment")
                                                                               orderby _attach.AttachmentId
                                                                               select _attach.AttachmentId).FirstOrDefault()
                                                                               && _212oenrp.Status == 8
                                            select _212oenrp.StudId).ToList();

                        result = result.Where(x => countCashier.Contains(x.id_number));
                    }
                    else
                    {
                        result = result.Where(x => x.approved_accounting_on != null);
                    }
                }
                else
                {
                    result = result.Where(x => (int)x.status == viewStudentPerStatusRequest.status);
                }
            }

            //if status stage requires filtering of courses, add another filter
            if (viewStudentPerStatusRequest.status == 1 || viewStudentPerStatusRequest.status == 3 || viewStudentPerStatusRequest.status == 4 || viewStudentPerStatusRequest.status == 5 || viewStudentPerStatusRequest.status == 6 || viewStudentPerStatusRequest.status == 7 || viewStudentPerStatusRequest.status == 10 || viewStudentPerStatusRequest.status == 99)
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == viewStudentPerStatusRequest.course_department).ToList();

                if (viewStudentPerStatusRequest.status == 10 && viewStudentPerStatusRequest.course_department.Equals(String.Empty) && viewStudentPerStatusRequest.course.Equals(String.Empty))
                {
                    //do nothing
                }
                else if (viewStudentPerStatusRequest.course_department.Equals(String.Empty) && viewStudentPerStatusRequest.course.Equals(String.Empty))
                {
                    //do nothing
                }
                else if (viewStudentPerStatusRequest.course.Equals(String.Empty))
                {
                    var courses = courseList.Select(x => x.CourseCode).ToList();
                    result = result.Where(x => courses.Contains(x.course_code));
                }
                else
                {
                    result = result.Where(x => x.course_code == viewStudentPerStatusRequest.course);
                }
            }
            else
            {
                if (!viewStudentPerStatusRequest.course.Equals(String.Empty))
                {
                    result = result.Where(x => x.course_code == viewStudentPerStatusRequest.course);
                }
            }

            //searching options
            if (!viewStudentPerStatusRequest.id_number.Equals(String.Empty))
            {
                result = result.Where(x => x.id_number == viewStudentPerStatusRequest.id_number);
            }
            if (!viewStudentPerStatusRequest.name.Equals(String.Empty) && !viewStudentPerStatusRequest.date.Equals(String.Empty))
            {
                result = result.Where(x => (x.firstname + "" + x.lastname).Contains(viewStudentPerStatusRequest.name) && DateTime.Parse(viewStudentPerStatusRequest.date + " 00:00:00") <= x.registered_on && DateTime.Parse(viewStudentPerStatusRequest.date + " 23:59:59") >= x.registered_on);
            }
            if (!viewStudentPerStatusRequest.name.Equals(String.Empty))
            {
                result = result.Where(x => (x.firstname + "" + x.lastname).Contains(viewStudentPerStatusRequest.name));
            }
            if (!viewStudentPerStatusRequest.date.Equals(String.Empty))
            {
                result = result.Where(x => DateTime.Parse(viewStudentPerStatusRequest.date + " 00:00:00") <= x.registered_on && DateTime.Parse(viewStudentPerStatusRequest.date + " 23:59:59") >= x.registered_on);
            }
            if (viewStudentPerStatusRequest.year_level != 0)
            {
                result = result.Where(x => x.year_level == viewStudentPerStatusRequest.year_level);
            }
            if (!viewStudentPerStatusRequest.classification.Equals(String.Empty))
            {
                result = result.Where(x => x.classification_abbr == viewStudentPerStatusRequest.classification);
            }

            var count = result.Count();

            if (viewStudentPerStatusRequest.page != 0 && viewStudentPerStatusRequest.limit != 0)
            {
                result = result.OrderBy(x => x.id_number).Skip(skip).Take(take);
            }

            return new ViewStudentPerStatusResponse { students = result.ToList(), count = count };
        }


        /*
        * Method to View List
        */

        public ViewStudentRegistrationResponse ViewRegistration(ViewStudentRegistrationRequest viewStudentRegistrationRequest)
        {
            //get data from different tables
            var studentinfo = _ucOnlinePortalContext._212studentInfos.Where(x => x.StudId == viewStudentRegistrationRequest.id_number).FirstOrDefault();
            var contactInfo = _ucOnlinePortalContext._212contactAddresses.Where(x => x.StudInfoId == studentinfo.StudInfoId).FirstOrDefault();
            var familyInfo = _ucOnlinePortalContext._212familyInfos.Where(x => x.StudInfoId == studentinfo.StudInfoId).FirstOrDefault();
            var schooInfo = _ucOnlinePortalContext._212schoolInfos.Where(x => x.StudInfoId == studentinfo.StudInfoId).FirstOrDefault(); ;
            var loginInfo = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == viewStudentRegistrationRequest.id_number).FirstOrDefault();
            var studOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == viewStudentRegistrationRequest.id_number).FirstOrDefault();

            var attachmentList = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == viewStudentRegistrationRequest.id_number && x.Type != "Payment").ToList();

            if (viewStudentRegistrationRequest.payment != null && viewStudentRegistrationRequest.payment == 1)
            {
                attachmentList = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == viewStudentRegistrationRequest.id_number && x.Type.Equals("Payment")).Take(1).ToList();
            }
            var course = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == studentinfo.CourseCode).FirstOrDefault();


            List<ViewStudentRegistrationResponse.attachment> attach = new List<ViewStudentRegistrationResponse.attachment>();
            attach = attachmentList.Select(x => new ViewStudentRegistrationResponse.attachment
            {
                attachment_id = x.AttachmentId,
                email = x.Email,
                filename = x.Filename,
                id_number = x.StudId,
                type = x.Type
            }).ToList();

            //populate data
            ViewStudentRegistrationResponse registrationResponse = new ViewStudentRegistrationResponse
            {
                stud_id = studentinfo.StudId,
                allowed_units = loginInfo.AllowedUnits.HasValue ? (int)loginInfo.AllowedUnits : 0,
                course = course.CourseAbbr,
                college = course.CourseDepartment,
                course_code = studOenrp.CourseCode,
                assigned_section = studOenrp.Section,
                year_level = studentinfo.YearLevel,
                mdn = studentinfo.Mdn,
                first_name = studentinfo.FirstName,
                middle_name = studentinfo.MiddleName,
                last_name = studentinfo.LastName,
                suffix = studentinfo.Suffix,
                gender = studentinfo.Gender,
                status = studentinfo.Status,
                nationality = studentinfo.Nationality,
                birthdate = studentinfo.BirthDate.ToString(),
                birthplace = studentinfo.BirthPlace,
                religion = studentinfo.Religion,
                start_term = studentinfo.StartTerm,
                is_verified = (short)loginInfo.IsVerified,
                classification = studentinfo.Classification,
                dept = studentinfo.Dept,
                pcountry = contactInfo.PCountry,
                pprovince = contactInfo.PProvince,
                pcity = contactInfo.PCity,
                pbarangay = contactInfo.PBarangay,
                pstreet = contactInfo.PStreet,
                pzip = contactInfo.PZip,
                cprovince = contactInfo.CProvince,
                ccity = contactInfo.CCity,
                cbarangay = contactInfo.CBarangay,
                cstreet = contactInfo.CStreet,
                mobile = contactInfo.Mobile,
                landline = contactInfo.Landline,
                email = contactInfo.Email,
                facebook = contactInfo.Facebook,
                father_name = familyInfo.FatherName,
                father_contact = familyInfo.FatherContact,
                father_occupation = familyInfo.FatherOccupation,
                mother_name = familyInfo.MotherName,
                mother_contact = familyInfo.MotherContact,
                mother_occupation = familyInfo.MotherOccupation,
                guardian_name = familyInfo.GuardianName,
                guardian_contact = familyInfo.GuardianContact,
                guardian_occupation = familyInfo.GuardianOccupation,
                elem_name = schooInfo.ElemName,
                elem_year = schooInfo.ElemYear,
                elem_last_year = schooInfo.ElemLastYear.HasValue ? (short)schooInfo.ElemLastYear : 0,
                elem_type = schooInfo.ElemType,
                elem_lrn_number = schooInfo.ElemLrnNo,
                elem_esc_school_id = schooInfo.ElemEscSchoolId,
                elem_esc_student_id = schooInfo.ElemEscStudentId,
                sec_name = schooInfo.SecName,
                sec_year = schooInfo.SecYear,
                sec_last_year = schooInfo.SecLastYear.HasValue ? (short)schooInfo.SecLastYear : 0,
                sec_type = schooInfo.SecType,
                sec_lrn_number = schooInfo.SecLrnNo,
                sec_esc_school_id = schooInfo.SecEscSchoolId,
                sec_esc_student_id = schooInfo.SecEscStudentId,
                col_name = schooInfo.ColName,
                col_year = schooInfo.ColYear,
                col_course = schooInfo.ColCourse,
                col_last_year = schooInfo.ColLastYear.HasValue ? (short)schooInfo.ColLastYear : 0,
                attachments = attach,
                request_overload = (short)studOenrp.RequestOverload,
                request_deblock = (short)studOenrp.RequestDeblock
            };

            return registrationResponse;
        }

        /*
        * Method to Set Approve or Disapprove
        */

        public SetApproveOrDisapprovedResponse SetApproveOrDisapprove(SetApproveOrDisapprovedRequest setApproveOrDisapprovedRequest)
        {
            var studentOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number).FirstOrDefault();
            var studentLogin = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number).FirstOrDefault();
            var studentInfo = _ucOnlinePortalContext._212studentInfos.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number).FirstOrDefault();
            var notification = _ucOnlinePortalContext._212notifications.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number);
            var studentOstp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number);
            var studentAttachment = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number);

            String final_id = setApproveOrDisapprovedRequest.id_number;
            _212notification newNotif = new _212notification();
            List<_212schedule> schedules = new List<_212schedule>();
            bool isFull = false;

            if (setApproveOrDisapprovedRequest.status == 1)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.ApprovedRegRegistrar = setApproveOrDisapprovedRequest.name_of_approver;
                studentOenrp.ApprovedRegRegistrarOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                if (!setApproveOrDisapprovedRequest.year_level.Equals(String.Empty))
                {
                    studentOenrp.YearLevel = (short)setApproveOrDisapprovedRequest.year_level;
                }
                if (!setApproveOrDisapprovedRequest.classification.Equals(String.Empty))
                {
                    studentOenrp.Classification = setApproveOrDisapprovedRequest.classification;
                }
                if (!setApproveOrDisapprovedRequest.allowed_units.Equals(String.Empty))
                {
                    studentLogin.AllowedUnits = (short)setApproveOrDisapprovedRequest.allowed_units;
                }

                _ucOnlinePortalContext.LoginInfos.Update(studentLogin);

                //Check classification 
                // if classification is old, update id number
                if (setApproveOrDisapprovedRequest.classification.Equals("O"))
                {
                    if (!setApproveOrDisapprovedRequest.existing_id_number.Equals(""))
                    {
                        //get the old loginfo
                        var toDelete = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == setApproveOrDisapprovedRequest.existing_id_number).FirstOrDefault();

                        //delete the old loginfo
                        if (toDelete != null)
                        {
                            _ucOnlinePortalContext.LoginInfos.Remove(toDelete);
                        }

                        //update student id
                        final_id = setApproveOrDisapprovedRequest.existing_id_number;
                        studentOenrp.StudId = setApproveOrDisapprovedRequest.existing_id_number;
                        studentLogin.StudId = setApproveOrDisapprovedRequest.existing_id_number;
                        studentInfo.StudId = setApproveOrDisapprovedRequest.existing_id_number;

                        if (studentAttachment != null)
                        {
                            studentAttachment.ToList().ForEach(x => x.StudId = setApproveOrDisapprovedRequest.existing_id_number);
                        }

                        notification.ToList().ForEach(x => x.StudId = setApproveOrDisapprovedRequest.existing_id_number);
                    }
                }
                else if (setApproveOrDisapprovedRequest.classification.Equals("H"))
                {
                    //create new id number
                    var config = _ucOnlinePortalContext.Configs.FirstOrDefault();

                    //fill with 0 those empty values
                    string sequence = config.Sequence.ToString();
                    sequence = sequence.PadLeft(4, '0');

                    StringBuilder idnumber = new StringBuilder();
                    idnumber.Append(config.IdYear);
                    idnumber.Append(config.CampusId);
                    idnumber.Append(sequence);

                    string idNumber = Utils.Function.Modulo10(idnumber.ToString());

                    config.Sequence = config.Sequence + 1;

                    //update sequence in config
                    _ucOnlinePortalContext.Configs.Update(config);

                    //update student id
                    final_id = idNumber;
                    studentOenrp.StudId = idNumber;
                    studentLogin.StudId = idNumber;
                    studentInfo.StudId = idNumber;

                    if (studentAttachment != null)
                    {
                        studentAttachment.ToList().ForEach(x => x.StudId = final_id);
                    }

                    notification.ToList().ForEach(x => x.StudId = idNumber);
                }

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.APPROVED_REGISTRATION_REGISTRAR + ". Your ID Number is : " + final_id
                };
            }
            else if (setApproveOrDisapprovedRequest.status == 2)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.DisapprovedRegRegistrar = setApproveOrDisapprovedRequest.name_of_approver;
                studentOenrp.DisapprovedRegRegistrarOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                newNotif = new _212notification
                {
                    StudId = setApproveOrDisapprovedRequest.id_number,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.DISAPPROVED_REGISTRATION_REGISTRAR + " " + setApproveOrDisapprovedRequest.message
                };

                _ucOnlinePortalContext._212notifications.Add(newNotif);
            }
            else if (setApproveOrDisapprovedRequest.status == 3)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.ApprovedRegDean = setApproveOrDisapprovedRequest.name_of_approver;
                studentOenrp.ApprovedRegDeanOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                final_id = setApproveOrDisapprovedRequest.id_number;

                if (setApproveOrDisapprovedRequest.id_number.Length != 8)
                {
                    //create new id number
                    var config = _ucOnlinePortalContext.Configs.FirstOrDefault();

                    //fill with 0 those empty values
                    string sequence = config.Sequence.ToString();
                    sequence = sequence.PadLeft(4, '0');

                    StringBuilder idnumber = new StringBuilder();
                    idnumber.Append(config.IdYear);
                    idnumber.Append(config.CampusId);
                    idnumber.Append(sequence);

                    string idNumber = Utils.Function.Modulo10(idnumber.ToString());

                    config.Sequence = config.Sequence + 1;

                    //update sequence in config
                    _ucOnlinePortalContext.Configs.Update(config);

                    //update student id
                    final_id = idNumber;
                    studentOenrp.StudId = idNumber;
                    studentLogin.StudId = idNumber;
                    studentInfo.StudId = idNumber;

                    if (studentAttachment != null)
                    {
                        studentAttachment.ToList().ForEach(x => x.StudId = final_id);
                    }

                    notification.ToList().ForEach(x => x.StudId = idNumber);
                }

                if (!setApproveOrDisapprovedRequest.section.Equals(String.Empty))
                {
                    studentOenrp.Section = setApproveOrDisapprovedRequest.section;

                    //Insert OSTSP if section is set by dean
                    schedules = _ucOnlinePortalContext._212schedules.Where(x => x.Section == setApproveOrDisapprovedRequest.section && x.CourseCode == setApproveOrDisapprovedRequest.course_code).ToList();

                    if (setApproveOrDisapprovedRequest.course_code.Equals("MM") || setApproveOrDisapprovedRequest.course_code.Equals("MV"))
                    {
                        schedules = _ucOnlinePortalContext._212schedules.Where(x => x.Section == setApproveOrDisapprovedRequest.section && x.CourseCode == setApproveOrDisapprovedRequest.course_code && x.Status == 1).ToList();
                    }

                    var schedFull = schedules.Where(x => x.Size == x.MaxSize).ToList();

                    if (schedFull.Count > 0)
                    {
                        schedules = schedules.Where(x => x.Size == x.MaxSize).ToList();
                        isFull = true;
                    }
                    else
                    {
                        foreach (_212schedule sched in schedules)
                        {
                            _212ostsp newStudentOstsp = new _212ostsp
                            {
                                StudId = final_id,
                                EdpCode = sched.EdpCode,
                                Status = 0,
                                Remarks = null,
                                AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                            };

                            var scheduleAdd = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == sched.EdpCode).FirstOrDefault();
                            scheduleAdd.Size += 1;
                            scheduleAdd.PendingEnrolled += 1;
                            _ucOnlinePortalContext._212schedules.Update(scheduleAdd);

                            //Save OSTSP
                            _ucOnlinePortalContext._212ostsps.Add(newStudentOstsp);
                            _ucOnlinePortalContext.SaveChanges();
                        }
                    }
                }
                if (!setApproveOrDisapprovedRequest.year_level.Equals(String.Empty) && setApproveOrDisapprovedRequest.year_level > 0)
                {
                    studentLogin.Year = (short)setApproveOrDisapprovedRequest.year_level;
                    studentOenrp.YearLevel = (short)setApproveOrDisapprovedRequest.year_level;
                }
                if (!setApproveOrDisapprovedRequest.classification.Equals(String.Empty))
                {
                    studentOenrp.Classification = setApproveOrDisapprovedRequest.classification;
                }
                if (!setApproveOrDisapprovedRequest.allowed_units.Equals(String.Empty))
                {
                    studentLogin.AllowedUnits = (short)setApproveOrDisapprovedRequest.allowed_units;
                }

                if (!setApproveOrDisapprovedRequest.mdn.Equals(String.Empty))
                {
                    if (studentInfo != null)
                    {
                        studentInfo.Mdn = setApproveOrDisapprovedRequest.mdn;
                    }
                    studentOenrp.Mdn = setApproveOrDisapprovedRequest.mdn;
                }

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.APPROVED_REGISTRATION_DEAN + ". Your ID Number is : " + final_id
                };

                if (studentOenrp.Dept.Equals("SH"))
                {
                    newNotif = new _212notification
                    {
                        StudId = final_id,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.APPROVED_REGISTRATION_DEAN + ". Additional Instructions: " + setApproveOrDisapprovedRequest.message + ". Your ID Number is : " + final_id
                    };
                }

            }
            else if (setApproveOrDisapprovedRequest.status == 4)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.DisapprovedRegDean = setApproveOrDisapprovedRequest.name_of_approver;
                studentOenrp.DisapprovedRegDeanOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.DISAPPROVED_REGISTRATION_DEAN + " " + setApproveOrDisapprovedRequest.message
                };
            }
            else if (setApproveOrDisapprovedRequest.status == 5)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.SubmittedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                studentOenrp.EnrollmentDate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.SELECTING_SUBJECTS
                };
            }
            else if (setApproveOrDisapprovedRequest.status == 6)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;
                //Insert OSTSP if section is set by dean
                var ostsp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number && x.Status != 2).Select(x => x.EdpCode).ToList();

                schedules = _ucOnlinePortalContext._212schedules.Where(x => ostsp.Contains(x.EdpCode) && x.Size == x.MaxSize).ToList();

                if (schedules.Count > 0)
                {
                    isFull = true;
                }
                else
                {
                    var schedulesAdd = _ucOnlinePortalContext._212schedules.Where(x => ostsp.Contains(x.EdpCode) && x.Status != 2);
                    schedulesAdd.ToList().ForEach(x => x.Size = (x.Size + 1));
                    schedulesAdd.ToList().ForEach(x => x.PendingEnrolled = (x.PendingEnrolled.Value + 1));

                    _ucOnlinePortalContext.SaveChanges();

                    //Insert OSTSP if section is set by dean
                    var schedulesOstp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number && x.Status != 2);

                    schedulesOstp.ToList().ForEach(x => x.Status = 1);
                    schedulesOstp.ToList().ForEach(x => x.AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                    _ucOnlinePortalContext.SaveChanges();

                    studentOenrp.ApprovedDean = setApproveOrDisapprovedRequest.name_of_approver;
                    studentOenrp.ApprovedDeanOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    newNotif = new _212notification
                    {
                        StudId = final_id,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.APPROVED_BY_DEAN
                    };

                    _ucOnlinePortalContext._212notifications.Add(newNotif);

                    //If student is Accounting, Auto Approve!
                    if (!setApproveOrDisapprovedRequest.classification.Equals(String.Empty))
                    {
                        if (setApproveOrDisapprovedRequest.classification.Equals("H"))
                        {
                            _ucOnlinePortalContext.SaveChanges();

                            studentOenrp.ApprovedAcctg = "AUTO-APPROVE";
                            studentOenrp.ApprovedAcctgOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                            studentOenrp.NeededPayment = 500;

                            newNotif = new _212notification
                            {
                                StudId = final_id,
                                NotifRead = 0,
                                Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                                Message = Literals.APPROVED_BY_ACCOUNTING
                            };

                            studentOenrp.Status = 8;

                            _ucOnlinePortalContext._212notifications.Add(newNotif);
                            _ucOnlinePortalContext.SaveChanges();
                        }
                    }
                }
            }
            else if (setApproveOrDisapprovedRequest.status == 7)
            {
                studentOenrp.DisapprovedDeanOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.DISAPPROVED_BY_DEAN + " " + setApproveOrDisapprovedRequest.message
                };

                //set back status for selecting subject
                studentOenrp.Status = (short)RequestResponse.Enums.EnrollmentStatus.APPROVED_REGISTRATION_DEAN;
                studentOenrp.SubmittedOn = null;

            }
            else if (setApproveOrDisapprovedRequest.status == 8)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.ApprovedAcctg = setApproveOrDisapprovedRequest.name_of_approver;
                studentOenrp.ApprovedAcctgOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                studentOenrp.NeededPayment = setApproveOrDisapprovedRequest.needed_payment;

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.APPROVED_BY_ACCOUNTING
                };

                if (setApproveOrDisapprovedRequest.needed_payment <= 0)
                {
                    _ucOnlinePortalContext._212notifications.Add(newNotif);
                    _ucOnlinePortalContext.SaveChanges();

                    studentOenrp.ApprovedCashier = setApproveOrDisapprovedRequest.name_of_approver;
                    studentOenrp.ApprovedCashierOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    newNotif = new _212notification
                    {
                        StudId = final_id,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.APPROVED_BY_CASHIER
                    };

                    _ucOnlinePortalContext._212notifications.Add(newNotif);
                    _ucOnlinePortalContext.SaveChanges();

                    newNotif = new _212notification
                    {
                        StudId = final_id,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.OFFICIALLY_ENROLLED + ". Please check your email, we sent a copy of your studyload."
                    };

                    var ostsp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number && x.Status == 1);
                    ostsp.ToList().ForEach(x => x.Status = 3);
                    ostsp.ToList().ForEach(x => x.AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

                    var edpCode = ostsp.Select(x => x.EdpCode);

                    var schedulesUpdate = _ucOnlinePortalContext._212schedules.Where(x => edpCode.Contains(x.EdpCode) && x.Status != 2);
                    schedulesUpdate.ToList().ForEach(x => x.PendingEnrolled = (x.PendingEnrolled.Value - 1));
                    schedulesUpdate.ToList().ForEach(x => x.OfficialEnrolled = (x.OfficialEnrolled.Value + 1));

                    _ucOnlinePortalContext.SaveChanges();

                    studentOenrp.Status = 10;
                    setApproveOrDisapprovedRequest.status = 9;
                }

            }
            else if (setApproveOrDisapprovedRequest.status == 9)
            {
                studentOenrp.Status = (short)setApproveOrDisapprovedRequest.status;

                studentOenrp.ApprovedCashier = setApproveOrDisapprovedRequest.name_of_approver;
                studentOenrp.ApprovedCashierOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.APPROVED_BY_CASHIER
                };

                _ucOnlinePortalContext._212notifications.Add(newNotif);
                _ucOnlinePortalContext.SaveChanges();

                newNotif = new _212notification
                {
                    StudId = final_id,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.OFFICIALLY_ENROLLED + ". Please check your email, we sent a copy of your studyload."
                };

                var ostsp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == setApproveOrDisapprovedRequest.id_number && x.Status == 1);
                ostsp.ToList().ForEach(x => x.Status = 3);
                ostsp.ToList().ForEach(x => x.AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));

                var edpCode = ostsp.Select(x => x.EdpCode);

                var schedulesUpdate = _ucOnlinePortalContext._212schedules.Where(x => edpCode.Contains(x.EdpCode) && x.Status != 2);
                schedulesUpdate.ToList().ForEach(x => x.PendingEnrolled = (x.PendingEnrolled.Value - 1));
                schedulesUpdate.ToList().ForEach(x => x.OfficialEnrolled = (x.OfficialEnrolled.Value + 1));

                _ucOnlinePortalContext.SaveChanges();

                studentOenrp.Status = 10;
            }

            if (setApproveOrDisapprovedRequest.status == 9)
            {
                //Send Offial Enrollment and Studyload
                GetStudyLoadRequest getRequest = new GetStudyLoadRequest { id_number = final_id };
                GetStudyLoadResponse response = GetStudyLoad(getRequest);

                StringBuilder constructStudyload = new StringBuilder();

                constructStudyload.Append("<table>");
                constructStudyload.Append("<tr><th>EDP CODE</th><th>SUBJECT NAME</th><th>TYPE</th><th>TIME</th><th>DAYS</th><th>UNITS</th></tr>");

                if (response.schedules.Count > 0)
                {
                    foreach (GetStudyLoadResponse.Schedules sched in response.schedules)
                    {
                        constructStudyload.Append("<tr>");
                        constructStudyload.Append("<td>" + sched.edp_code + "</td>");
                        constructStudyload.Append("<td>" + sched.subject_name + "</td>");
                        constructStudyload.Append("<td>" + sched.subject_type + "</td>");
                        constructStudyload.Append("<td>" + sched.begin_time + " - " + sched.end_time + " " + sched.mdn + "</td>");
                        constructStudyload.Append("<td>" + sched.days + "</td>");
                        constructStudyload.Append("<td>" + sched.units + "</td>");
                        constructStudyload.Append("</tr>");
                    }
                }
                constructStudyload.Append("</table>");

                var Tk = Task.Run(() =>
                {
                    var emailDetails = new EmailDetails
                    {
                        To = new EmailAddress { Address = studentLogin.Email, Name = studentLogin.FirstName + " " + studentLogin.LastName }

                    };
                    emailDetails.SpecificInfo.Add("{{code}}", constructStudyload.ToString());
                    _emailHandler.SendEmail(emailDetails, (int)RequestResponse.Enums.EmailType.OFFICIALENROLLMENT);
                });

                Tk.Wait();
            }
            else
            {
            }

            if (!isFull)
            {
                _ucOnlinePortalContext._212notifications.Add(newNotif);
                _ucOnlinePortalContext._212oenrps.Update(studentOenrp);
                _ucOnlinePortalContext.SaveChanges();

                return new SetApproveOrDisapprovedResponse { success = 1, id_number = final_id, edp_code = null };
            }
            else
            {
                List<string> edpcodes = new List<string>();

                foreach (_212schedule sched in schedules)
                {
                    edpcodes.Add(sched.EdpCode);
                }
                return new SetApproveOrDisapprovedResponse { success = 0, id_number = final_id, edp_code = edpcodes };
            }
        }

        /*
       * Method to get open sections
       */

        public GetActiveSectionsResponse GetActiveSections(GetActiveSectionsRequest getActiveSectionsRequest)
        {
            //get schedules having the same coursecode and year
            var sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Contains(getActiveSectionsRequest.year_level.ToString()) && !x.Section.Contains("XX")).ToList();

            if (getActiveSectionsRequest.year_level == 1118)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(2, 2).Equals("1A") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.year_level == 1119)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(2, 2).Equals("1P") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.year_level == 1218)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(2, 2).Equals("2A") && !x.Section.Contains("XX")).ToList();

            }
            else if (getActiveSectionsRequest.year_level == 1219)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(2, 2).Equals("2P") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.year_level == 1128)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(1, 2).Equals("-A") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.year_level == 1129)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(1, 2).Equals("-P") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.year_level == 1228)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(2, 2).Equals("-A") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.year_level == 1229)
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Substring(2, 2).Equals("-P") && !x.Section.Contains("XX")).ToList();
            }
            else if (getActiveSectionsRequest.course_code.Equals("MM") || getActiveSectionsRequest.course_code.Equals("MV"))
            {
                sections = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getActiveSectionsRequest.course_code && x.Section.Contains(getActiveSectionsRequest.year_level.ToString()) && !x.Section.Contains("XX") && x.Status == 1).ToList();
            }

            List<string> sects = new List<string>();

            //sort by section'
            if (sections.Count > 0)
            {
                var sectionList = sections.OrderBy(x => x.Section).ToList();

                //algo to check every subject in section. if section has 1 full subject, dont include
                if (sectionList != null)
                {
                    string currentSec = sectionList.First().Section;
                    bool valid = true;

                    foreach (_212schedule sched in sectionList)
                    {
                        if (!currentSec.Equals(sched.Section))
                        {
                            if (valid)
                            {
                                sects.Add(currentSec);
                            }

                            currentSec = sched.Section;
                            valid = true;
                        }

                        if (valid)
                        {
                            valid = sched.MaxSize - sched.Size >= 1 ? true : false;
                        }
                    }

                    if (valid)
                    {
                        sects.Add(currentSec);
                    }
                }
            }

            return new GetActiveSectionsResponse { sections = sects };
        }

        /*
        * Method to get student request
        */

        public StudentReqResponse GetStudentRequest(StudentReqRequest studentReqRequest)
        {
            //always get status 5 -> for dean
            var result = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                          join _loginInfo in _ucOnlinePortalContext.LoginInfos
                          on _212oenrp.StudId equals _loginInfo.StudId
                          join _courseList in _ucOnlinePortalContext.CourseLists
                          on _212oenrp.CourseCode equals _courseList.CourseCode
                          where _212oenrp.Status == 5 && (_212oenrp.RequestDeblock == 1 || _212oenrp.RequestOverload == 1)
                          select new StudentReqResponse.Student
                          {
                              id_number = _212oenrp.StudId,
                              lastname = _loginInfo.LastName,
                              firstname = _loginInfo.FirstName,
                              mi = _loginInfo.Mi,
                              suffix = _loginInfo.Suffix,
                              classification = Utils.Function.getClassification(_212oenrp.Classification),
                              course_year = _courseList.CourseAbbr + " " + _212oenrp.YearLevel,
                              course_code = _212oenrp.CourseCode,
                              status = _212oenrp.Status,
                              date = _212oenrp.RegisteredOn,
                              request_overload = (int)_212oenrp.RequestOverload,
                              request_deblock = (int)_212oenrp.RequestDeblock
                          });

            if (!studentReqRequest.course_department.Equals(String.Empty))
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == studentReqRequest.course_department).ToList();
                var courses = courseList.Select(x => x.CourseCode).ToList();

                result = result.Where(x => courses.Contains(x.course_code));
            }

            var count = result.Count();

            return new StudentReqResponse { students = result.ToList(), count = count };
        }

        /*
        * Method to apply request
        */
        public ApplyReqResponse ApplyRequest(ApplyReqRequest applyReqRequest)
        {
            var student = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == applyReqRequest.stud_id).FirstOrDefault();

            student.RequestDeblock = (short)applyReqRequest.request_deblock;
            student.RequestOverload = (short)applyReqRequest.request_overload;

            _ucOnlinePortalContext.SaveChanges();

            return new ApplyReqResponse { success = 1 };
        }


        /*
        * Method to approve request
        */
        public ApproveReqResponse ApproveRequest(ApproveReqRequest approveReqRequest)
        {
            var studOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == approveReqRequest.id_number).FirstOrDefault();
            var login = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == approveReqRequest.id_number).FirstOrDefault();

            _212notification newNotif = new _212notification();

            if (approveReqRequest.approved_overload == 2)
            {
                studOenrp.RequestOverload = 2;
                login.AllowedUnits = (short)approveReqRequest.max_units;

                newNotif = new _212notification
                {
                    StudId = approveReqRequest.id_number,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.APPROVE_OVERLOAD + " " + approveReqRequest.max_units
                };
            }
            else if (approveReqRequest.approved_overload == 3)
            {
                studOenrp.RequestOverload = 3;
                newNotif = new _212notification
                {
                    StudId = approveReqRequest.id_number,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.DISAPPROVE_OVERLOAD
                };
            }


            if (approveReqRequest.approved_deblock == 2)
            {
                studOenrp.RequestDeblock = 2;
                studOenrp.Section = "";
                newNotif = new _212notification
                {
                    StudId = approveReqRequest.id_number,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.APPROVE_DE_BLOCK
                };

                studOenrp.SubmittedOn = null;
                studOenrp.EnrollmentDate = null;


                _ucOnlinePortalContext._212ostsps.RemoveRange(_ucOnlinePortalContext._212ostsps.Where(x => x.StudId == approveReqRequest.id_number));
            }
            if (approveReqRequest.approved_deblock == 3)
            {
                studOenrp.RequestDeblock = 3;
                newNotif = new _212notification
                {
                    StudId = approveReqRequest.id_number,
                    NotifRead = 0,
                    Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                    Message = Literals.DISAPPROVE_DE_BLOCK
                };
            }

            if (approveReqRequest.approved_promissory == 3)
            {
                studOenrp.RequestPromissory = 3;

                if (studOenrp.PromiPay != approveReqRequest.promise_pay)
                {
                    newNotif = new _212notification
                    {
                        StudId = approveReqRequest.id_number,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.APPROVE_PROMISSORY + " but changed the promissory amount"
                    };
                }
                else
                {
                    newNotif = new _212notification
                    {
                        StudId = approveReqRequest.id_number,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = Literals.APPROVE_PROMISSORY
                    };
                }

                var promiP = studOenrp.PromiPay;

                if (approveReqRequest.promise_pay == 0)
                {
                    studOenrp.PromiPay = promiP;
                }
                else
                {
                    studOenrp.PromiPay = approveReqRequest.promise_pay;
                }

                if (!approveReqRequest.message.Equals(String.Empty))
                {
                    _ucOnlinePortalContext._212notifications.Add(newNotif);
                    _ucOnlinePortalContext.SaveChanges();
                    newNotif = new _212notification

                    {
                        StudId = approveReqRequest.id_number,
                        NotifRead = 0,
                        Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                        Message = "Promissory Message: " + approveReqRequest.message
                    };
                }

            }

            _ucOnlinePortalContext._212oenrps.Update(studOenrp);
            _ucOnlinePortalContext._212notifications.Add(newNotif);
            _ucOnlinePortalContext.SaveChanges();
            return new ApproveReqResponse { success = 1 };
        }

        /*
       * Method to get sections 
       */
        public GetSectionResponse GetSection(GetSectionRequest getSectionRequest)
        {
            List<_212schedule> schedules = new List<_212schedule>();

            if (!getSectionRequest.college_abbr.Equals(String.Empty))
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == getSectionRequest.college_abbr).ToList();
                var courses = courseList.Select(x => x.CourseCode).ToList();

                schedules = _ucOnlinePortalContext._212schedules.Where(x => courses.Contains(x.CourseCode)).ToList();
            }
            if (!getSectionRequest.course_code.Equals(String.Empty))
            {
                if (schedules.Count > 0)
                {
                    schedules = schedules.Where(x => x.CourseCode == getSectionRequest.course_code).ToList();
                }
                else
                {
                    schedules = _ucOnlinePortalContext._212schedules.Where(x => x.CourseCode == getSectionRequest.course_code).ToList();
                }
            }
            if (!getSectionRequest.course_code.Equals(String.Empty))
            {
                if (getSectionRequest.year_level != 0)
                {
                    if (getSectionRequest.year_level == 111)
                    {
                        schedules = schedules.Where(x => x.Section.Substring(2, 1).Equals("1")).ToList();
                    }
                    else if (getSectionRequest.year_level == 121)
                    {
                        schedules = schedules.Where(x => x.Section.Substring(2, 1).Equals("2")).ToList();
                    }
                    else if (getSectionRequest.year_level == 112)
                    {
                        schedules = schedules.Where(x => x.Section.Substring(1, 1).Equals("-")).ToList();
                    }
                    else if (getSectionRequest.year_level == 122)
                    {
                        schedules = schedules.Where(x => x.Section.Substring(2, 1).Equals("-")).ToList();
                    }
                    else
                    {
                        schedules = schedules.Where(x => x.Section.Contains(getSectionRequest.year_level.ToString())).ToList();
                    }
                }
            }

            schedules = schedules.GroupBy(x => x.Section).Select(grp => grp.First()).ToList();

            var result = schedules.Select(x => new GetSectionResponse.sections
            {
                course_code = x.CourseCode,
                section = x.Section
            });


            return new GetSectionResponse { section = result.ToList() };
        }

        /*
       * Method to update status sections 
       */
        public ChangeSchedStatusResponse ChangeSchedStatus(ChangeSchedStatusRequest changeSchedStatusRequest)
        {
            if (!changeSchedStatusRequest.course_code.Equals(String.Empty) && !changeSchedStatusRequest.section.Equals(String.Empty))
            {
                var schedule = _ucOnlinePortalContext._212schedules.Where(x => x.Section == changeSchedStatusRequest.section && x.CourseCode == changeSchedStatusRequest.course_code);
                schedule.ToList().ForEach(x => x.Status = (short)changeSchedStatusRequest.status);
                _ucOnlinePortalContext.SaveChanges();
            }

            if (!changeSchedStatusRequest.edp_code.Equals(String.Empty) && changeSchedStatusRequest.edp_code.Count > 0)
            {
                var schedules = _ucOnlinePortalContext._212schedules.Where(x => changeSchedStatusRequest.edp_code.Contains(x.EdpCode));
                schedules.ToList().ForEach(x => x.Status = (short)changeSchedStatusRequest.status);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new ChangeSchedStatusResponse { success = 1 };
        }

        /*
       * Method to cancel enrollment
       */
        public CancelEnrollmentResponse CancelEnrollment(CancelEnrollmentRequest cancelEnrollmentRequest)
        {
            var studentOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == cancelEnrollmentRequest.id_number).FirstOrDefault();

            if (studentOenrp != null)
            {
                studentOenrp.Status = (short)EnrollmentStatus.CANCELLED;
            }

            var studentOtsp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == cancelEnrollmentRequest.id_number).ToList();
            var edpCodes = studentOtsp.Select(x => x.EdpCode).ToList();

            if (studentOtsp.Count > 0)
            {
                studentOtsp.ForEach(x => x.Status = 2);

                var schedules = _ucOnlinePortalContext._212schedules.Where(x => edpCodes.Contains(x.EdpCode)).ToList();
                schedules.ForEach(x => x.Size = (x.Size - 1));
            }

            _212notification newNotif = new _212notification
            {
                StudId = cancelEnrollmentRequest.id_number,
                NotifRead = 0,
                Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                Message = Literals.CANCELLED
            };

            _ucOnlinePortalContext._212notifications.Add(newNotif);
            _ucOnlinePortalContext.SaveChanges();

            return new CancelEnrollmentResponse { success = 1 };
        }

        /*
       * Method to get status count
       */
        public GetStatusCountResponse GetStatusCount(GetStatusCountRequest getStatusCountRequest)
        {
            int requestCount = 0;
            int pendingPromissory = 0;
            int approvedPromissory = 0;

            Dictionary<int, int> counts = new Dictionary<int, int>();

            List<string> courses = new List<string>();
            if (!getStatusCountRequest.course_department.Equals(String.Empty))
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == getStatusCountRequest.course_department).ToList();
                courses = courseList.Select(x => x.CourseCode).ToList();

                var count_dissapprove = _ucOnlinePortalContext._212oenrps.Where(x => courses.Contains(x.CourseCode) && x.DisapprovedDeanOn != null).Count();
                counts.Add(7, count_dissapprove);

                var count_approved_dean = _ucOnlinePortalContext._212oenrps.Where(x => courses.Contains(x.CourseCode) && x.ApprovedDeanOn != null).Count();
                counts.Add(6, count_approved_dean);
            }

            if (!getStatusCountRequest.course_department.Equals(String.Empty))
            {
                foreach (var line in _ucOnlinePortalContext._212oenrps.Where(x => courses.Contains(x.CourseCode)).GroupBy(x => x.Status)
                        .Select(group => new
                        {
                            Metric = group.Key,
                            Count = group.Count()
                        })
                        .OrderBy(x => x.Metric))
                {
                    if (line.Metric != 6)
                        counts.Add(line.Metric, line.Count);
                }


                requestCount = _ucOnlinePortalContext._212oenrps.Where(x => courses.Contains(x.CourseCode) && (x.RequestOverload == 1 || x.RequestDeblock == 1)).Count();
            }
            else
            {
                var count_dissapprove = _ucOnlinePortalContext._212oenrps.Where(x => x.DisapprovedDeanOn != null).Count();

                counts.Add(7, count_dissapprove);

                var count_accounting = _ucOnlinePortalContext._212oenrps.Where(x => x.ApprovedAcctgOn != null).Count();
                counts.Add(14, count_accounting);

                foreach (var line in _ucOnlinePortalContext._212oenrps.GroupBy(x => x.Status)
                       .Select(group => new
                       {
                           Metric = group.Key,
                           Count = group.Count()
                       })
                       .OrderBy(x => x.Metric))
                {
                    counts.Add(line.Metric, line.Count);
                }
            }

            List<int> count = new List<int>();
            for (int counter = 0; counter < 15; counter++)
            {
                if (counts.ContainsKey(counter))
                {
                    if (counter == 8)
                    {
                        var countCashier = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                                            join _attachment in _ucOnlinePortalContext._212attachments
                                            on _212oenrp.StudId equals _attachment.StudId
                                            where _attachment.AttachmentId == (from _attach in _ucOnlinePortalContext._212attachments
                                                                               where _attach.StudId == _attachment.StudId
                                                                               && _attach.Type.Equals("Payment")
                                                                               orderby _attach.AttachmentId
                                                                               select _attach.AttachmentId).FirstOrDefault()
                                                                               && _212oenrp.Status == 8
                                            select new
                                            {
                                                studId = _212oenrp.StudId
                                            }).Count();

                        count.Add(countCashier);
                    }
                    else
                    {
                        count.Add(counts[counter]);
                    }
                }
                else
                {
                    count.Add(0);
                }
            }

            if (!getStatusCountRequest.course_department.Equals(String.Empty))
            {
                pendingPromissory = _ucOnlinePortalContext._212oenrps.Where(x => (x.RequestPromissory == 1) && courses.Contains(x.CourseCode)).Count();
                approvedPromissory = _ucOnlinePortalContext._212oenrps.Where(x => (x.RequestPromissory == 3) && courses.Contains(x.CourseCode)).Count();
            }
            else
            {
                pendingPromissory = _ucOnlinePortalContext._212oenrps.Where(x => (x.RequestPromissory == 2)).Count();
                approvedPromissory = _ucOnlinePortalContext._212oenrps.Where(x => (x.RequestPromissory == 3)).Count();
            }

            GetStatusCountResponse response = new GetStatusCountResponse
            {
                registered = count[0],
                approved_registration_registrar = count[1],
                disapproved_registration_registrar = count[2],
                approved_registration_dean = count[3],
                disapproved_registration_dean = count[4],
                selecting_subjects = count[5] - requestCount,
                approved_by_dean = count[6],
                disapproved_by_dean = count[7],
                approved_by_accounting = count[8],
                approved_by_cashier = count[9],
                officially_enrolled = count[10],
                withdrawn_enrollment_before_start_of_class = count[11],
                withdrawn_enrollment_start_of_class = count[12],
                cancelled = count[13],
                accounting_count = count[14],
                request = requestCount,
                pending_promissory = pendingPromissory,
                approved_promissory = approvedPromissory
            };

            return response;
        }

        /*
        * Method to save payments
        */
        public SavePaymentResponse SavePayments(SavePaymentRequest savePaymentsRequest)
        {
            if (savePaymentsRequest.attachments.Count > 0)
            {
                foreach (SavePaymentRequest.Attachmentss attachment in savePaymentsRequest.attachments)
                {
                    _212attachment newAttachment = new _212attachment
                    {
                        StudId = savePaymentsRequest.id_number,
                        Email = attachment.email,
                        Filename = attachment.filename,
                        Type = "Payment"
                    };

                    _ucOnlinePortalContext._212attachments.Add(newAttachment);
                    _ucOnlinePortalContext.SaveChanges();
                }
            }

            return new SavePaymentResponse { succcess = 1 };
        }

        /*
       * Method to view classlist
       */
        public ViewClasslistResponse ViewClasslist(ViewClasslistRequest viewClasslistRequest)
        {
            var schedule = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == viewClasslistRequest.edp_code).FirstOrDefault();

            var result = (from _ostsp in _ucOnlinePortalContext._212ostsps
                          join _212schedule in _ucOnlinePortalContext._212schedules
                          on _ostsp.EdpCode equals _212schedule.EdpCode
                          join _loginfo in _ucOnlinePortalContext.LoginInfos
                          on _ostsp.StudId equals _loginfo.StudId
                          join _course in _ucOnlinePortalContext.CourseLists
                          on _loginfo.CourseCode equals _course.CourseCode
                          where _ostsp.EdpCode == viewClasslistRequest.edp_code
                          select new ViewClasslistResponse.Enrolled
                          {
                              id_number = _loginfo.StudId,
                              last_name = _loginfo.LastName,
                              firstname = _loginfo.FirstName,
                              course_year = _course.CourseAbbr + " " + _loginfo.Year,
                              mobile_number = _loginfo.MobileNumber,
                              email = _loginfo.Email,
                              status = _ostsp.Status
                          });

            var OfficialEnrolled = result.Where(x => x.status == 3).OrderBy(x => x.last_name).ToList();
            var PendingEnrolled = result.Where(x => x.status == 1).OrderBy(x => x.last_name).ToList();

            var NotAcceptedSection = result.Where(x => x.status == 0).OrderBy(x => x.last_name).ToList();
            List<ViewClasslistResponse.Enrolled> not_accepted = new List<ViewClasslistResponse.Enrolled>();

            foreach (ViewClasslistResponse.Enrolled enr in NotAcceptedSection)
            {
                var studOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == enr.id_number).FirstOrDefault();

                if (studOenrp.Section != null && !studOenrp.Section.Equals(String.Empty))
                {
                    not_accepted.Add(enr);
                }
            }

            ViewClasslistResponse response = new ViewClasslistResponse
            {
                edp_code = schedule.EdpCode,
                subject_name = schedule.Description + " " + schedule.SubType,
                time_info = schedule.TimeStart + " - " + schedule.TimeEnd + " " + schedule.Mdn + " " + schedule.Days,
                units = schedule.Units.ToString(),
                official_enrolled = OfficialEnrolled,
                pending_enrolled = PendingEnrolled,
                subject_size = schedule.Size,
                official_enrolled_size = OfficialEnrolled.Count(),
                pending_enrolled_size = PendingEnrolled.Count(),
                not_accepted_section = schedule.Size - (OfficialEnrolled.Count() + PendingEnrolled.Count()),
                not_accepted = not_accepted
            };

            return response;
        }

        /*
        * Method to view classlist
        *   0 REGISTERED,
            1 APPROVED_REGISTRATION_REGISTRAR,
            2 DISAPPROVED_REGISTRATION_REGISTRAR,
            3 APPROVED_REGISTRATION_DEAN,
            4 DISAPPROVED_REGISTRATION_DEAN,
            5 SELECTING_SUBJECTS,
            6 APPROVED_BY_DEAN,
            7 DISAPPROVED_BY_DEAN,
            8 APPROVED_BY_ACCOUNTING,
            9 APPROVED_BY_CASHIER,
            10 OFFICIALLY_ENROLLED,
            11 WITHDRAWN_ENROLLMENT_BEFORE_START_OF_CLASS,
            12 WITHDRAWN_ENROLLMENT_START_OF_CLASS,
            13 CANCELLED
        */
        public GetEnrollmentStatusResponse GetEnrollmentStatus(GetEnrollmentStatusRequest getEnrollmentStatusRequest)
        {
            List<GetEnrollmentStatusResponse.courseStatus> records = new List<GetEnrollmentStatusResponse.courseStatus>();

            Dictionary<int, int> counts = new Dictionary<int, int>();

            var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.Department == getEnrollmentStatusRequest.department && x.CourseActive == 1).ToList();

            var coursesList = courseList.Select(x => x.CourseCode);

            if (!getEnrollmentStatusRequest.dte.ToString().ToUpper().Equals("1/1/0001 12:00:00 AM"))
            {
                foreach (var line in _ucOnlinePortalContext._212oenrps.Where(x => coursesList.Contains(x.CourseCode) && DateTime.Parse(getEnrollmentStatusRequest.dte.ToShortDateString() + " 00:00:00") <= x.RegisteredOn && DateTime.Parse(getEnrollmentStatusRequest.dte.ToShortDateString() + " 23:59:59") >= x.RegisteredOn).GroupBy(x => x.Status)
                       .Select(group => new
                       {
                           Metric = group.Key,
                           Count = group.Count()
                       })
                       .OrderBy(x => x.Metric))
                {
                    counts.Add(line.Metric, line.Count);
                }
            }
            else
            {
                foreach (var line in _ucOnlinePortalContext._212oenrps.Where(x => coursesList.Contains(x.CourseCode)).GroupBy(x => x.Status)
                      .Select(group => new
                      {
                          Metric = group.Key,
                          Count = group.Count()
                      })
                      .OrderBy(x => x.Metric))
                {
                    counts.Add(line.Metric, line.Count);
                }
            }

            int countPendingPayment = 0;
            int countPendingCashier = 0;

            List<int> count = new List<int>();
            for (int counter = 0; counter < 14; counter++)
            {
                if (counts.ContainsKey(counter))
                {
                    if (counter == 8)
                    {
                        /*var pending_cashier = _ucOnlinePortalContext._212oenrps.Where(x => x.Status == 8 && coursesList.Contains(x.CourseCode)).Select(x => x.StudId).ToList();

                        result = result.Where(x => (int)x.status == 8);*/

                        var countCashier = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                                            join _attachment in _ucOnlinePortalContext._212attachments
                                            on _212oenrp.StudId equals _attachment.StudId
                                            where _attachment.AttachmentId == (from _attach in _ucOnlinePortalContext._212attachments
                                                                               where _attach.StudId == _attachment.StudId
                                                                               && _attach.Type.Equals("Payment")
                                                                               orderby _attach.AttachmentId
                                                                               select _attach.AttachmentId).FirstOrDefault()
                                                                               && _212oenrp.Status == 8 && coursesList.Contains(_212oenrp.CourseCode)
                                            select _212oenrp.StudId);

                        countPendingCashier = countCashier.Count();

                        countPendingPayment = _ucOnlinePortalContext._212oenrps.Where(x => x.Status == 8 && coursesList.Contains(x.CourseCode) && !countCashier.Contains(x.StudId)).Count();
                    }
                    count.Add(counts[counter]);
                }
                else
                {
                    count.Add(0);
                }
            }

            GetEnrollmentStatusResponse.courseStatus statusAll = new GetEnrollmentStatusResponse.courseStatus
            {
                courseName = "All",
                pending_registered = count[0] + count[1],
                subject_selection = count[3],
                pending_dean = count[5],
                pending_accounting = count[6],
                pending_payment = countPendingPayment,
                pending_cashier = countPendingCashier,
                pending_total = count[5] + count[6] + countPendingCashier,
                official_total = count[10]
            };

            records.Add(statusAll);

            foreach (CourseList cl in courseList)
            {
                int year_level = 0;

                if (getEnrollmentStatusRequest.department.Equals("CL"))
                {
                    year_level = 1;
                }
                else
                {
                    year_level = 11;
                }

                for (int year = year_level; year < year_level + cl.CourseYearLimit; year++)
                {
                    counts = new Dictionary<int, int>();
                    if (!getEnrollmentStatusRequest.dte.ToString().ToUpper().Equals("1/1/0001 12:00:00 AM"))
                    {
                        foreach (var line in _ucOnlinePortalContext._212oenrps.Where(x => x.CourseCode == cl.CourseCode && DateTime.Parse(getEnrollmentStatusRequest.dte.ToShortDateString() + " 00:00:00") <= x.RegisteredOn && DateTime.Parse(getEnrollmentStatusRequest.dte.ToShortDateString() + " 23:59:59") >= x.RegisteredOn && x.YearLevel == year).GroupBy(x => x.Status)
                              .Select(group => new
                              {
                                  Metric = group.Key,
                                  Count = group.Count()
                              })
                              .OrderBy(x => x.Metric))
                        {
                            counts.Add(line.Metric, line.Count);
                        }
                    }
                    else
                    {
                        foreach (var line in _ucOnlinePortalContext._212oenrps.Where(x => x.CourseCode == cl.CourseCode && x.YearLevel == year).GroupBy(x => x.Status)
                              .Select(group => new
                              {
                                  Metric = group.Key,
                                  Count = group.Count()
                              })
                              .OrderBy(x => x.Metric))
                        {
                            counts.Add(line.Metric, line.Count);
                        }
                    }

                    countPendingPayment = 0;
                    countPendingCashier = 0;

                    count = new List<int>();
                    for (int counter = 0; counter < 14; counter++)
                    {
                        if (counts.ContainsKey(counter))
                        {
                            if (counter == 8)
                            {
                                var countCashier = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                                                    join _attachment in _ucOnlinePortalContext._212attachments
                                                    on _212oenrp.StudId equals _attachment.StudId
                                                    where _attachment.AttachmentId == (from _attach in _ucOnlinePortalContext._212attachments
                                                                                       where _attach.StudId == _attachment.StudId
                                                                                       && _attach.Type.Equals("Payment")
                                                                                       orderby _attach.AttachmentId
                                                                                       select _attach.AttachmentId).FirstOrDefault()
                                                                                       && _212oenrp.Status == 8 && _212oenrp.CourseCode == cl.CourseCode && _212oenrp.YearLevel == year
                                                    select _212oenrp.StudId);

                                countPendingCashier = countCashier.Count();

                                countPendingPayment = _ucOnlinePortalContext._212oenrps.Where(x => x.Status == 8 && x.CourseCode == cl.CourseCode && x.YearLevel == year && !countCashier.Contains(x.StudId)).Count();

                            }
                            count.Add(counts[counter]);
                        }
                        else
                        {
                            count.Add(0);
                        }
                    }

                    statusAll = new GetEnrollmentStatusResponse.courseStatus
                    {
                        courseName = cl.CourseDepartment + " - " + cl.CourseAbbr,
                        pending_registered = count[0] + count[1],
                        subject_selection = count[3],
                        pending_dean = count[5],
                        pending_accounting = count[6],
                        pending_payment = countPendingPayment,
                        pending_cashier = countPendingCashier,
                        pending_total = count[5] + count[6] + countPendingCashier,
                        official_total = count[10],
                        year_level = year
                    };

                    records.Add(statusAll);
                }

            }
            return new GetEnrollmentStatusResponse { courseStat = records };
        }

        /*
       * Method to view classlist
       */
        public UpdateStudentInfoResponse UpdateStudentInfo(UpdateStudentInfoRequest updateStudentInfoRequest)
        {
            var studLoginInfo = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == updateStudentInfoRequest.id_number).FirstOrDefault();

            studLoginInfo.CourseCode = updateStudentInfoRequest.course_code;
            studLoginInfo.Dept = updateStudentInfoRequest.dept;
            studLoginInfo.Year = (short)updateStudentInfoRequest.year;
            studLoginInfo.MobileNumber = updateStudentInfoRequest.mobile;
            studLoginInfo.Facebook = updateStudentInfoRequest.facebook;


            //Add OENRP
            _212oenrp newStudentOenrp = new _212oenrp
            {
                StudId = updateStudentInfoRequest.id_number,
                YearLevel = (short)updateStudentInfoRequest.year,
                CourseCode = updateStudentInfoRequest.course_code,
                RegisteredOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                Units = 0,
                Classification = updateStudentInfoRequest.classification,
                Dept = updateStudentInfoRequest.dept,
                Status = (short)EnrollmentStatus.REGISTERED,
                AdjustmentCount = 1,
                RequestDeblock = 0,
                RequestOverload = 0,
                NeededPayment = 0,
                Mdn = updateStudentInfoRequest.mdn,
                PromiPay = 0,
                RequestPromissory = 0
            };

            _ucOnlinePortalContext.LoginInfos.Update(studLoginInfo);
            _ucOnlinePortalContext._212oenrps.Add(newStudentOenrp);
            _ucOnlinePortalContext.SaveChanges();

            _212notification newNotification = new _212notification
            {
                StudId = updateStudentInfoRequest.id_number,
                Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                Message = Literals.REGISTERED,
                NotifRead = 0
            };

            _ucOnlinePortalContext._212notifications.Add(newNotification);
            _ucOnlinePortalContext.SaveChanges();

            var studentOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == updateStudentInfoRequest.id_number).FirstOrDefault();

            studentOenrp.ApprovedRegRegistrar = "AUTO-APPROVE";
            studentOenrp.ApprovedRegRegistrarOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            studentOenrp.Status = 1;

            newNotification = new _212notification
            {
                StudId = updateStudentInfoRequest.id_number,
                NotifRead = 0,
                Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                Message = Literals.APPROVED_REGISTRATION_REGISTRAR
            };

            _ucOnlinePortalContext._212notifications.Add(newNotification);
            _ucOnlinePortalContext.SaveChanges();

            return new UpdateStudentInfoResponse { success = 1 };
        }

        /*
       * Method to view classlist
       */
        public ViewStudentEvaluationResponse ViewEvaluation(ViewStudentEvaluationRequest viewStudentEvaluationRequest)
        {
            var result = (from _gradesEval in _ucOnlinePortalContext.GradeEvaluations
                          join _subject_info in _ucOnlinePortalContext.SubjectInfos
                          on _gradesEval.IntCode equals _subject_info.InternalCode into sched
                          from _subject_info in sched.DefaultIfEmpty()
                          where _gradesEval.StudId == viewStudentEvaluationRequest.id_number
                          select new ViewStudentEvaluationResponse.Grades
                          {
                              subject_name = _subject_info.SubjectName,
                              subject_type = _subject_info.SubjectType,
                              descriptive = _subject_info.Descr1.Trim() + " " + _subject_info.Descr2.Trim(),
                              midterm_grade = _gradesEval.MidtermGrade,
                              final_grade = _gradesEval.FinalGrade,
                              units = _subject_info.Units,
                              term = _gradesEval.Term
                          }).ToList();


            List<ViewStudentEvaluationResponse.Grades> listGrades = new List<ViewStudentEvaluationResponse.Grades>();
            List<String> subNames = new List<string>();

            foreach (ViewStudentEvaluationResponse.Grades grd in result)
            {
                if (grd.term.Equals("20211"))
                {
                    if (subNames.Contains(grd.subject_name))
                    {
                        var findGrade = listGrades.Where(x => x.subject_name == grd.subject_name).FirstOrDefault();
                        if (grd.midterm_grade != null && !grd.midterm_grade.Equals(String.Empty))
                        {
                            findGrade.midterm_grade = grd.midterm_grade;

                        }
                        if (grd.final_grade != null && !grd.final_grade.Equals(String.Empty))
                        {
                            findGrade.final_grade = grd.final_grade;
                        }
                        continue;
                    }
                    else
                    {
                        subNames.Add(grd.subject_name);
                        listGrades.Add(grd);
                    }
                }
                else
                {
                    listGrades.Add(grd);
                }
            }

            return new ViewStudentEvaluationResponse { studentGrades = listGrades.OrderByDescending(x => x.term).ThenBy(x => x.subject_name).ToList() };
        }


        /*
      * Method to view classlist
      */
        public ViewOldStudentInfoResponse ViewOldStudentInfo(ViewOldStudentInfoRequest viewOldStudentInfoRequest)
        {
            var studOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == viewOldStudentInfoRequest.id_number).FirstOrDefault();
            var studLogin = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == viewOldStudentInfoRequest.id_number).FirstOrDefault();

            var attachmentList = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == viewOldStudentInfoRequest.id_number && x.Type != "Payment").ToList();

            if (viewOldStudentInfoRequest.payment != null && viewOldStudentInfoRequest.payment == 1)
            {
                attachmentList = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == viewOldStudentInfoRequest.id_number && x.Type.Equals("Payment")).Take(1).ToList();
            }

            List<ViewOldStudentInfoResponse.attachment> attach = new List<ViewOldStudentInfoResponse.attachment>();
            attach = attachmentList.Select(x => new ViewOldStudentInfoResponse.attachment
            {
                attachment_id = x.AttachmentId,
                email = x.Email,
                filename = x.Filename,
                id_number = x.StudId,
                type = x.Type
            }).ToList();

            ViewOldStudentInfoResponse oldStudInfo = new ViewOldStudentInfoResponse();

            var course = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == studLogin.CourseCode).FirstOrDefault();

            if (studLogin != null)
            {
                oldStudInfo.stud_id = studLogin.StudId;
                oldStudInfo.last_name = studLogin.LastName;
                oldStudInfo.first_name = studLogin.FirstName;
                oldStudInfo.middle_name = studLogin.Mi;
                oldStudInfo.suffix = studLogin.Suffix;
                oldStudInfo.start_term = studLogin.StartTerm == null ? 0 : int.Parse(studLogin.StartTerm);
                oldStudInfo.dept = studLogin.Dept;
                oldStudInfo.course_code = studLogin.CourseCode;
                oldStudInfo.course = course.CourseAbbr;
                oldStudInfo.college = course.CourseDepartment;
                oldStudInfo.year_level = (short)studLogin.Year;
                oldStudInfo.mobile = studLogin.MobileNumber;
                oldStudInfo.email = studLogin.Email;
                oldStudInfo.facebook = studLogin.Facebook;
                oldStudInfo.allowed_units = studLogin.AllowedUnits == null ? 0 : (short)studLogin.AllowedUnits;
                oldStudInfo.gender = studLogin.Sex;
                oldStudInfo.birthdate = studLogin.Birthdate.ToString();
                oldStudInfo.is_verified = (short)studLogin.IsVerified;
            }

            if (studOenrp != null)
            {
                oldStudInfo.section = studOenrp.Section;
                oldStudInfo.mdn = studOenrp.Mdn;
                oldStudInfo.classification = studOenrp.Classification;
                oldStudInfo.assigned_section = studOenrp.Section;
                oldStudInfo.request_deblock = (short)studOenrp.RequestDeblock;
                oldStudInfo.request_overload = (short)studOenrp.RequestOverload;
            }

            oldStudInfo.attachments = attach;

            return oldStudInfo;
        }


        /*
         * Method to view classlist
         */
        public SelectSubjectInfoResponse SelectSubject(SelectSubjectInfoRequest selectSubjectInfoRequest)
        {
            var subject = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == selectSubjectInfoRequest.edp_code).FirstOrDefault();

            if (subject == null)
            {
                return null;
            }
            else
            {
                SelectSubjectInfoResponse subjectResponse = new SelectSubjectInfoResponse
                {
                    ScheduleId = subject.ScheduleId,
                    EdpCode = subject.EdpCode,
                    Description = subject.Description,
                    InternalCode = subject.InternalCode,
                    SubType = subject.SubType,
                    Units = subject.Units,
                    TimeStart = subject.TimeStart,
                    TimeEnd = subject.TimeEnd,
                    Mdn = subject.Mdn,
                    Days = subject.Days,
                    MTimeStart = subject.MTimeStart,
                    MTimeEnd = subject.MTimeEnd,
                    MDays = subject.MDays,
                    MaxSize = subject.MaxSize,
                    Section = subject.Section,
                    Room = subject.Room,
                    SplitType = subject.SplitType,
                    SplitCode = subject.SplitCode,
                    IsGened = subject.IsGened,
                    status = subject.Status

                };

                return subjectResponse;
            }
        }

        /*
        * Method to view classlist
        */
        public UpdateSubjectResponse UpdateSubject(UpdateSubjectRequest updateSubjectRequest)
        {
            var subject = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == updateSubjectRequest.EdpCode).FirstOrDefault();

            subject.Description = updateSubjectRequest.Description;
            subject.SubType = updateSubjectRequest.SubType;
            subject.Units = updateSubjectRequest.Units;
            subject.TimeStart = updateSubjectRequest.TimeStart;
            subject.TimeEnd = updateSubjectRequest.TimeEnd;
            subject.Mdn = updateSubjectRequest.Mdn;
            subject.Days = updateSubjectRequest.Days;
            subject.MTimeStart = updateSubjectRequest.MTimeStart;
            subject.MTimeEnd = updateSubjectRequest.MTimeEnd;
            subject.MDays = updateSubjectRequest.MDays;
            subject.MaxSize = updateSubjectRequest.MaxSize;
            subject.Section = updateSubjectRequest.Section;
            subject.Room = updateSubjectRequest.Room;
            subject.SplitType = updateSubjectRequest.SplitType;
            subject.SplitCode = updateSubjectRequest.SplitCode;
            subject.IsGened = updateSubjectRequest.IsGened;
            subject.Status = (short)updateSubjectRequest.status;

            _ucOnlinePortalContext._212schedules.Update(subject);
            _ucOnlinePortalContext.SaveChanges();

            return new UpdateSubjectResponse { success = 1 };
        }

        /*
        * Method to view classlist
        */
        public RemoveDuplicateOtspResponse RemoveDuplicateOstsp()
        {
            String sqlToRemove = "WITH cte AS ( SELECT stud_id, edp_code, row_number() OVER(PARTITION BY stud_id, edp_code ORDER BY stud_id) AS[rn]  FROM[UCOnlinePortal].[dbo].[212ostsp]) DELETE cte WHERE[rn] > 1";
            _ucOnlinePortalContext.Database.ExecuteSqlRaw(sqlToRemove);

            var distinctOstsp = _ucOnlinePortalContext._212ostsps.Select(x => x.EdpCode).Distinct().ToList();

            foreach (string edpcode in distinctOstsp)
            {
                int pending = 0;
                int official = 0;

                var ostspDetail = _ucOnlinePortalContext._212ostsps.Where(x => x.EdpCode == edpcode).ToList();

                foreach (_212ostsp ost in ostspDetail)
                {
                    var studOstsp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == ost.StudId).FirstOrDefault();

                    if (studOstsp != null)
                    {
                        if (studOstsp.Section != null && !studOstsp.Section.Equals(String.Empty))
                        {
                            if (ost.Status == 3)
                                official++;
                            else
                                pending++;
                        }
                        else
                        {
                            if (ost.Status == 3)
                                official++;
                            else if (ost.Status == 1)
                                pending++;
                        }
                    }
                }

                var schedule = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == edpcode).FirstOrDefault();

                if (schedule != null)
                {
                    schedule.Size = official + pending;
                    schedule.PendingEnrolled = pending;
                    schedule.OfficialEnrolled = official;
                }

                _ucOnlinePortalContext._212schedules.Update(schedule);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new RemoveDuplicateOtspResponse { success = 1 };
        }

        /*
       * Method to view classlist
       */
        public ViewClasslistPerSectionResponse ViewClasslistPerSection(ViewClasslistPerSectionRequest viewClasslistPerSectionRequest)
        {
            var courseDesc = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == viewClasslistPerSectionRequest.course_code).FirstOrDefault();

            var result = (from _oenrp in _ucOnlinePortalContext._212oenrps
                          join _login in _ucOnlinePortalContext.LoginInfos
                          on _oenrp.StudId equals _login.StudId
                          join _courselist in _ucOnlinePortalContext.CourseLists
                          on _login.CourseCode equals _courselist.CourseCode
                          where _oenrp.Section == viewClasslistPerSectionRequest.section
                          && _oenrp.CourseCode == viewClasslistPerSectionRequest.course_code
                          select new ViewClasslistPerSectionResponse.Enrolled
                          {
                              id_number = _login.StudId,
                              last_name = _login.LastName,
                              firstname = _login.FirstName,
                              course_year = _courselist.CourseAbbr + " " + _login.Year,
                              mobile_number = _login.MobileNumber,
                              email = _login.Email,
                              status = _oenrp.Status
                          });

            ViewClasslistPerSectionResponse response = new ViewClasslistPerSectionResponse
            {
                course = courseDesc.CourseDescription,
                section = viewClasslistPerSectionRequest.section,
                section_size = result.Count().ToString(),
                assigned_section = result.ToList()
            };

            return response;
        }

        /*
       * Method to view classlist
       */
        public UpdateStudentStatusResponse UpdateStudentStatus(UpdateStudentStatusRequest updateStudentStatusRequest)
        {
            var studOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == updateStudentStatusRequest.id_number).FirstOrDefault();

            if (studOenrp != null)
            {
                if (studOenrp.Status < updateStudentStatusRequest.new_status)
                {
                    return new UpdateStudentStatusResponse { success = 0 };
                }
                else
                {
                    studOenrp.Status = (short)updateStudentStatusRequest.new_status;

                    if (updateStudentStatusRequest.new_status == 0)
                    {
                        studOenrp.ApprovedRegDean = null;
                        studOenrp.ApprovedRegDeanOn = null;
                        studOenrp.DisapprovedRegDean = null;
                        studOenrp.DisapprovedRegDeanOn = null;
                        studOenrp.ApprovedRegRegistrar = null;
                        studOenrp.ApprovedRegRegistrarOn = null;
                        studOenrp.DisapprovedRegRegistrar = null;
                        studOenrp.DisapprovedRegRegistrarOn = null;
                        studOenrp.ApprovedDean = null;
                        studOenrp.ApprovedDeanOn = null;
                        studOenrp.DisapprovedDean = null;
                        studOenrp.DisapprovedDeanOn = null;
                        studOenrp.ApprovedAcctg = null;
                        studOenrp.ApprovedAcctgOn = null;
                        studOenrp.NeededPayment = 0;
                        studOenrp.ApprovedCashier = null;
                        studOenrp.ApprovedCashierOn = null;
                        studOenrp.EnrollmentDate = null;
                        studOenrp.SubmittedOn = null;
                    }
                    else if (updateStudentStatusRequest.new_status == 1)
                    {
                        studOenrp.ApprovedRegDean = null;
                        studOenrp.ApprovedRegDeanOn = null;
                        studOenrp.DisapprovedRegDean = null;
                        studOenrp.DisapprovedRegDeanOn = null;
                        studOenrp.ApprovedDean = null;
                        studOenrp.ApprovedDeanOn = null;
                        studOenrp.DisapprovedDean = null;
                        studOenrp.DisapprovedDeanOn = null;
                        studOenrp.ApprovedAcctg = null;
                        studOenrp.ApprovedAcctgOn = null;
                        studOenrp.NeededPayment = 0;
                        studOenrp.ApprovedCashier = null;
                        studOenrp.ApprovedCashierOn = null;
                        studOenrp.EnrollmentDate = null;
                        studOenrp.SubmittedOn = null;
                    }
                    else if (updateStudentStatusRequest.new_status == 3)
                    {
                        studOenrp.ApprovedDean = null;
                        studOenrp.ApprovedDeanOn = null;
                        studOenrp.DisapprovedDean = null;
                        studOenrp.DisapprovedDeanOn = null;
                        studOenrp.ApprovedAcctg = null;
                        studOenrp.ApprovedAcctgOn = null;
                        studOenrp.NeededPayment = 0;
                        studOenrp.ApprovedCashier = null;
                        studOenrp.ApprovedCashierOn = null;
                        studOenrp.EnrollmentDate = null;
                        studOenrp.SubmittedOn = null;
                    }
                    else if (updateStudentStatusRequest.new_status == 5)
                    {
                        studOenrp.ApprovedDean = null;
                        studOenrp.ApprovedDeanOn = null;
                        studOenrp.ApprovedAcctg = null;
                        studOenrp.ApprovedAcctgOn = null;
                        studOenrp.NeededPayment = 0;
                        studOenrp.ApprovedCashier = null;
                        studOenrp.ApprovedCashierOn = null;
                    }
                    else if (updateStudentStatusRequest.new_status == 6)
                    {
                        studOenrp.ApprovedAcctg = null;
                        studOenrp.ApprovedAcctgOn = null;
                        studOenrp.NeededPayment = 0;
                        studOenrp.ApprovedCashier = null;
                        studOenrp.ApprovedCashierOn = null;
                    }
                    else if (updateStudentStatusRequest.new_status == 8)
                    {
                        studOenrp.ApprovedCashier = null;
                        studOenrp.ApprovedCashierOn = null;
                    }

                    var hasOstsp = _ucOnlinePortalContext._212ostsps.Where(x => x.StudId == updateStudentStatusRequest.id_number).ToList();

                    if (hasOstsp != null && (updateStudentStatusRequest.new_status < 3))
                    {
                        foreach (_212ostsp ostsp in hasOstsp)
                        {
                            var schedule = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == ostsp.EdpCode).FirstOrDefault();

                            if (ostsp.Status == 1)
                            {
                                schedule.Size = schedule.Size - 1;
                                schedule.PendingEnrolled = schedule.PendingEnrolled - 1;
                            }
                            else if (ostsp.Status == 3)
                            {
                                schedule.Size = schedule.Size - 1;
                                schedule.OfficialEnrolled = schedule.OfficialEnrolled - 1;
                            }

                            if (studOenrp.Section != null && !studOenrp.Section.Equals(String.Empty))
                            {
                                if (ostsp.Status == 0)
                                {
                                    schedule.Size = schedule.Size - 1;
                                    schedule.PendingEnrolled = schedule.PendingEnrolled - 1;
                                }
                            }

                            _ucOnlinePortalContext._212schedules.Update(schedule);
                            _ucOnlinePortalContext.SaveChanges();
                        }


                        studOenrp.Section = null;
                        _ucOnlinePortalContext.SaveChanges();

                        _ucOnlinePortalContext._212ostsps.RemoveRange(_ucOnlinePortalContext._212ostsps.Where(x => x.StudId == updateStudentStatusRequest.id_number));
                    }
                }

                _ucOnlinePortalContext._212oenrps.Update(studOenrp);
                _ucOnlinePortalContext.SaveChanges();
            }
            else
            {
                return new UpdateStudentStatusResponse { success = 0 };
            }

            return new UpdateStudentStatusResponse { success = 1 };
        }

        /*
       * Method to view classlist
       */
        public ManualEnrollmentResponse ManualEnrollment(ManualEnrollmentRequest manualEnrollmentRequest)
        {
            var studentOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == manualEnrollmentRequest.id_number);

            if (studentOenrp == null)
            {
                return new ManualEnrollmentResponse { success = 0 };
            }
            else
            {
                string edp_code = manualEnrollmentRequest.edp_codes;
                string[] split_edp = edp_code.Split(',');

                if (split_edp.Length > 1)
                {

                    foreach (string os in split_edp)
                    {
                        _212ostsp ostpN = new _212ostsp
                        {
                            StudId = manualEnrollmentRequest.id_number,
                            EdpCode = os,
                            Status = 0,
                            Remarks = null,
                            AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                        };

                        _ucOnlinePortalContext._212ostsps.Add(ostpN);
                        _ucOnlinePortalContext.SaveChanges();
                    }
                }
                else
                {
                    _212ostsp ostpN = new _212ostsp
                    {
                        StudId = manualEnrollmentRequest.id_number,
                        EdpCode = edp_code,
                        Status = 0,
                        Remarks = null,
                        AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                    };

                    _ucOnlinePortalContext._212ostsps.Add(ostpN);
                    _ucOnlinePortalContext.SaveChanges();
                }
            }
            return new ManualEnrollmentResponse { success = 1 };
        }

        public RequestPromissoryResponse RequestPromissory(RequestPromissoryRequest requestPromissoryRequest)
        {
            var studOenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == requestPromissoryRequest.stud_id).FirstOrDefault();

            if (studOenrp == null)
            {
                return new RequestPromissoryResponse { success = 0 };
            }
            else
            {
                double payPercent = studOenrp.NeededPayment.Value * .30;

                if (requestPromissoryRequest.promise_pay >= payPercent)
                {
                    studOenrp.RequestPromissory = 1;
                }
                else
                {
                    studOenrp.RequestPromissory = 2;
                }

                studOenrp.PromiPay = requestPromissoryRequest.promise_pay;

                _212promissory newProm = new _212promissory
                {
                    StudId = requestPromissoryRequest.stud_id,
                    PromiMessage = requestPromissoryRequest.message,
                    PromiDate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                };

                _ucOnlinePortalContext.Update(studOenrp);
                _ucOnlinePortalContext._212promissories.Add(newProm);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new RequestPromissoryResponse { success = 1 }; ;
        }

        public GetPromissoryListResponse GetPromissoryList(GetPromissoryListRequest getPromissoryListRequest)
        {
            int take = (int)getPromissoryListRequest.limit;
            int skip = (int)getPromissoryListRequest.limit * ((int)getPromissoryListRequest.page - 1);

            //always get status 5 -> for dean
            var result = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                          join _loginInfo in _ucOnlinePortalContext.LoginInfos
                          on _212oenrp.StudId equals _loginInfo.StudId
                          join _courseList in _ucOnlinePortalContext.CourseLists
                          on _212oenrp.CourseCode equals _courseList.CourseCode
                          join _promi in _ucOnlinePortalContext._212promissories
                          on _212oenrp.StudId equals _promi.StudId
                          where (_212oenrp.Status == 8 || _212oenrp.Status == 10) && _212oenrp.RequestPromissory == getPromissoryListRequest.status
                          select new GetPromissoryListResponse.Student
                          {
                              id_number = _212oenrp.StudId,
                              lastname = _loginInfo.LastName,
                              firstname = _loginInfo.FirstName,
                              mi = _loginInfo.Mi,
                              suffix = _loginInfo.Suffix,
                              classification = Utils.Function.getClassification(_212oenrp.Classification),
                              course_year = _courseList.CourseAbbr + " " + _212oenrp.YearLevel,
                              course_code = _212oenrp.CourseCode,
                              status = (short)_212oenrp.RequestPromissory,
                              date = _promi.PromiDate,
                              message = _promi.PromiMessage,
                              promise_pay = _212oenrp.PromiPay.Value,
                              needed_payment = _212oenrp.NeededPayment.Value
                          });

            if (!getPromissoryListRequest.course_department.Equals(String.Empty))
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == getPromissoryListRequest.course_department).ToList();
                var courses = courseList.Select(x => x.CourseCode).ToList();

                result = result.Where(x => courses.Contains(x.course_code));
            }

            var count = result.Count();

            if (getPromissoryListRequest.page != 0 && getPromissoryListRequest.limit != 0)
            {
                result = result.OrderBy(x => x.date).Skip(skip).Take(take);
            }

            return new GetPromissoryListResponse { students = result.ToList(), count = count };
        }

        public CorrectTotalUnitsResponse CorrectTotalUnits()
        {
            int[] enrStat = { 6, 8, 10 };

            var studOenp = _ucOnlinePortalContext._212oenrps.Where(x => enrStat.Contains(x.Status)).ToList();

            foreach (_212oenrp _212Oenrp in studOenp)
            {
                //always get status 5 -> for dean
                var result = (from _212ostsp in _ucOnlinePortalContext._212ostsps
                              join _212schedule in _ucOnlinePortalContext._212schedules
                              on _212ostsp.EdpCode equals _212schedule.EdpCode
                              where _212ostsp.StudId == _212Oenrp.StudId &&
                              (_212ostsp.Status == 1 || _212ostsp.Status == 3)
                              select new
                              {
                                  units = _212schedule.Units
                              }).ToList();

                decimal totalUnits = result.Select(x => Convert.ToDecimal(x.units)).Sum();

                var studO = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == _212Oenrp.StudId).FirstOrDefault();

                studO.Units = (short)totalUnits;
                _ucOnlinePortalContext.Update(studO);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new CorrectTotalUnitsResponse { success = 1 };
        }

        public AddNotificationResponse AddNotification(AddNotificationRequest addNotificationRequest)
        {
            _212notification newNotification = new _212notification
            {
                StudId = addNotificationRequest.stud_id,
                NotifRead = 0,
                Dte = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                Message = addNotificationRequest.from_sender + ":" + addNotificationRequest.message
            };

            _ucOnlinePortalContext._212notifications.Add(newNotification);
            _ucOnlinePortalContext.SaveChanges();

            return new AddNotificationResponse { success = 1 };
        }

        public UpdateInfoResponse GetInfoUpdate(UpdateInfoRequest updateInfoRequest)
        {
            var loginInfo = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == updateInfoRequest.stud_id).FirstOrDefault();

            if (loginInfo != null)
            {
                UpdateInfoResponse response = new UpdateInfoResponse
                {
                    first_name = loginInfo.FirstName,
                    last_name = loginInfo.LastName,
                    middle_initial = loginInfo.Mi,
                    year_level = (short)loginInfo.Year,
                    dept = loginInfo.Dept,
                    course_code = loginInfo.CourseCode
                };

                var classification = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == updateInfoRequest.stud_id).FirstOrDefault();

                if (classification != null)
                    response.classification = classification.Classification;

                return response;
            }
            else
            {
                return null;
            }
        }

        public UpdateInforResponse UpdateInfor(UpdateInforRequest updateInforRequest)
        {
            var loginInfo = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == updateInforRequest.stud_id).FirstOrDefault();

            if (loginInfo != null)
            {
                loginInfo.FirstName = updateInforRequest.first_name;
                loginInfo.LastName = updateInforRequest.last_name;
                loginInfo.Mi = updateInforRequest.middle_initial;
                loginInfo.Year = (short)updateInforRequest.year_level;
                loginInfo.Dept = updateInforRequest.dept;
                loginInfo.CourseCode = updateInforRequest.course_code;

                _ucOnlinePortalContext.LoginInfos.Update(loginInfo);
                _ucOnlinePortalContext.SaveChanges();
            }

            var oenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == updateInforRequest.stud_id).FirstOrDefault();

            if (oenrp != null)
            {
                oenrp.Dept = updateInforRequest.dept;
                oenrp.CourseCode = updateInforRequest.course_code;
                oenrp.YearLevel = (short)updateInforRequest.year_level;
                oenrp.Classification = updateInforRequest.classification;


                _ucOnlinePortalContext.Update(oenrp);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new UpdateInforResponse { success = 1 };
        }

        public SetClosedSubjectResponse SetClosed()
        {
            var scheduleClosed = _ucOnlinePortalContext._212schedules.Where(x => x.MaxSize == x.Size).ToList();

            scheduleClosed.ForEach(x => x.Status = 5);

            _ucOnlinePortalContext.SaveChanges();

            return new SetClosedSubjectResponse { success = 1 };
        }

        public GetTeachersListResponse GetTeachersList(GetTeachersListRequest getTeachersListRequest)
        {
            var TeachersList = _ucOnlinePortalContext.LoginInfos.Where(x => x.UserType.Contains("FACULTY"));

            if (!getTeachersListRequest.id_number.Equals(String.Empty))
            {
                TeachersList = TeachersList.Where(x => x.StudId == getTeachersListRequest.id_number);
            }

            if (!getTeachersListRequest.id_number.Equals(String.Empty))
            {
                TeachersList = TeachersList.Where(x => (x.FirstName + x.LastName).Contains(getTeachersListRequest.name));
            }

            List<GetTeachersListResponse.Teachers> teachers = new List<GetTeachersListResponse.Teachers>();
            teachers = TeachersList.Select(x => new GetTeachersListResponse.Teachers
            {
                id_number = x.StudId,
                first_name = x.FirstName,
                last_name = x.LastName
            }).ToList();

            return new GetTeachersListResponse { teacherList = teachers };
        }

        public SaveTeachersLoadResponse SaveTeachersLoad(SaveTeachersLoadRequest saveTeachersLoadRequest)
        {
            var ostspSelected = _ucOnlinePortalContext._212schedules.Where(x => x.Instructor == saveTeachersLoadRequest.id_number).Select(x => x.EdpCode).ToList();

            var toDelete = ostspSelected.Except(saveTeachersLoadRequest.edp_codes).ToList();
            var toAdd = saveTeachersLoadRequest.edp_codes.Except(ostspSelected).ToList();

            if (toDelete.Count > 0)
            {
                foreach (string edp_code in toDelete)
                {
                    var removeInstructor = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == edp_code).FirstOrDefault();

                    removeInstructor.Instructor = "";

                    _ucOnlinePortalContext._212schedules.Update(removeInstructor);
                    _ucOnlinePortalContext.SaveChanges();
                }
            }

            if (toAdd.Count > 0)
            {
                foreach (string edp_code in toAdd)
                {
                    var removeInstructor = _ucOnlinePortalContext._212schedules.Where(x => x.EdpCode == edp_code).FirstOrDefault();

                    removeInstructor.Instructor = saveTeachersLoadRequest.id_number;

                    _ucOnlinePortalContext._212schedules.Update(removeInstructor);
                    _ucOnlinePortalContext.SaveChanges();
                }
            }

            return new SaveTeachersLoadResponse { success = 1 };
        }

        public GetTeachersLoadResponse GetTeachersLoad(GetTeachersLoadRequest getTeachersLoadRequest)
        {
            //Get user data
            var teachersLoad = _ucOnlinePortalContext._212schedules.Where(x => x.Instructor == getTeachersLoadRequest.id_number);

            //check if the the data exist
            if (teachersLoad == null)
            {
                //return empty data
                return null;
            }
            else
            {
                //Get data from _212ostsp and _212schedules
                var result = (from _212schedules in _ucOnlinePortalContext._212schedules
                              join _subject_info in _ucOnlinePortalContext.SubjectInfos
                              on _212schedules.InternalCode equals _subject_info.InternalCode into sched
                              from _subject_info in sched.DefaultIfEmpty()
                              join _courselist in _ucOnlinePortalContext.CourseLists
                              on _212schedules.CourseCode equals _courselist.CourseCode into course
                              from _courselist in course.DefaultIfEmpty()
                              where _212schedules.Instructor == getTeachersLoadRequest.id_number
                              select new GetTeachersLoadResponse.Schedules
                              {
                                  edpcode = _212schedules.EdpCode,
                                  subject_name = _212schedules.Description,
                                  subject_type = _212schedules.SubType,
                                  days = _212schedules.Days,
                                  begin_time = _212schedules.TimeStart,
                                  end_time = _212schedules.TimeEnd,
                                  mdn = _212schedules.Mdn,
                                  m_begin_time = _212schedules.MTimeStart,
                                  m_end_time = _212schedules.MTimeEnd,
                                  m_days = _212schedules.MDays,
                                  size = _212schedules.Size,
                                  max_size = _212schedules.MaxSize,
                                  units = _212schedules.Units,
                                  room = _212schedules.Room,
                                  descriptive_title = _subject_info.Descr1 + _subject_info.Descr2,
                                  split_code = _212schedules.SplitCode,
                                  split_type = _212schedules.SplitType,
                                  section = _212schedules.Section,
                                  course_abbr = _courselist.CourseAbbr
                              }).ToList();

                return new GetTeachersLoadResponse { schedules = result };
            }
        }

        public SaveAdjustmentResponse SaveAdjustment(SaveAdjustmentRequest saveAdjustmentRequest)
        {
            var oenrp = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == saveAdjustmentRequest.id_number).FirstOrDefault();

            if (oenrp != null)
            {
                if (oenrp.AdjustmentCount == 0)
                {
                    return new SaveAdjustmentResponse { success = 0 };
                }
                else
                {
                    if (saveAdjustmentRequest.addEdpCodes.Length > 0)
                    {
                        foreach (string edp_code in saveAdjustmentRequest.addEdpCodes)
                        {

                            _212ostsp ostpN = new _212ostsp
                            {
                                StudId = saveAdjustmentRequest.id_number,
                                EdpCode = edp_code,
                                Status = 4,
                                Remarks = null,
                                AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                            };

                            _ucOnlinePortalContext._212ostsps.Add(ostpN);
                            _ucOnlinePortalContext.SaveChanges();
                        }
                    }

                    if (saveAdjustmentRequest.deleteEdpCodes.Length > 0)
                    {
                        foreach (string edp_code in saveAdjustmentRequest.deleteEdpCodes)
                        {
                            _212ostsp ostpN = new _212ostsp
                            {
                                StudId = saveAdjustmentRequest.id_number,
                                EdpCode = edp_code,
                                Status = 5,
                                Remarks = null,
                                AdjustedOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"))
                            };

                            _ucOnlinePortalContext._212ostsps.Add(ostpN);
                            _ucOnlinePortalContext.SaveChanges();
                        }
                    }

                    oenrp.AdjustmentCount = (short)(oenrp.AdjustmentCount - 1);
                    oenrp.AdjustmentOn = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    _ucOnlinePortalContext._212oenrps.Update(oenrp);
                    _ucOnlinePortalContext.SaveChanges();
                }
            }

            return new SaveAdjustmentResponse { success = 1 };
        }

        public GetAdjustmentListResponse GetAdjustmentlist(GetAdjustmentListRequest getAdjustmentListRequest)
        {
            int take = (int)getAdjustmentListRequest.limit;
            int skip = (int)getAdjustmentListRequest.limit * ((int)getAdjustmentListRequest.page - 1);

            //always get status 5 -> for dean
            var result = (from _212oenrp in _ucOnlinePortalContext._212oenrps
                          join _loginInfo in _ucOnlinePortalContext.LoginInfos
                          on _212oenrp.StudId equals _loginInfo.StudId
                          join _courseList in _ucOnlinePortalContext.CourseLists
                          on _212oenrp.CourseCode equals _courseList.CourseCode
                          where _212oenrp.AdjustmentCount == getAdjustmentListRequest.status && _212oenrp.Status >= 6
                          select new GetAdjustmentListResponse.Student
                          {
                              id_number = _212oenrp.StudId,
                              lastname = _loginInfo.LastName,
                              firstname = _loginInfo.FirstName,
                              mi = _loginInfo.Mi,
                              suffix = _loginInfo.Suffix,
                              classification = Utils.Function.getClassification(_212oenrp.Classification),
                              course_year = _courseList.CourseAbbr + " " + _212oenrp.YearLevel,
                              course_code = _212oenrp.CourseCode,
                              status = (short)_212oenrp.AdjustmentCount,
                              date = _212oenrp.EnrollmentDate.Value
                          });

            if (!getAdjustmentListRequest.course_department.Equals(String.Empty))
            {
                var courseList = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseDepartmentAbbr == getAdjustmentListRequest.course_department).ToList();
                var courses = courseList.Select(x => x.CourseCode).ToList();

                result = result.Where(x => courses.Contains(x.course_code));
            }

            var count = result.Count();

            if (getAdjustmentListRequest.page != 0 && getAdjustmentListRequest.limit != 0)
            {
                result = result.OrderBy(x => x.date).Skip(skip).Take(take);
            }

            return new GetAdjustmentListResponse { students = result.ToList(), count = count };
        }

        public GetCurriculumResponse GetCurriculum(GetCurriculumRequest getCurriculumRequest)  //id_number
        {
            //getStudentCourse 
            var getStudentInfo = _ucOnlinePortalContext._212studentInfos.Where(x => x.StudId == getCurriculumRequest.id_number).FirstOrDefault();
            //check if the the data exist
            
            if (getStudentInfo == null)
            {
                //return empty data
                return new GetCurriculumResponse { };
            }
            else
            {
                var getUnits = 0;
                var getStudent_oenrp = _ucOnlinePortalContext._212oenrps.Where(oenrps => oenrps.StudId == getCurriculumRequest.id_number).FirstOrDefault();
                if (getStudent_oenrp != null) { //getStudent_ornrp.Units 0
                    getUnits = getStudent_oenrp.Units;
                }
                //var getClassification
                var getYear = _ucOnlinePortalContext.Curricula.Where(cur => cur.IsDeployed == 1).Select(cur => cur.Year).Min();

                if (getStudentInfo.Classification == "H") {
                    getYear = _ucOnlinePortalContext.Curricula.Where(cur => cur.IsDeployed == 1).Select(cur => cur.Year).Max();
                }

                if (getYear == null)
                    return new GetCurriculumResponse { };

                var subjects = (from subject in _ucOnlinePortalContext.SubjectInfos
                                join curriculum in _ucOnlinePortalContext.Curricula
                                on subject.CurriculumYear equals curriculum.Year
                                where (subject.CourseCode == getStudentInfo.CourseCode && subject.CurriculumYear == getYear)
                                select new GetCurriculumResponse.Subjects
                                {
                                    internal_code = subject.InternalCode,
                                    subject_name = subject.SubjectName,
                                    subject_type = subject.SubjectType,
                                    descr_1 = subject.Descr1,
                                    descr_2 = subject.Descr2,
                                    units = Convert.ToString(subject.Units),
                                    semester = Convert.ToString(subject.Semester),
                                    year_level = subject.YearLevel,
                                    course_code = subject.CourseCode,
                                    split_code = subject.SplitCode,
                                    split_type = subject.SplitType
                                }).ToList();

                var remarks = (from remark in _ucOnlinePortalContext.Prerequisites
                               join subject in _ucOnlinePortalContext.SubjectInfos
                               on remark.Prerequisites equals subject.InternalCode
                               select new GetCurriculumResponse.Prerequisites
                               {
                                   internal_code = remark.InternalCode,
                                   subject_code = subject.SubjectName,
                                   prerequisites = remark.Prerequisites
                               }).ToList();

                var grades = (from subject in _ucOnlinePortalContext.SubjectInfos
                              join grade in _ucOnlinePortalContext.GradeEvaluations
                              on subject.InternalCode equals grade.IntCode
                              where grade.StudId == getCurriculumRequest.id_number
                              select new GetCurriculumResponse.Grades
                              {
                                  internal_code = grade.IntCode,
                                  eval_id = grade.GradeEvalId,
                                  subject_code = subject.SubjectName,
                                  final_grade = grade.FinalGrade
                              }).ToList();

                var schedules = (from schedule in _ucOnlinePortalContext._212schedules
                                 select new GetCurriculumResponse.Schedules
                                 {
                                     internal_code = schedule.InternalCode,
                                     edp_code = schedule.EdpCode,
                                     subject_code = schedule.Description,
                                     subject_type = schedule.SubType,
                                     time_start = schedule.TimeStart,
                                     time_end = schedule.TimeEnd,
                                     mdn = schedule.Mdn,
                                     days = schedule.Days,
                                     split_type = schedule.SplitType,
                                     split_code = schedule.SplitCode,
                                     course_code = schedule.CourseCode,
                                     section = schedule.Section,
                                     room = schedule.Room
                                 }).ToList();

                //var subjects = getStudentCourse.CourseCode;
                return new GetCurriculumResponse { subjects = subjects, course_code = getYear.ToString(), prerequisites = remarks, grades = grades, schedules = schedules,units = getUnits };
            }
        }


        public StudentSubjectResponse RequestSubject(StudentSubjectRequest studentRequest)  //id_number
        {
            if (studentRequest == null)
            {
                return new StudentSubjectResponse { success = 0 };
            }

            var getSubjectInfo = _ucOnlinePortalContext.SubjectInfos.Where(x => x.InternalCode == studentRequest.internal_code).FirstOrDefault();

            if (getSubjectInfo == null)
            {
                return new StudentSubjectResponse { success = 0 };
            }

            var checkIfExistSubjectRequest = _ucOnlinePortalContext.RequestSchedules.Where(x => x.InternalCode == studentRequest.internal_code).FirstOrDefault();

            if (checkIfExistSubjectRequest == null) {
                RequestSchedule subjectRequest = new RequestSchedule
                {
                    SubjectName = getSubjectInfo.SubjectName,
                    TimeStart = studentRequest.time_start,
                    TimeEnd = studentRequest.time_end,
                    Mdn = studentRequest.mdn,
                    Days = studentRequest.days,
                    Rtype = studentRequest.rtype,
                    MTimeEnd = studentRequest.m_time_start,
                    MTimeStart = studentRequest.m_time_end,
                    Status = 0,
                    InternalCode = studentRequest.internal_code
                };
                _ucOnlinePortalContext.RequestSchedules.Add(subjectRequest);
            }
            var checkIfExist = _ucOnlinePortalContext.StudentRequests.Where(x => x.InternalCode == studentRequest.internal_code && x.StudId == studentRequest.id_number).FirstOrDefault();

            if (checkIfExist == null) {
                StudentRequest studentSubjectRequest = new StudentRequest
                {
                    StudId = studentRequest.id_number,
                    InternalCode = studentRequest.internal_code
                };
                _ucOnlinePortalContext.StudentRequests.Add(studentSubjectRequest);
            }
            
            _ucOnlinePortalContext.SaveChanges();
            return new StudentSubjectResponse { success = 1 };
        }
        public GetSubjectReqResponse GetRequestSubject(GetSubjectReqRequest getRequest)
        {
            if (getRequest == null)
            {
                return new GetSubjectReqResponse { };
            }

            var request = (from rsubjects in _ucOnlinePortalContext.RequestSchedules
                           join subject in _ucOnlinePortalContext.SubjectInfos
                           on rsubjects.InternalCode equals subject.InternalCode
                           where subject.CourseCode == getRequest.course_code
                           select new GetSubjectReqResponse.RequestedSubject
                           {
                               subject_name = rsubjects.SubjectName,
                               desc_1 = subject.Descr1,
                               desc_2 = subject.Descr2,
                               time_start = rsubjects.TimeStart,
                               time_end = rsubjects.TimeEnd,
                               mdn = rsubjects.Mdn,
                               days = rsubjects.Days,
                               rtype = rsubjects.Rtype,
                               m_time_start = rsubjects.MTimeStart,
                               m_time_end = rsubjects.MTimeEnd,
                               status = rsubjects.Status,
                               internal_code = rsubjects.InternalCode
                           }).ToList();

            return new GetSubjectReqResponse { request = request };
        }

        public GetStudentReqResponse GetStudentSubjectRequest(GetStudentReqRequest getRequest)
        {
            if (getRequest == null)
            {
                return new GetStudentReqResponse { };
            }

            var request = (from rsubjects in _ucOnlinePortalContext.RequestSchedules
                           join subject in _ucOnlinePortalContext.StudentRequests
                           on rsubjects.InternalCode equals subject.InternalCode
                           where subject.StudId == getRequest.id_number
                           select new GetStudentReqResponse.RequestedSubject
                           {
                               subject_name = rsubjects.SubjectName,
                               time_start = rsubjects.TimeStart,
                               time_end = rsubjects.TimeEnd,
                               mdn = rsubjects.Mdn,
                               days = rsubjects.Days,
                               rtype = rsubjects.Rtype,
                               m_time_start = rsubjects.MTimeStart,
                               m_time_end = rsubjects.MTimeEnd,
                               status = rsubjects.Status,
                               internal_code = rsubjects.InternalCode
                           }).ToList();
            
            var studentInfo = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == getRequest.id_number).FirstOrDefault();

            var filtered = (from subject in _ucOnlinePortalContext.SubjectInfos
                            where subject.CourseCode == studentInfo.CourseCode
                            select new GetStudentReqResponse.FilteredSubject
                            {
                                subject_name = subject.SubjectName,
                                internal_code = subject.InternalCode
                            }).ToList();

            var schedulesOstp = _ucOnlinePortalContext.RequestSchedules.ToList();
            var selected = schedulesOstp.Select(x => x.InternalCode).ToList();


            return new GetStudentReqResponse { request = request, filtered = filtered };
        }

        public AddStudentReqResponse AddSubjectRequest(AddStudentReqRequest getRequest)
        {
            if (getRequest == null)
                return new AddStudentReqResponse { success = 0 };

            StudentRequest studentSubjectRequest = new StudentRequest
            {
                StudId = getRequest.id_number,
                InternalCode = getRequest.internal_code
            };
            _ucOnlinePortalContext.StudentRequests.Add(studentSubjectRequest);
            _ucOnlinePortalContext.SaveChanges();

            return new AddStudentReqResponse { success = 1 };
        }

        public CancelSubjectReqResponse CancelSubjectRequest(CancelSubjectReqRequest getRequest)
        {
            if (getRequest == null)
                return new CancelSubjectReqResponse { success = 0 };

            var findTmpLogin = _ucOnlinePortalContext.StudentRequests.Where(x => x.StudId == getRequest.id_number && x.InternalCode == getRequest.internal_code).FirstOrDefault();
            _ucOnlinePortalContext.StudentRequests.Remove(findTmpLogin);
            _ucOnlinePortalContext.SaveChanges();

            return new CancelSubjectReqResponse { success = 1 };
        }
        public GetAllCurriculumResponse GetAllCurriculum()
        {
            var year = (from curriculum in _ucOnlinePortalContext.Curricula
                            select new GetAllCurriculumResponse.SchoolYear
                            {
                                year = curriculum.Year,
                                isDeployed = curriculum.IsDeployed
                            }).ToList();
            var courses = (from course in _ucOnlinePortalContext.CourseLists
                            select new GetAllCurriculumResponse.Courses
                            {
                                course_code = course.CourseCode,
                                course_description = course.CourseDescription,
                                course_abbr = course.CourseAbbr,
                                course_year_limit = course.CourseYearLimit,
                                course_department = course.CourseDepartment,
                                course_department_abbr = course.CourseDepartmentAbbr,
                                course_active = course.CourseActive,
                                department = course.Department,
                                enrollment_open = course.EnrollmentOpen
                            }).ToList();
            var subjects = (from subject in _ucOnlinePortalContext.SubjectInfos
                            select new GetAllCurriculumResponse.Subjects
                            {
                                internal_code = subject.InternalCode,
                                subject_name = subject.SubjectName,
                                subject_type = subject.SubjectType,
                                descr_1 = subject.Descr1,
                                descr_2 = subject.Descr2,
                                units = Convert.ToString(subject.Units),
                                semester = Convert.ToString(subject.Semester),
                                course_code = subject.CourseCode,
                                year_level = Convert.ToString(subject.YearLevel),
                                split_type = subject.SplitType,
                                split_code = subject.SplitCode,
                                //curriculim_year = subject.CurriculumYear
                               
                            }).ToList();

            var departments = (from department in _ucOnlinePortalContext.CourseLists
                            select new GetAllCurriculumResponse.Departments
                            {
                                course_department = department.CourseDepartment,
                                course_department_abbr = department.CourseDepartmentAbbr,
                                department = department.Department
                            }).Distinct().ToList();


            //var group_departments = departments
            return new GetAllCurriculumResponse { year = year, courses = courses,  subjects = subjects, departments = departments};
        }
    }
}
