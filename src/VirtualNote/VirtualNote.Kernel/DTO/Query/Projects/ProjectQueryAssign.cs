using System.Collections.Generic;

namespace VirtualNote.Kernel.DTO.Query.Projects
{
    public sealed class ProjectQueryAssign : IQueryDTO
    {
        public string ProjectName { get; set; }
        public string ResponsableName { get; set; }

        public IEnumerable<KeyIdValueString> AvailableWorkers { get; set; }
        public IEnumerable<int> WorkersIdsWorking { get; set; }
    }
}
