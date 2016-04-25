using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualNote.Kernel.DTO.Query.Projects
{
    public sealed class ProjectQueryDetails : IQueryDTO
    {
        public int ProjectId { get; set; }
        public String ProjectName { get; set; }
        public DateTime CreatedAt { get; set; }
        public String ClientName { get; set; }
        public String ResponsableName { get; set; }
        public String Description { get; set; }
        public IEnumerable<String> AssignedWorkers { get; set; }
        public int Issues { get; set; }
        public bool Enabled { get; set; }
    }
}
