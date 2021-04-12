using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UCPortal.DTO.Request;
using UCPortal.DTO.Response;

namespace UCPortal.BusinessLogic.Enrollment
{
    public interface IEnrollmentManagement
    {
        RegistrationResponse RegisterStudent(RegistrationRequest registrationRequest);
        SaveEnrollmentResponse SaveEnrollmentData(SaveEnrollmentRequest saveEnrollRequest);
        GetDepartmentResponse GetDepartment(GetDepartmentRequest getDepartmentRequest);
        GetCoursesResponse GetCourses(GetCoursesRequest getCollegeRequest);
        ViewScheduleResponse ViewSchedules(ViewScheduleRequest viewScheduleRequest);
        GetStudyLoadResponse GetStudyLoad(GetStudyLoadRequest getRequest);
        GetStudentStatusResponse GetStudentStatus(GetStudentStatusRequest getStudentStatusRequest);
        ViewStudentPerStatusResponse ViewStudentStatus(ViewStudentPerStatusRequest viewStudentPerStatusRequest);
        ViewStudentRegistrationResponse ViewRegistration(ViewStudentRegistrationRequest viewStudentRegistrationRequest);
        SetApproveOrDisapprovedResponse SetApproveOrDisapprove(SetApproveOrDisapprovedRequest setApproveOrDisapprovedRequest);
        GetActiveSectionsResponse GetActiveSections(GetActiveSectionsRequest getActiveSectionsRequest);
        StudentReqResponse GetStudentRequest(StudentReqRequest studentReqRequest);
        ApplyReqResponse ApplyRequest(ApplyReqRequest applyReqRequest);
        ApproveReqResponse ApproveRequest(ApproveReqRequest approveReqRequest);
        GetSectionResponse GetSection(GetSectionRequest getSectionRequest);
        ChangeSchedStatusResponse ChangeSchedStatus(ChangeSchedStatusRequest changeSchedStatusRequest);
        CancelEnrollmentResponse CancelEnrollment(CancelEnrollmentRequest cancelEnrollmentRequest);
        GetStatusCountResponse GetStatusCount(GetStatusCountRequest getStatusCountRequest);
        SavePaymentResponse SavePayments(SavePaymentRequest savePaymentsRequest);
        ViewClasslistResponse ViewClasslist(ViewClasslistRequest viewClasslistRequest);
        GetEnrollmentStatusResponse GetEnrollmentStatus(GetEnrollmentStatusRequest getEnrollmentStatusRequest);
        UpdateStudentInfoResponse UpdateStudentInfo(UpdateStudentInfoRequest updateStudentInfoRequest);
        ViewStudentEvaluationResponse ViewEvaluation(ViewStudentEvaluationRequest viewStudentEvaluationRequest);
        ViewOldStudentInfoResponse ViewOldStudentInfo(ViewOldStudentInfoRequest viewOldStudentInfoRequest);
        SelectSubjectInfoResponse SelectSubject(SelectSubjectInfoRequest selectSubjectInfoRequest);
        UpdateSubjectResponse UpdateSubject(UpdateSubjectRequest updateSubjectRequest);
        RemoveDuplicateOtspResponse RemoveDuplicateOstsp();
        ViewClasslistPerSectionResponse ViewClasslistPerSection(ViewClasslistPerSectionRequest viewClasslistPerSectionRequest);
        UpdateStudentStatusResponse UpdateStudentStatus(UpdateStudentStatusRequest updateStudentStatusRequest);
        ManualEnrollmentResponse ManualEnrollment(ManualEnrollmentRequest manualEnrollmentRequest);
        RequestPromissoryResponse RequestPromissory(RequestPromissoryRequest requestPromissoryRequest);
        GetPromissoryListResponse GetPromissoryList(GetPromissoryListRequest getPromissoryListRequest);
        CorrectTotalUnitsResponse CorrectTotalUnits();
        AddNotificationResponse AddNotification(AddNotificationRequest addNotificationRequest);
        UpdateInfoResponse GetInfoUpdate(UpdateInfoRequest updateInfoRequest);
        UpdateInforResponse UpdateInfor(UpdateInforRequest updateInfor);
        SetClosedSubjectResponse SetClosed();
        GetTeachersListResponse GetTeachersList(GetTeachersListRequest getTeachersListRequest);
        SaveTeachersLoadResponse SaveTeachersLoad(SaveTeachersLoadRequest saveTeachersLoadRequest);
        GetTeachersLoadResponse GetTeachersLoad(GetTeachersLoadRequest getTeachersLoadRequest);
        SaveAdjustmentResponse SaveAdjustment(SaveAdjustmentRequest saveAdjustmentRequest);
        GetAdjustmentListResponse GetAdjustmentlist(GetAdjustmentListRequest getAdjustmentListRequest);
        GetCurriculumResponse GetCurriculum(GetCurriculumRequest getCurriculumRequest);
        StudentSubjectResponse RequestSubject(StudentSubjectRequest requestSubject);
        GetSubjectReqResponse GetRequestSubject(GetSubjectReqRequest getRequest);
        GetStudentReqResponse GetStudentSubjectRequest(GetStudentReqRequest getRequest);
        AddStudentReqResponse AddSubjectRequest(AddStudentReqRequest getRequest);
        CancelSubjectReqResponse CancelSubjectRequest(CancelSubjectReqRequest getRequest);
        GetAllCurriculumResponse GetAllCurriculum();
        GetCourseInfoResponse GetCourseInfo(GetCourseInfoRequest getRequest);
        AddCurriculumResponse AddCurriculum(AddCurriculumRequest getRequest);
        CloseCurriculumReponse CloseCurriculum(CloseCurriculumRequest getRequest);
        GetSubjectInfoResponse GetSubjectInfo(GetSubjectInfoRequest getRequest);
        RemovePrerequisiteResponse RemovePrerequisite(RemovePrerequisiteRequest getRequest);
        SavePrerequisiteResponse SavePrerequisite(SavePrerequisiteRequest savePrerequisiteRequest);
    }
}
