using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtualNote.Kernel.DTO.Query.Projects;
using VirtualNote.Kernel.Validation.DataAnnotations;

namespace VirtualNote.Kernel.DTO
{
    public sealed class ProjectServiceDTO : IServiceDTO 
    {
        public ProjectQueryCreateUpdate InitialData { get; set; }   // Encapsulate all clients and all members in a kvp

        public int ProjectID { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(30, ErrorMessage = "Name must have between 3 and 30 characters", MinimumLength = 3)]
        // [RegularExpression(@"^[\S]*$", ErrorMessage = "Name cannot contain spaces")]
        public String Name { get; set; }

        public bool Enabled { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(100, ErrorMessage = "Description must have between 5 and 100 characters", MinimumLength = 5)]
        public String Description { get; set; }

        public int ResponsableId { get; set; }
        public int ClientId { get; set; }
    }

    public sealed class ProjectServiceAssignWorkersDTO : IServiceDTO
    {
        public int ProjectId { get; set; }
        public IEnumerable<int> workersIds { get; set; }
    }
}
