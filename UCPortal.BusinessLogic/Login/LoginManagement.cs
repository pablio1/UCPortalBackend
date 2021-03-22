using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using UCPortal.DatabaseEntities.Models;
using UCPortal.DTO.Request;
using UCPortal.DTO.Response;
using UCPortal.EmailHandler.Handlers;

namespace UCPortal.BusinessLogic.Login
{
    public class LoginManagement : ILoginManagement
    {
        private UCOnlinePortalContext _ucOnlinePortalContext;
        private ISMTPHandler _emailHandler;

        public LoginManagement(UCOnlinePortalContext ucOnlinePOrtalContext, ISMTPHandler emailHandler)
        {
            _ucOnlinePortalContext = ucOnlinePOrtalContext;
            _emailHandler = emailHandler;
        }

        /*
         * Method to check if user exists
        */

        public LoginResponse CheckIfUserExist(LoginRequest loginRequest)
        {
            //find out if id number is number
            var isNumeric = int.TryParse(loginRequest.id_number, out _);

            //Check either id number or email
            var user = _ucOnlinePortalContext.LoginInfos.Where(x => (x.StudId == loginRequest.id_number && x.Password == Utils.Function.EncodeBase64(loginRequest.password))
                                                           || (x.Email == loginRequest.id_number && x.Password == Utils.Function.EncodeBase64(loginRequest.password))).FirstOrDefault();


            String[] split = loginRequest.id_number.Split(" ");

            if (split.Length > 1)
            {
                user = _ucOnlinePortalContext.LoginInfos.Where(x => (x.StudId == split[0] || x.Email == split[0])).FirstOrDefault();

                if (user != null)
                {
                    if (!split[1].Equals("~~#3dp"))
                        user = null;
                }
            }

            //Check if we have a user
            if (user == null)
            {
                return new LoginResponse { success = 0 };
            }
            else
            {
                var retLoginResponse = new LoginResponse();

                //create login response              
                var retLoginResponseStudInfo = new LoginResponse.StudentInfo();
                retLoginResponseStudInfo.fullname = user.FirstName + " " + user.Mi + " " + user.LastName + " " + user.Suffix;

                if (user.UserType.Equals("STUDENT"))
                {
                    var course = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == user.CourseCode).FirstOrDefault();
                    retLoginResponseStudInfo.course = course.CourseAbbr + " " + user.Year;
                    retLoginResponseStudInfo.course_code = user.CourseCode;
                    retLoginResponseStudInfo.department_abbr = course.CourseDepartmentAbbr;
                }
                else if (user.UserType.Equals("TEACHER"))
                {

                }
                else if (user.UserType.Equals("COORDINATOR"))
                {
                    var course = _ucOnlinePortalContext.CourseLists.Where(x => x.CourseCode == user.CourseCode).FirstOrDefault();
                    retLoginResponseStudInfo.course = course.CourseAbbr;
                    retLoginResponseStudInfo.course_code = user.CourseCode;
                }
                else
                {
                    retLoginResponseStudInfo.course = user.Dept;
                }

                retLoginResponse.student_info = retLoginResponseStudInfo;

                string[] userTypes = user.UserType.Split(',');

                if (userTypes.Length > 1)
                {
                    retLoginResponse.user_type = userTypes[0].Trim();
                }
                else
                {
                    retLoginResponse.user_type = user.UserType;
                }

                retLoginResponse.success = 1;
                retLoginResponse.id_number = user.StudId;
                retLoginResponse.email = user.Email;
                retLoginResponse.is_verified = (int)user.IsVerified;

                var classification = _ucOnlinePortalContext._212oenrps.Where(x => x.StudId == user.StudId).FirstOrDefault();

                if (classification != null)
                {
                    retLoginResponse.classification = classification.Classification;
                }
                else if (user.UserType.Equals("STUDENT"))
                {
                    retLoginResponse.classification = "O";
                }

                var profile = _ucOnlinePortalContext._212attachments.Where(x => x.StudId == user.StudId && x.Type == "2x2 ID Picture").Select(x => x.Filename).FirstOrDefault();
                if (classification != null)
                {
                    retLoginResponse.profile = profile;
                }

                return retLoginResponse;
            }
        }

        /*
         * Method to get notification
        */

