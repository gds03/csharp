using VirtualNote.Database;
using VirtualNote.Kernel.Configurations.StructureMap;

namespace VirtualNote.Kernel.Managers
{
    /// <summary>
    ///     Esta classe representa uma transação.
    ///     Quando usa um repositorio em memoria nao faz nada.
    ///     Quando usa um repositorio em base de dados representa uma transação por pedido http
    /// </summary>
    /// 
    public static class TransactionManager
    {
        private static IRepository Db
        {
            get { return ObjectsManager.GetInstance<IRepository>(); }
        }

        public static void Commit()
        {
            Db.SaveToDisk();
        }

        public static void Dispose()
        {
            Db.Dispose();
        }
    }
}
