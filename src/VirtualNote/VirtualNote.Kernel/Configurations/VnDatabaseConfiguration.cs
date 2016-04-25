using VirtualNote.Database.Configurations;

namespace VirtualNote.Kernel.Configurations
{
    public static class VnDatabaseConfiguration
    {
        public const bool LoadTestData = false;            // Especificar aqui se pretendemos carregar dados iniciais

        public static void Initialize()
        {
            DbConfigurations.Initialize(LoadTestData);
        }
    }
}