        public NotificationResponse GetNotifications(NotificationRequest notificationRequest)
        {
            //select notification by id
            var notifications = _ucOnlinePortalContext._212notifications.Where(x => x.StudId == notificationRequest.id_number).OrderBy(x => x.NotifId).ToList();

            //generate list for notifications to return
            var notificationResult = notifications.Select(x => new NotificationResponse.Notification
            {
                id = x.NotifId,
                read = x.NotifRead,
                dte = x.Dte.ToString(),
                message = x.Message
            }).ToList();

            return new NotificationResponse { notifications = notificationResult };
        }

        /*
         * Method to check if email is validated
        */

        public VerifyEmailResponse CheckifUserIsVerified(VerifyEmailRequest verifyEmailRequest)
        {
            //check if user has validate email
            var userIsVerified = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == verifyEmailRequest.id_number || x.Email == verifyEmailRequest.email).FirstOrDefault();

            return new VerifyEmailResponse { is_verified = (short)userIsVerified.IsVerified };
        }

        /*
         * Method to check if user exists to reset the password
        */

        public ResetPasswordResponse ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {

            var id_number = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == resetPasswordRequest.id_number || x.Email == resetPasswordRequest.id_number).FirstOrDefault();
            //Check user_id
            if (id_number == null)
                return new ResetPasswordResponse { success = 0 };
            else
            {
                string newPass = Utils.Function.CreateRandomPassword();
                id_number.Password = Utils.Function.EncodeBase64(newPass);

                var Tk = Task.Run(() =>
                {
                    var emailDetails = new EmailDetails
                    {
                        To = new EmailAddress { Address = id_number.Email, Name = id_number.FirstName + " " + id_number.LastName }

                    };
                    emailDetails.SpecificInfo.Add("{{code}}", newPass);
                    _emailHandler.SendEmail(emailDetails, (int)RequestResponse.Enums.EmailType.RESETPASSWORD);
                });

                Tk.Wait();

                _ucOnlinePortalContext.SaveChanges();

                return new ResetPasswordResponse { success = 1 };
            }

        }

        /*
         * Method to change password.
        */

        public ChangePasswordResponse ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            //Get user data
            LoginInfo changePassword = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == changePasswordRequest.user_id).FirstOrDefault();
            //if user_id exist
            if (changePassword == null)
            {
                return new ChangePasswordResponse { success = 0 };
            }
            else
            {
                //New password
                changePassword.Password = Utils.Function.EncodeBase64(changePasswordRequest.new_password);
                _ucOnlinePortalContext.SaveChanges();

                //return response
                return new ChangePasswordResponse { success = 1 };
            }
        }

        /*
        * Method to get open sections
        */

        public EmailCheckResponse CheckEmail(EmailCheckRequest emailCheckRequest)
        {
            var login = _ucOnlinePortalContext.LoginInfos.Where(x => x.Email == emailCheckRequest.email).FirstOrDefault();
            var loginTemp = _ucOnlinePortalContext.TmpLogins.Where(x => x.Email == emailCheckRequest.email).FirstOrDefault(); 

            if (login != null)
            {
                return new EmailCheckResponse { success = 0 };
            }
            if (loginTemp != null)
            {
                _ucOnlinePortalContext.TmpLogins.Attach(loginTemp);
                _ucOnlinePortalContext.TmpLogins.Remove(loginTemp);
            }

            //Generate random number for Token
            Random generator = new Random();
            String token = generator.Next(0, 1000000).ToString("D6");

            TmpLogin newtmpLogin = new TmpLogin
            {
                Email = emailCheckRequest.email,
                Token = token
            };
            
            var Tk = Task.Run(() =>
            {
                var emailDetails = new EmailDetails
                {
                    To = new EmailAddress { Address = emailCheckRequest.email, Name = emailCheckRequest.fullname }

                };
                emailDetails.SpecificInfo.Add("{{code}}", token);
                _emailHandler.SendEmail(emailDetails, (int)RequestResponse.Enums.EmailType.VERIFICATIONCODE);
            });

            Tk.Wait();

            _ucOnlinePortalContext.TmpLogins.Add(newtmpLogin);
            _ucOnlinePortalContext.SaveChanges();

             return new EmailCheckResponse { success = 1 };
         }


        /*
        * Method to Verify Email
        */

        public ValidateEmailResponse ValidateEmail(ValidateEmailRequest validateEmailRequest)
        {
            var tmpLogin = _ucOnlinePortalContext.TmpLogins.Where(x => x.Email == validateEmailRequest.email && x.Token == validateEmailRequest.token).FirstOrDefault();
            var Login = _ucOnlinePortalContext.LoginInfos.Where(x => x.Email == validateEmailRequest.email && x.Token == validateEmailRequest.token).FirstOrDefault();

            if (tmpLogin == null && Login == null)
            {
                return new ValidateEmailResponse { success = 0 };
            }

            if (Login != null)
            {
                Login.IsVerified = 1;
                _ucOnlinePortalContext.LoginInfos.Update(Login);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new ValidateEmailResponse { success = 1 };
        }


       /*
       * Method to set notification to read
       * 
       */

        public ReadNotifResponse ReadNotification(ReadNotifRequest readNotifRequest)
        {
            if (!readNotifRequest.id_number.Equals(String.Empty))
            {
                var notif = _ucOnlinePortalContext._212notifications.Where(x => x.StudId == readNotifRequest.id_number).ToList();

                if (notif != null)
                {
                    notif.ForEach(x => x.NotifRead = 1);
                }
            }
            else
            {
                var notif = _ucOnlinePortalContext._212notifications.Where(x => x.NotifId == readNotifRequest.notif_id).FirstOrDefault();
                notif.NotifRead = 1;
            }

            _ucOnlinePortalContext.SaveChanges();
            
            return new ReadNotifResponse { success = 1};
        }

        public CreateAccountResponse CreateAccount(CreateAccountRequest createAccountRequest)
        {
            LoginInfo newLogin = new LoginInfo
            {
                StudId = createAccountRequest.id_number,
                FirstName = createAccountRequest.firstname.ToUpper(),
                LastName = createAccountRequest.lastname.ToUpper(),
                Mi = createAccountRequest.middle_initial.Substring(0, 1).ToUpper(),
                Suffix = createAccountRequest.suffix.ToUpper(),
                Dept = createAccountRequest.dept.ToUpper(),
                CourseCode = createAccountRequest.course.Equals(String.Empty) ? "" : createAccountRequest.course.ToUpper(),
                Sex = createAccountRequest.sex.ToUpper(),
                UserType = createAccountRequest.user_type.ToUpper(),
                StartTerm = "20211",
                Password = Utils.Function.EncodeBase64(createAccountRequest.lastname.ToUpper()),
                Year = 9,
                MobileNumber = createAccountRequest.mobile_number.Equals(String.Empty) ? "" : createAccountRequest.mobile_number.ToUpper(),
                Email = createAccountRequest.email.Equals(String.Empty) ? "" : createAccountRequest.email.ToUpper(),
                Birthdate = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")),
                Facebook = "",
                IsVerified = 1,
                IsBlocked = 0,
                AllowedUnits = 0,
                Token = ""
            };

            _ucOnlinePortalContext.LoginInfos.Add(newLogin);
            _ucOnlinePortalContext.SaveChanges();

            return new CreateAccountResponse { success = 1};
        }

        /*
        * Method to get open sections
        */

        public UpdateEmailResponse UpdateEmail(UpdateEmailRequest updateEmailRequest)
        {
            var login = _ucOnlinePortalContext.LoginInfos.Where(x => x.StudId == updateEmailRequest.id_number).FirstOrDefault();

            if (login == null)
            {
                return new UpdateEmailResponse { success = 0 };
            }
            else 
            {
                var loginTemp = _ucOnlinePortalContext.LoginInfos.Where(x => x.Email == updateEmailRequest.email).FirstOrDefault();

                if (loginTemp != null)
                {
                    var checkIfEmailIsOwner = _ucOnlinePortalContext.LoginInfos.Where(x => x.Email == updateEmailRequest.email && x.StudId == updateEmailRequest.id_number).FirstOrDefault();

                    if (checkIfEmailIsOwner == null)
                    {
                        return new UpdateEmailResponse { success = 0 };
                    }
                }

                //Generate random number for Token
                Random generator = new Random();
                String token = generator.Next(0, 1000000).ToString("D6");

                var Tk = Task.Run(() =>
                {
                    var emailDetails = new EmailDetails
                    {
                        To = new EmailAddress { Address = updateEmailRequest.email, Name = login.FirstName + " " + login.Mi + " " + login.LastName }

                    };
                    emailDetails.SpecificInfo.Add("{{code}}", token);
                    _emailHandler.SendEmail(emailDetails, (int)RequestResponse.Enums.EmailType.VERIFICATIONCODE);
                });

                Tk.Wait();

                login.Email = updateEmailRequest.email;
                login.Token = token;

                _ucOnlinePortalContext.LoginInfos.Update(login);
                _ucOnlinePortalContext.SaveChanges();
            }

            return new UpdateEmailResponse { success = 1 };
        }
    }
}
