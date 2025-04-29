namespace ACEAutomationProcesses.Database
{
    class MemberModel
    {
        enum MemberType
        {
            User = 0,
            Group = 1
        };

        public string Name { get; set; }
        public long Id { get; set; }
        public string Type { get; set; }
    }
}
