using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualNote.Kernel.DTO.Query.Issues.Details.InitialData
{
    public class IssueQueryInitialData
    {
        public IEnumerable<KeyIdValueString> ClientProjects { get; set; }
        
        public IEnumerable<PriorityEnum> Priorities { get; set; }
        public IEnumerable<TypeEnum> Types { get; set; }
        public IEnumerable<StateEnum> States { get; set; }
    }
}
