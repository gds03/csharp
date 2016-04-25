using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts
{
    public interface IProjectsService : IRepositoryService
    {
        /// <summary>
        ///     Tenta inserir um projecto na bd.
        /// </summary>
        /// <param name="projectDto"></param>
        /// <returns>true se inseriu com sucesso, false se existe um projecto com o mesmo nome</returns>
        bool Add(ProjectServiceDTO projectDto);



        /// <summary>
        ///     Tenta actualizar um projecto na bd.
        ///     Caso o responsavel seja alterado para um membro que se encontre a trabalhar no projecto
        ///     esse membro é removido da lista dos trabalhadores ficando somente responsavel.
        ///     Os restantes trabalhadores ficam inalteraveis caso existam.
        /// </summary>
        /// <param name="projectDto"></param>
        /// <returns>true se inseriu com sucesso, false se existe um projecto com o mesmo nome</returns>
        bool Update(ProjectServiceDTO projectDto);


        /// <summary>
        ///     Tenta apagar um projecto na bd.
        ///     Se retornou false o projecto ficou disabled
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <returns>true se removeu com sucesso, false se o projecto contem issues e não pode ser removido</returns>
        bool Remove(int projectId, out string projectName);


        /// <summary>
        ///     Associa trabalhadores a um projecto
        /// </summary>
        /// <param name="infoDto"></param>
        /// <param name="projectName"></param>
        void Assign(ProjectServiceAssignWorkersDTO infoDto, out string projectName);
    }
}
