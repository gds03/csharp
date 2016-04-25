using System;

namespace VirtualNote.Kernel.DTO.Query.Projects
{
    public sealed class ProjectQueryList : IQueryDTO
    {
        public int ProjectId { get; set; }
        public String ProjectName { get; set; }
        public DateTime CreatedAt { get; set; }
        public String ClientName { get; set; }
        public String ResponsableName { get; set; }
        public String Description { get; set; }
        public int AssignedWorkers { get; set; }
        public int Issues { get; set; }
        public bool Enabled { get; set; }
    }
}
