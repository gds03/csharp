using System;
using System.Collections.Generic;

namespace VirtualNote.Kernel.DTO.Query.Projects
{
    public sealed class ProjectQueryCreateUpdate
    {
        public IEnumerable<KeyIdValueString> Clients { get; set; }
        public IEnumerable<KeyIdValueString> Members { get; set; }
    }
}
