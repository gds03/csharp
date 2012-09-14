using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnhancedLibrary.Utilities.DataAccess;
using System.Data.Common;
using System.Data.SqlClient;

namespace EnhancedLibrary.Console
{
    class MACCarteira
    {
        [Key]
        [Identity]
        public int CarteiraId { get; set; }
        public String Nome { get; set; }
        public int? DinamizadorId { get; set; }
        public int? TecnicoComercialId { get; set; }
        public int? BackupId { get; set; }
        public int CarteiraTipoId { get; set; }
        public int OELocarentId { get; set; }
        public String Descricao { get; set; }
        public int RedeBancariaID { get; set; }
        public bool Activo { get; set; }
        public bool Visivel { get; set; }
        public long UserCria { get; set; }
        public DateTime DataCria { get; set; }
        public long? UserAlt { get; set; }
        public DateTime? DataAlt { get; set; }
        public long? UserDel { get; set; }
        public DateTime? DataDel { get; set; }


        public override string ToString()
        {
            return String.Format(
                @"CarteiraId: {0}, Nome: {1}, DinamizadorId {2}, TecnicoComercialId {3}, BackupId {4}, CarteiraTipoId {5},
                  Descricao {6}, RedeBancariaId {7}, Activo: {8}, Visivel {9}, DataCria {10}",
                  CarteiraId, Nome, DinamizadorId, TecnicoComercialId, BackupId, CarteiraTipoId,
                  Descricao, RedeBancariaID, Activo, Visivel, DataCria);
        }
    }


    public class ObjectMapperTest
    {
        public static void Execute()
        {
            string CONNECTION_STRING = ConnectionStringHelper.SQL2005NotTrusted("LRTDATASQLS304", "PMCDES", "dsides9", "123456");
            DbConnection conn;

            //
            // Initialize one connection and point to PMCDES
            //

            conn = new SqlConnection(CONNECTION_STRING);

            using ( ObjectMapper mapper = new ObjectMapper(conn) )
            {

                //
                // Get all wallets and show them
                // 

                global::System.Console.WriteLine("+++ Showing all wallets +++");
                global::System.Console.ReadLine();

                IList<MACCarteira> allWallets = mapper.Select<MACCarteira>();

                foreach ( var wallet in allWallets )
                {
                    global::System.Console.WriteLine(wallet);
                }



                //
                // Get the last wallet and UPDATE her data
                //


                global::System.Console.WriteLine("+++ Updating the last and retriving from database to see if was updated +++");
                global::System.Console.ReadLine();

                var lastWallet = allWallets.Last();

                lastWallet.Descricao = "updating description";
                lastWallet.Nome = "Updating name";

                mapper.Update(lastWallet);

                // Prove that was updated
                int x;
                lastWallet = mapper.Select<MACCarteira>(c => c.CarteiraId == lastWallet.CarteiraId || c.Descricao == "xpto").Single();


                global::System.Console.WriteLine("+++ Showing updated wallet +++");
                global::System.Console.ReadLine();
                global::System.Console.WriteLine(lastWallet);




                //
                // Create a new wallet and INSERT
                //


                global::System.Console.WriteLine("+++ Inserting a new wallet on database +++");
                global::System.Console.ReadLine();

                var newWallet = new MACCarteira();

                newWallet.Nome = "inserting name";
                newWallet.Descricao = "inserting description";
                newWallet.DinamizadorId = 49;
                newWallet.TecnicoComercialId = null;
                newWallet.BackupId = 43;

                newWallet.CarteiraTipoId = 1;
                newWallet.OELocarentId = 5;

                newWallet.RedeBancariaID = 2;
                newWallet.Activo = true;
                newWallet.Visivel = true;
                newWallet.UserCria = 1;
                newWallet.DataCria = DateTime.Now;

                mapper.Insert(newWallet);


                // Prove that was inserted
                global::System.Console.WriteLine("+++ Showing the id generated from database +++");
                global::System.Console.ReadLine();
                global::System.Console.WriteLine(newWallet.CarteiraId);

                x = newWallet.CarteiraId;
                newWallet = mapper.Select<MACCarteira>(c => c.CarteiraId == x).Single();
                global::System.Console.WriteLine("+++ Showing the inserted wallet +++");
                global::System.Console.ReadLine();
                global::System.Console.WriteLine(newWallet);



                //
                // Delete the inserted wallet
                // 


                global::System.Console.WriteLine("+++ Deleting the new inserted wallet +++");
                global::System.Console.ReadLine();


                allWallets = mapper.Select<MACCarteira>();
                global::System.Console.WriteLine("+++ In total we have {0} records +++", allWallets.Count);
                global::System.Console.ReadLine();

                mapper.Delete(newWallet);


                allWallets = mapper.Select<MACCarteira>();
                global::System.Console.WriteLine("+++ After deletion we have now {0} records +++", allWallets.Count);
                global::System.Console.ReadLine();
            }
        }
    }
}
