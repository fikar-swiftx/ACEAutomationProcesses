using System.Collections.Generic;

namespace ACEAutomationProcesses.OTCS
{
    public class GenericModel
    {
        public string Ticket { get; set; }
        public string Id {get; set;}
        public string Data { get; set; }
    }

    public class RootObject
    {
        public List<Datum> Data { get; set; }
    }

    public class Datum
    {
        public int Id { get; set; }
        public string Name {get; set;}
        public List<Items> Items { get; set; }
    }

    public class Items
    {
        public bool Container { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
