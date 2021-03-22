using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace UCPortal.DatabaseEntities.Models
{
    public partial class UCOnlinePortalContext : DbContext
    {
        public UCOnlinePortalContext()
        {
        }

        public UCOnlinePortalContext(DbContextOptions<UCOnlinePortalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<CourseList> CourseLists { get; set; }
        public virtual DbSet<Curriculum> Curricula { get; set; }
        public virtual DbSet<GradeEvaluation> GradeEvaluations { get; set; }
        public virtual DbSet<LoginInfo> LoginInfos { get; set; }
        public virtual DbSet<Prerequisite> Prerequisites { get; set; }
        public virtual DbSet<RequestSchedule> RequestSchedules { get; set; }
        public virtual DbSet<StudentRequest> StudentRequests { get; set; }
        public virtual DbSet<SubjectInfo> SubjectInfos { get; set; }
        public virtual DbSet<TmpLogin> TmpLogins { get; set; }
        public virtual DbSet<_212attachment> _212attachments { get; set; }
        public virtual DbSet<_212contactAddress> _212contactAddresses { get; set; }
        public virtual DbSet<_212familyInfo> _212familyInfos { get; set; }
        public virtual DbSet<_212grade> _212grades { get; set; }
        public virtual DbSet<_212notification> _212notifications { get; set; }
        public virtual DbSet<_212oenrp> _212oenrps { get; set; }
        public virtual DbSet<_212ostsp> _212ostsps { get; set; }
        public virtual DbSet<_212promissory> _212promissories { get; set; }
        public virtual DbSet<_212schedule> _212schedules { get; set; }
        public virtual DbSet<_212schoolInfo> _212schoolInfos { get; set; }
        public virtual DbSet<_212studentInfo> _212studentInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
<<<<<<< HEAD
            
                optionsBuilder.UseSqlServer("Server=ADMIN-PC\\SQLEXPRESS;Database=UCOnlinePortal;Trusted_Connection=True;");
=======
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=UCOnlinePortal;Trusted_Connection=True;");
>>>>>>> 49a508f9b7e58cdc735872f7260f5b25edfe929f
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>(entity =>
            {
                entity.ToTable("config");

                entity.Property(e => e.ConfigId).HasColumnName("config_id");

                entity.Property(e => e.CampusId).HasColumnName("campus_id");

                entity.Property(e => e.IdYear).HasColumnName("id_year");

                entity.Property(e => e.Sequence).HasColumnName("sequence");
            });

            modelBuilder.Entity<CourseList>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("course_list");

                entity.Property(e => e.AdjustmentEnd)
                    .HasColumnType("datetime")
                    .HasColumnName("adjustment_end");

                entity.Property(e => e.AdjustmentStart)
                    .HasColumnType("datetime")
                    .HasColumnName("adjustment_start");

                entity.Property(e => e.CourseAbbr)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_abbr");

                entity.Property(e => e.CourseActive).HasColumnName("course_active");

                entity.Property(e => e.CourseCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_code");

                entity.Property(e => e.CourseDepartment)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("course_department");

                entity.Property(e => e.CourseDepartmentAbbr)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_department_abbr");

                entity.Property(e => e.CourseDescription)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("course_description");

                entity.Property(e => e.CourseId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("course_id");

                entity.Property(e => e.CourseYearLimit).HasColumnName("course_year_limit");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("department");

                entity.Property(e => e.EnrollmentOpen).HasColumnName("enrollment_open");
            });

            modelBuilder.Entity<Curriculum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("curriculum");

                entity.Property(e => e.CurrId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("curr_id");

                entity.Property(e => e.IsDeployed).HasColumnName("isDeployed");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<GradeEvaluation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("grade_evaluation");

                entity.Property(e => e.FinalGrade)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("final_grade");

                entity.Property(e => e.GradeEvalId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("grade_eval_id");

                entity.Property(e => e.IntCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("int_code");

                entity.Property(e => e.MidtermGrade)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("midterm_grade");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("term");
            });

            modelBuilder.Entity<LoginInfo>(entity =>
            {
                entity.HasKey(e => e.CinfoId);

                entity.ToTable("login_info");

                entity.Property(e => e.CinfoId).HasColumnName("cinfo_id");

                entity.Property(e => e.AllowedUnits).HasColumnName("allowed_units");

                entity.Property(e => e.Birthdate)
                    .HasColumnType("datetime")
                    .HasColumnName("birthdate");

                entity.Property(e => e.CourseCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_code");

                entity.Property(e => e.Dept)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("dept");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Facebook)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("facebook");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsBlocked).HasColumnName("is_blocked");

                entity.Property(e => e.IsVerified).HasColumnName("is_verified");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.Mi)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("mi");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("mobile_number");

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Sex)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("sex");

                entity.Property(e => e.StartTerm)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("start_term");

                entity.Property(e => e.StudId)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.Token)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.Property(e => e.UserType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("user_type");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<Prerequisite>(entity =>
            {
                entity.ToTable("prerequisite");

                entity.Property(e => e.PrerequisiteId).HasColumnName("prerequisite_id");

                entity.Property(e => e.InternalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("internal_code");

                entity.Property(e => e.Prerequisites)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("prerequisites");
            });

            modelBuilder.Entity<RequestSchedule>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("request_schedule");

                entity.Property(e => e.RequestId).HasColumnName("request_id");

                entity.Property(e => e.Days)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("days");

                entity.Property(e => e.InternalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("internal_code");

                entity.Property(e => e.MTimeEnd)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("m_time_end");

                entity.Property(e => e.MTimeStart)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("m_time_start");

                entity.Property(e => e.Mdn)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("mdn");

                entity.Property(e => e.Rtype).HasColumnName("rtype");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.SplitCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("split_code");

                entity.Property(e => e.SplitType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("split_type");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("subject_name");

                entity.Property(e => e.TimeEnd)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("time_end");

                entity.Property(e => e.TimeStart)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("time_start");
            });

            modelBuilder.Entity<StudentRequest>(entity =>
            {
                entity.HasKey(e => e.StudRequestId);

                entity.ToTable("student_request");

                entity.Property(e => e.StudRequestId).HasColumnName("stud_request_id");

                entity.Property(e => e.InternalCode)
                    .HasMaxLength(10)
                    .HasColumnName("internal_code")
                    .IsFixedLength(true);

                entity.Property(e => e.StudId)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");
            });

            modelBuilder.Entity<SubjectInfo>(entity =>
            {
                entity.HasKey(e => e.SubInfoId);

                entity.ToTable("subject_info");

                entity.Property(e => e.SubInfoId).HasColumnName("sub_info_id");

                entity.Property(e => e.CourseCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_code");

                entity.Property(e => e.CurriculumYear).HasColumnName("curriculum_year");

                entity.Property(e => e.Descr1)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("descr_1");

                entity.Property(e => e.Descr2)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("descr_2");

                entity.Property(e => e.InternalCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("internal_code");

                entity.Property(e => e.Semester).HasColumnName("semester");

                entity.Property(e => e.SplitCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("split_code");

                entity.Property(e => e.SplitType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("split_type");

                entity.Property(e => e.SubjectName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("subject_name");

                entity.Property(e => e.SubjectType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("subject_type");

                entity.Property(e => e.Units).HasColumnName("units");

                entity.Property(e => e.YearLevel).HasColumnName("year_level");
            });

            modelBuilder.Entity<TmpLogin>(entity =>
            {
                entity.ToTable("tmp_login");

                entity.Property(e => e.TmpLoginId).HasColumnName("tmp_login_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("token");
            });

            modelBuilder.Entity<_212attachment>(entity =>
            {
                entity.HasKey(e => e.AttachmentId);

                entity.ToTable("212attachments");

                entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("filename");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<_212contactAddress>(entity =>
            {
                entity.HasKey(e => e.AddConId);

                entity.ToTable("212contact_address");

                entity.Property(e => e.AddConId).HasColumnName("add_con_id");

                entity.Property(e => e.CBarangay)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("c_barangay");

                entity.Property(e => e.CCity)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("c_city");

                entity.Property(e => e.CProvince)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("c_province");

                entity.Property(e => e.CStreet)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("c_street");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Facebook)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("facebook");

                entity.Property(e => e.Landline)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("landline");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("mobile");

                entity.Property(e => e.PBarangay)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_barangay");

                entity.Property(e => e.PCity)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_city");

                entity.Property(e => e.PCountry)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_country");

                entity.Property(e => e.PProvince)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("p_province");

                entity.Property(e => e.PStreet)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("p_street");

                entity.Property(e => e.PZip)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("p_zip");

                entity.Property(e => e.StudInfoId).HasColumnName("stud_info_id");
            });

            modelBuilder.Entity<_212familyInfo>(entity =>
            {
                entity.HasKey(e => e.FamilyInfoId);

                entity.ToTable("212family_info");

                entity.Property(e => e.FamilyInfoId).HasColumnName("family_info_id");

                entity.Property(e => e.FatherContact)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("father_contact");

                entity.Property(e => e.FatherName)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("father_name");

                entity.Property(e => e.FatherOccupation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("father_occupation");

                entity.Property(e => e.GuardianContact)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("guardian_contact");

                entity.Property(e => e.GuardianName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("guardian_name");

                entity.Property(e => e.GuardianOccupation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("guardian_occupation");

                entity.Property(e => e.MotherContact)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("mother_contact");

                entity.Property(e => e.MotherName)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("mother_name");

                entity.Property(e => e.MotherOccupation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mother_occupation");

                entity.Property(e => e.StudInfoId).HasColumnName("stud_info_id");
            });

            modelBuilder.Entity<_212grade>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("212grades");

                entity.Property(e => e.Dte)
                    .HasColumnType("datetime")
                    .HasColumnName("dte");

                entity.Property(e => e.EdpCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("edp_code");

                entity.Property(e => e.Final)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("final");

                entity.Property(e => e.GradesId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("grades_id");

                entity.Property(e => e.Midterm)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("midterm");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");
            });

            modelBuilder.Entity<_212notification>(entity =>
            {
                entity.HasKey(e => e.NotifId);

                entity.ToTable("212notification");

                entity.Property(e => e.NotifId).HasColumnName("notif_id");

                entity.Property(e => e.Dte)
                    .HasColumnType("datetime")
                    .HasColumnName("dte");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("message");

                entity.Property(e => e.NotifRead).HasColumnName("notif_read");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");
            });

            modelBuilder.Entity<_212oenrp>(entity =>
            {
                entity.HasKey(e => e.OenrpId);

                entity.ToTable("212oenrp");

                entity.Property(e => e.OenrpId).HasColumnName("oenrp_id");

                entity.Property(e => e.AdjustmentBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("adjustment_by");

                entity.Property(e => e.AdjustmentCount).HasColumnName("adjustment_count");

                entity.Property(e => e.AdjustmentOn)
                    .HasColumnType("datetime")
                    .HasColumnName("adjustment_on");

                entity.Property(e => e.ApprovedAcctg)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("approved_acctg");

                entity.Property(e => e.ApprovedAcctgOn)
                    .HasColumnType("datetime")
                    .HasColumnName("approved_acctg_on");

                entity.Property(e => e.ApprovedCashier)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("approved_cashier");

                entity.Property(e => e.ApprovedCashierOn)
                    .HasColumnType("datetime")
                    .HasColumnName("approved_cashier_on");

                entity.Property(e => e.ApprovedDean)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("approved_dean");

                entity.Property(e => e.ApprovedDeanOn)
                    .HasColumnType("datetime")
                    .HasColumnName("approved_dean_on");

                entity.Property(e => e.ApprovedRegDean)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("approved_reg_dean");

                entity.Property(e => e.ApprovedRegDeanOn)
                    .HasColumnType("datetime")
                    .HasColumnName("approved_reg_dean_on");

                entity.Property(e => e.ApprovedRegRegistrar)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("approved_reg_registrar");

                entity.Property(e => e.ApprovedRegRegistrarOn)
                    .HasColumnType("datetime")
                    .HasColumnName("approved_reg_registrar_on");

                entity.Property(e => e.Classification)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("classification");

                entity.Property(e => e.CourseCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_code");

                entity.Property(e => e.Dept)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("dept");

                entity.Property(e => e.DisapprovedDean)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("disapproved_dean");

                entity.Property(e => e.DisapprovedDeanOn)
                    .HasColumnType("datetime")
                    .HasColumnName("disapproved_dean_on");

                entity.Property(e => e.DisapprovedRegDean)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("disapproved_reg_dean");

                entity.Property(e => e.DisapprovedRegDeanOn)
                    .HasColumnType("datetime")
                    .HasColumnName("disapproved_reg_dean_on");

                entity.Property(e => e.DisapprovedRegRegistrar)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("disapproved_reg_registrar");

                entity.Property(e => e.DisapprovedRegRegistrarOn)
                    .HasColumnType("datetime")
                    .HasColumnName("disapproved_reg_registrar_on");

                entity.Property(e => e.EnrollmentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("enrollment_date");

                entity.Property(e => e.Mdn)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("mdn");

                entity.Property(e => e.NeededPayment).HasColumnName("needed_payment");

                entity.Property(e => e.PromiPay)
                    .HasColumnName("promi_pay")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RegisteredOn)
                    .HasColumnType("datetime")
                    .HasColumnName("registered_on");

                entity.Property(e => e.RequestDeblock).HasColumnName("request_deblock");

                entity.Property(e => e.RequestOverload).HasColumnName("request_overload");

                entity.Property(e => e.RequestPromissory)
                    .HasColumnName("request_promissory")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Section)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("section");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");

                entity.Property(e => e.SubmittedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("submitted_on");

                entity.Property(e => e.Units).HasColumnName("units");

                entity.Property(e => e.YearLevel).HasColumnName("year_level");
            });

            modelBuilder.Entity<_212ostsp>(entity =>
            {
                entity.HasKey(e => e.StsId);

                entity.ToTable("212ostsp");

                entity.Property(e => e.StsId).HasColumnName("sts_id");

                entity.Property(e => e.AdjustedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("adjusted_on");

                entity.Property(e => e.EdpCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("edp_code");

                entity.Property(e => e.Remarks)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("remarks");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");
            });

            modelBuilder.Entity<_212promissory>(entity =>
            {
                entity.HasKey(e => e.PromiId);

                entity.ToTable("212promissory");

                entity.Property(e => e.PromiId).HasColumnName("promi_id");

                entity.Property(e => e.PromiDate)
                    .HasColumnType("datetime")
                    .HasColumnName("promi_date");

                entity.Property(e => e.PromiMessage)
                    .IsRequired()
                    .HasMaxLength(1500)
                    .IsUnicode(false)
                    .HasColumnName("promi_message");

                entity.Property(e => e.StudId)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");
            });

            modelBuilder.Entity<_212schedule>(entity =>
            {
                entity.HasKey(e => e.ScheduleId);

                entity.ToTable("212schedules");

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");

                entity.Property(e => e.CourseCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_code");

                entity.Property(e => e.Days)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("days");

                entity.Property(e => e.Deployed).HasColumnName("deployed");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.EdpCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("edp_code");

                entity.Property(e => e.Instructor)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("instructor");

                entity.Property(e => e.Instructor2)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("instructor_2");

                entity.Property(e => e.InternalCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("internal_code");

                entity.Property(e => e.IsGened).HasColumnName("is_gened");

                entity.Property(e => e.MDays)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("m_days");

                entity.Property(e => e.MTimeEnd)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("m_time_end");

                entity.Property(e => e.MTimeStart)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("m_time_start");

                entity.Property(e => e.MaxSize).HasColumnName("max_size");

                entity.Property(e => e.Mdn)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("mdn");

                entity.Property(e => e.OfficialEnrolled).HasColumnName("official_enrolled");

                entity.Property(e => e.PendingEnrolled).HasColumnName("pending_enrolled");

                entity.Property(e => e.Room)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("room");

                entity.Property(e => e.Section)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("section");

                entity.Property(e => e.Size).HasColumnName("size");

                entity.Property(e => e.SplitCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("split_code");

                entity.Property(e => e.SplitType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("split_type");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.SubType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("sub_type");

                entity.Property(e => e.TimeEnd)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("time_end");

                entity.Property(e => e.TimeStart)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("time_start");

                entity.Property(e => e.Units).HasColumnName("units");
            });

            modelBuilder.Entity<_212schoolInfo>(entity =>
            {
                entity.HasKey(e => e.SchoolInfoId);

                entity.ToTable("212school_info");

                entity.Property(e => e.SchoolInfoId).HasColumnName("school_info_id");

                entity.Property(e => e.ColCourse)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("col_course");

                entity.Property(e => e.ColLastYear).HasColumnName("col_last_year");

                entity.Property(e => e.ColName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("col_name");

                entity.Property(e => e.ColYear)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("col_year");

                entity.Property(e => e.ElemEscSchoolId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("elem_esc_school_id");

                entity.Property(e => e.ElemEscStudentId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("elem_esc_student_id");

                entity.Property(e => e.ElemLastYear).HasColumnName("elem_last_year");

                entity.Property(e => e.ElemLrnNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("elem_lrn_no");

                entity.Property(e => e.ElemName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("elem_name");

                entity.Property(e => e.ElemType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("elem_type");

                entity.Property(e => e.ElemYear)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("elem_year");

                entity.Property(e => e.SecEscSchoolId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("sec_esc_school_id");

                entity.Property(e => e.SecEscStudentId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("sec_esc_student_id");

                entity.Property(e => e.SecLastStrand)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("sec_last_strand");

                entity.Property(e => e.SecLastYear).HasColumnName("sec_last_year");

                entity.Property(e => e.SecLrnNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("sec_lrn_no");

                entity.Property(e => e.SecName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("sec_name");

                entity.Property(e => e.SecType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("sec_type");

                entity.Property(e => e.SecYear)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("sec_year");

                entity.Property(e => e.StudInfoId).HasColumnName("stud_info_id");
            });

            modelBuilder.Entity<_212studentInfo>(entity =>
            {
                entity.HasKey(e => e.StudInfoId);

                entity.ToTable("212student_info");

                entity.Property(e => e.StudInfoId).HasColumnName("stud_info_id");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("datetime")
                    .HasColumnName("birth_date");

                entity.Property(e => e.BirthPlace)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("birth_place");

                entity.Property(e => e.Classification)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("classification");

                entity.Property(e => e.CourseCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("course_code");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_created");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("date_updated");

                entity.Property(e => e.Dept)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("dept");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("first_name");

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("gender");

                entity.Property(e => e.IsVerified).HasColumnName("is_verified");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("last_name");

                entity.Property(e => e.Mdn)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("mdn");

                entity.Property(e => e.MiddleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("middle_name");

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nationality");

                entity.Property(e => e.Religion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("religion");

                entity.Property(e => e.StartTerm).HasColumnName("start_term");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.StudId)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("stud_id");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("suffix");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("token");

                entity.Property(e => e.YearLevel).HasColumnName("year_level");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
