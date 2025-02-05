namespace DataAccessLayer.Services
{

    public class EmailTemplate
    {


        public long? RefID1
        {
            get;
            set;
        }

        public int? RefID2
        {
            get;
            set;
        }

        public string? Template
        {
            get;
            set;
        }

        public string? EmployeeID
        {
            get;
            set;
        }


        public string? Subject
        {
            get;
            set;
        }


        public string? DisplayName
        {
            get;
            set;
        }


        public string? UserName
        {
            get;
            set;
        }


        public string? Password
        {
            get;
            set;
        }


        public string? URL
        {
            get;
            set;
        }


        public string? ExpiredDateTime
        {
            get;
            set;
        }


        public string? Active
        {
            get;
            set;
        }


        public string? TemplateCode
        {
            get;
            set;
        }

        public int? UserID
        {
            get;
            set;
        }


        public long ContactNo
        {
            get; set;
        }

        public string Month
        {
            get;
            set;
        }

        public string? Year
        {
            get;
            set;
        }


        public string? Status
        {
            get;
            set;
        }
        public string? EmailID
        {
            get;
            set;
        }
    }
    public class GetEmailTemplate
    {
        public string? EmailID { get; set; }
        public string? Template { get; set; }
        public string? Subject { get; set; }
        public string? MobileContent { get; set; }
    }
}