using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts.Issues
{
    public interface IIssueClientService : IRepositoryService
    {
        /// <summary>
        ///     Adiciona um issue na bd
        /// </summary>
        /// <param name="issueClientDto"></param>
        /// <exception cref="HijackedException">Se o projectID nao pertence ao cliente</exception>
        /// <exception cref="ProjectDisabledException">Se o projecto estiver desactivo</exception>
        void Add(IssueServiceClientDTO issueClientDto);



        /// <summary>
        ///     Tenta editar um issue.
        ///     Nota: Se enviar outro projectId o issue fica associado sempre ao primeiro projecto,
        ///           de forma a que é redundante enviar qualquer projectId
        /// </summary>
        /// <param name="issueClientDto"></param>
        /// <exception cref="HijackedException">Se o issue que forneço nao me pertence</exception>
        /// <exception cref="ProjectDisabledException">Se o projecto estiver desactivo</exception>
        /// <returns>
        ///  - true se actualizou com sucesso,
        ///  - false se nao actualizou, e indica que o issue já se encontra em estado terminated
        /// </returns>
        bool Update(IssueServiceClientDTO issueClientDto);


        /// <summary>
        ///     Tenta remover um issue na bd
        /// </summary>
        /// <param name="issueId"></param>
        /// <exception cref="HijackedException">Se o issue que forneço nao me pertence</exception>
        /// <exception cref="ProjectDisabledException">Se o projecto estiver desactivo</exception>
        /// <returns>true se removeu com sucesso, false, a indicar que o issue se encontra num estado diferente de waiting state e nao pode ser removido</returns>
        bool Remove(int issueId);
    }
}
