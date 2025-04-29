using System;

namespace ACEAutomationProcesses.Model
{
    class ChangeIdScenario
    {
        public String ProcessIndicator { get; set; }
        public long Seq { get; set; }
        public String NewId { get; set; }
        public String OldId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public long FormId { get; set; }
        public String ProcessStatus { get; set; }
        public String Agency { get; set; }

    }
}
