using System.Collections.Generic;

namespace VirtualNote.Kernel.Contracts
{
    public interface IEmailConfigMngr
    {
        /// <summary>
        ///     Devolve as configurações para o userId
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="userId"></param>
        /// <param name="hasElement">
        ///     Serve para indicar se a lista está vazia porque nao encontrou o elemento ou se
        ///     esta vazia porque nao tem valores.
        /// </param>
        /// <returns>Lista vazia se o elemento nao foi encontrado ou se nao tem valores.</returns>
        IEnumerable<EmailConfig> Find(UserType userType, int userId, out bool hasElement);

        /// <summary>
        ///     Tenta inserir valores de configuração para um User
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="userId"></param>
        /// <param name="values"></param>
        /// <returns>true se inseriu com sucesso, false se já existe o userId </returns>
        bool Add(UserType userType, int userId, IEnumerable<EmailConfig> values);

        /// <summary>
        ///     Tenta actualizar valores de configuração para um User
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="userId"></param>
        /// <param name="values"></param>
        /// <returns>true se actualizou com sucesso, false se não existe o userId </returns>
        bool Update(UserType userType, int userId, IEnumerable<EmailConfig> values);

        /// <summary>
        ///     Tenta apagar o userId e valores de configuração
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="userId"></param>
        /// <returns>true se removeu com sucesso, false se não existe o userId</returns>
        bool Delete(UserType userType, int userId);
    }


    public enum EmailConfig
    {
        // Clients values
        Client_RequestAccepted = 1,                     // Algum membro aceitou o pedido
        Client_RequestWaitingStateAgain = 2,            // Um issue em resolução está novamente em wait state
        Client_RequestTerminated = 3,                   // Um issue foi terminado


        // Members values (Admins too)
        Member_NewRequestSubmited = 4,                  // Um pedido chegou ao sistema num projecto em que estou incrito
        Member_RequestAcceptedByOtherMember = 5,        // Outro membro do projecto aceitou o pedido
    }

    public enum UserType
    {
        admin = 1,
        member = 2,
        client = 3,
    }
}
