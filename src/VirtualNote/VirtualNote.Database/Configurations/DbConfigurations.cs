using VirtualNote.Database.Configurations.Database;

namespace VirtualNote.Database.Configurations
{
    public static class DbConfigurations
    {
        public static bool UsingTestData { get; private set; }

        public static void Initialize(bool loadTestData) {
            UsingTestData = loadTestData;
            System.Data.Entity
                  .Database
                  .SetInitializer(new VirtualNoteDbContextInitializerStrategy(loadTestData));
        }
    }
}
