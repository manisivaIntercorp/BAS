using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public static class EmailTemplateCode
    {
        public const string USER_ACCOUNT_CREATED = "T001";
        public const string FORGOT_PASSWORD = "T002";
        public const string CHANGE_PASSWORD = "T003";
        public const string RESET_PASSWORD = "T004";
        public const string SEND_OTP = "T005";
        public const string BAS_VMS_INVITE = "T200";
        public const string TENANT_INVITE = "T201";
        public const string TENANT_PRE_APPROVE = "T202";
        public const string TENANT_APPROVE = "T203";
        public const string TENANT_REJECT = "T204";
        public const string TENANT_ACKNOWLEDGMENT = "T205";
        public const string TENANT_CHANGE_MEETING = "T206";
        public const string TENANT_CANCEL_MEETING = "T208";
        public const string EMAIL_PAYSLIP = "T207";
        public const string EMAIL_IRAS = "T209";
        public const string EMAIL_CISCO = "T210";
        public const string EMAIL_CLAIM = "T212";
        public const string EMAIL_LEAVE_SUBMIT = "T213";
        public const string EMAIL_LEAVE_APPROVE = "T214";
        public const string EMAIL_LEAVE_APPROVE_FINAL = "T219";
        public const string EMAIL_eCLAIM_APPROVE = "T216";
        public const string EMAIL_eCLAIM_REJECT = "T217";
        public const string EMAIL_eCLAIM_SUBMIT = "T218";
        public const string EMAIL_LEAVE_REJECT = "T215";
        public const string COURSE_DELETE = "T005";
        public const string COURSE_MODIFIED = "T006";
        public const string APPRAISAL_INVITING = "T401";
        public const string MANAGER_APPROVAL = "T402";

        public const string EMAIL_USER_CANCEL_LEAVE = "T220";
        public const string EMAIL_CANCEL_LEAVE_APPROVE = "T221";
        public const string EMAIL_CANCEL_LEAVE_APPROVE_FINAL = "T222";
        public const string EMAIL_CANCEL_LEAVE_REJECT = "T223";
        public const string EMAIL_INVITE_SELF_SERVICE = "T501";
        public const string EMAIL_SELF_SERVICE_CREDENTIAL = "T502";
        public const string EMAIL_TIME_BY_TRIP_AMMENDMENT_INTERFACE = "T213";
        public const string SELF_ORG_LICENSE_ACTIVATED = "T601";
        public const string SELF_ORG_LICENSE_ACTIVATE_FAILED = "T602";
        public const string EMAIL_SEND_REQUESTEMPLOYEEBIDDING = "T701";// Roster Employee Bidding Send Request
        public const string EMAIL_ROSTER_PLAN_APPROVE = "T702";

        public const string EMAIL_ROSTER_EMPLOYEE_BIDDING_APPROVE = "T704";// Roster Employee Bidding Approve
        public const string EMAIL_ROSTER_EMPLOYEE_BIDDING_REJECT = "T705"; // Roster Employee Bidding Reject
        public const string EMAIL_SEND_REQUESTEMPLOYEERESPONSEBIDDING = "T706";//Employee Response Bidding Request
        public const string EMAIL_ROSTER_SHIFT_CHANGE_REQUEST = "T703"; // Shift Change Request to Approver
        public const string EMAIL_ROSTER_SHIFT_SWAPPING_REQUEST = "T707"; // Shift Swapping Request to Employee
        public const string EMAIL_ROSTER_SHIFT_CHANGE_APPROVE = "T708"; // Shift Change Approved / Rejected
        public const string EMAIL_ROSTER_SHIFT_SWAPPING_APPROVE = "T709"; // Shift Swapping Approved / Rejected
        public const string EMAIL_ROSTER_SHIFT_SWAPPING_ACCEPT = "T710"; // Shift Swapping Accepted / Rejected
        public const string EMAIL_APPLY_PERMIT_SUBMITTED = "T801"; // PTW Submiteed mail to Approvers
        public const string EMAIL_APPLY_PERMIT_REJECTED = "T802"; // PTW Rejected mail to Users
        public const string EMAIL_APPLY_PERMIT_COMPLETED = "T803"; // PTW Completed mail to Users
        public const string EMAIL_APPLY_INCIDENT_SUBMITTED = "T804"; // Incident Submiteed mail to Approvers
        public const string EMAIL_APPLY_INCIDENT_REJECTED = "T805"; // PTW Rejected mail to Users
        public const string EMAIL_APPLY_INCIDENT_COMPLETED = "T806"; // PTW Completed mail to Users
    }
}
