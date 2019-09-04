﻿namespace SharedServicesModule
{
    public class RecordViewModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string TaskName { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }

        public RecordViewModel(int id, string projectName, string taskName, string beginDate, string endDate)
        {
            Id = id;
            ProjectName = projectName;
            TaskName = taskName;
            BeginDate = beginDate;
            EndDate = endDate;
        }

        public RecordViewModel()
        {
        }
    }
}