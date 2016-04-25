using System.Data.Entity;

namespace VirtualNote.Database.Configurations.Database
{
    internal sealed class VirtualNoteDbContextInitializerStrategy : DropCreateDatabaseIfModelChanges<VirtualNoteDbContext>
    {
        readonly bool m_testData; 


        public VirtualNoteDbContextInitializerStrategy(bool loadTestData)
        {
            m_testData = loadTestData;
        }

        protected override void Seed(VirtualNoteDbContext context)
        {
            if (m_testData) 
            {
                DiskRepository repository = context.Repository;
                repository.AddDefaultData(m_testData);

                // Enviar para a base de dados
                repository.SaveToDisk();
            }
        }
    }
}
