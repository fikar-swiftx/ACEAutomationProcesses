using System;

namespace ACEAutomationProcesses.Model
{
    class ChangeMdScenario
    {
        public string ProcessIndicator { get; set; }
        public long Seq { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Agency { get; set; }
        public string CurrentPrcCode { get; set; }
        public string SchemeOfService { get; set; }
        public string SchemeOfServiceDescription { get; set; }
        public string DivisionalStatus { get; set; }
        public string DivisionText { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public long FormId { get; set; }
        public string ServingAgency { get; set; }
        public string ProcessStatus { get; set; }

    }
}
