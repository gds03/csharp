using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts
{
    public interface IClientsService : IRepositoryService
    {
        /// <summary>
        ///     Tenta inserir um cliente na bd.
        /// </summary>
        /// <param name="clientDto"></param>
        /// <returns>true se inseriu com sucesso, false se existe um utilizador com o mesmo nome </returns>
        bool Add(ClientServiceDTO clientDto);



        /// <summary>
        ///     Tenta actualizar um cliente da bd.
        /// </summary>
        /// <param name="clientDto"></param>
        /// <returns>true se actualizou com sucesso, false se existe um utilizador com o mesmo nome</returns>
        bool Update(ClientServiceDTO clientDto);


        /// <summary>
        ///     Tenta apagar um cliente da bd
        ///     Se retornou false o cliente ficou disabled
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientName"></param>
        /// <returns>true se removeu com sucesso, false se o cliente está associado a um projecto e não pode ser removido</returns>
        bool Remove(int clientId, out string clientName);
    }
}
