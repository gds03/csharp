using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Kernel.Contracts;

namespace VirtualNote.Kernel.Managers
{
    
    public sealed class EmailConfigMngr : IEmailConfigMngr
    {
        private const string ConfigKey = "EmailsXmlFilePath";

        private static readonly string FileName;
        private static readonly EmailConfigMngr Singleton = new EmailConfigMngr();
        private static readonly XElement Configs;          // Memory representation data of a xml file!
        private static readonly ReaderWriterLockSlim ManagerLock = new ReaderWriterLockSlim();

        //
        //  Prevenir outros de criarem instancias desta classe
        private EmailConfigMngr() {

        }

        //
        // 1º Vez que referenciarem membros esta classe o CLR carrega as configs
        static EmailConfigMngr(){
            FileName = ConfigurationManager.AppSettings[ConfigKey];
            Configs = XElement.Load(FileName);      // Apenas uma thread pode estar aqui.
        }


        // Helper Methods
        private XElement FindUser(UserType userType, int userId) {
            return Configs.Element(userType.ToString())
                          .Elements("user")
                          .SingleOrDefault(x => (int)x.Attribute("id") == userId);
        }

        
        // Public Properties
        public static EmailConfigMngr Instance {
            get { return Singleton; }
        }









        #region Public interface 

        public IEnumerable<EmailConfig> Find(UserType userType, int userId, out bool hasElement)
        {
            ManagerLock.EnterReadLock();      // As threads so ficam bloqueadas se existir um writer

            try 
            {
                XElement element = FindUser(userType, userId);

                if (element == null) {
                    hasElement = false;
                    return new List<EmailConfig>();
                }

                hasElement = true;
                return element.Elements("value")
                              .Select(x => (EmailConfig)x.Value.ToInt());
            }
            finally {
                ManagerLock.ExitReadLock();
            }
        }

        public bool Add(UserType userType, int userId, IEnumerable<EmailConfig> values)
        {
            ManagerLock.EnterReadLock();            // Read

            try {
                if (FindUser(userType, userId) != null)
                    return false;
            }
            finally{
                ManagerLock.ExitReadLock();         // EO Read
            }

            
            ManagerLock.EnterWriteLock();           // Write

            try {
                Configs.Element(userType.ToString())                                   // Não retorna null porque nunca é apagado e existe sempre
                       .Add(new XElement("user",
                                new XAttribute("id", userId),                          // Coloca atributo
                                values.Select(c => new XElement("value", (int)c))      // Coloca coleccao de values
                ));

                // Save persistent to file
                Configs.Save(FileName);
                return true;
            }

            finally{
                ManagerLock.ExitWriteLock();        // EO Write
            }
        }

        public bool Update(UserType userType, int userId, IEnumerable<EmailConfig> values) 
        {
            ManagerLock.EnterReadLock();           // Read
            XElement userElement;

            try {
                if ((userElement = FindUser(userType, userId)) == null)
                    return false;
            }
            finally{
                ManagerLock.ExitReadLock();         // EO Read
            }

            ManagerLock.EnterWriteLock();           // Write

            try {
                // Apaga todos
                userElement.RemoveNodes();

                // Insere todos (actualizado)
                userElement.Add(values.Select(c => new XElement("value", (int)c)));

                // Save persistent to file
                Configs.Save(FileName);
                return true;
            }
            finally{
                ManagerLock.ExitWriteLock();        // EO Write
            }
        }

        public bool Delete(UserType userType, int userId) {
            ManagerLock.EnterReadLock();            // Read

            XElement userElement;

            try {
                if ((userElement = FindUser(userType, userId)) == null)
                    return false;
            }
            finally{
                ManagerLock.ExitReadLock();         // EO Read
            }


            ManagerLock.EnterWriteLock();           // Write

            try {
                userElement.Remove();

                // Save persistent
                Configs.Save(FileName);
                return true;
            }
            finally{
                ManagerLock.ExitWriteLock();       // EO Write
            }
        }


        #endregion

    }
}
