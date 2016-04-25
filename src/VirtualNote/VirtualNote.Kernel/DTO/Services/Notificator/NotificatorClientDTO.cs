namespace VirtualNote.Kernel.DTO.Services.Notificator
{
    public sealed class NotificatorClientDTO : IServiceDTO
    {
        // Informacao do cliente
        public string ClientName { get; private set; }

        // Informacao do projecto
        public int ProjectId { get; private set; }
        public string ProjectName { get; private set; }

        // Informacao do issue
        public PriorityEnum Priority { get; private set; }
        public TypeEnum Type { get; private set; }
        public string ShortDescription { get; private set; }
        public string DetailedDescription { get; private set; }


        public NotificatorClientDTO(string clientName, int projectId, string projectName, PriorityEnum priority,
            TypeEnum type, string shortDescription, string detailedDescription){
            ClientName = clientName;

            ProjectId = projectId;
            ProjectName = projectName;

            Priority = priority;
            Type = type;
            ShortDescription = shortDescription;
            DetailedDescription = detailedDescription;
        }
    }
}
