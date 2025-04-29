namespace ACEAutomationProcesses.Model.CSV
{
    class AppointmentCsv
    {
        public string ProcessIndicator { get; set; }        
        public string PERNR { get; set; }
        public string NRIC { get; set; }
        public string Name { get; set; }
        public string Agency { get; set; }
        public string CurrentPRCCode { get; set; }
        public string PreviousPRCCode { get; set; }
        public string SchemeofService { get; set; }
        public string SchemeofServiceDescription { get; set; }
        public string DivisionalStatus { get; set; }
        public string DivisonText { get; set; }
        public string EffectiveDate { get; set; }

    }
}
