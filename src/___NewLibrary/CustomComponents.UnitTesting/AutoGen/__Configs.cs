//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace CustomComponents.UnitTesting.AutoGen
//{
//    public class Configs
//    {
//        // dummy ids
//        private int? BeneficiaryID { get; set; }

//        private int? UserID { get; set; }
//        private string Email { get; set; }

//        /// <summary>
//        ///     Returns the ID of the new dummy beneficiary
//        /// </summary>
//        public int CreateOrGetDummyBeneficiary()
//        {
//            if (BeneficiaryID != null)
//                return BeneficiaryID.Value;

//            Guid beneficiaryName = Guid.NewGuid();

//            using (var Context = NewContext())
//            {
//                // Create user
//                var newBenef = new Beneficiary
//                {
//                    BeneficiaryNr = new Random().Next(100000000).ToString(),
//                    Name = beneficiaryName.ToString(),
//                    BirthDate = DateTime.Now,
//                    Sex = "M",
//                    NrChildren = 4,
//                    NationalHealthServiceNr = "abc",
//                    HealthTaxExemption = false,
//                    MaritalStatusId = new QueryTable<GenericMasterData>(Context.GenericMasterData).GetMaritalStatusList().First().Id,
//                    EducationDegreeId = new QueryTable<GenericMasterData>(Context.GenericMasterData).GetEducationDegreeList().First().Id,
//                    BeneficiaryTypeId = new QueryTable<GenericMasterData>(Context.GenericMasterData).GetBeneficiaryTypeList().First().Id,
//                    EmploymentSituationId = new QueryTable<GenericMasterData>(Context.GenericMasterData).GetEmploymentSituationList().First().Id,
//                    UpdatedBy = 1,

//                    CreatedOn = DateTime.Now,
//                    UpdatedOn = DateTime.Now
//                };

//                Context.Beneficiary.Add(newBenef);
//                Context.SaveChanges();

//                BeneficiaryID = newBenef.Id;
//                return BeneficiaryID.Value;
//            }
//        }

//        /// <summary>
//        ///     Deletes the dummy beneficiary from db
//        /// </summary>
//        public void DeleteDummyBenificiary()
//        {
//            if (BeneficiaryID != null)
//            {
//                using (var Context = NewContext())
//                {
//                    var beneficiary = Context.Beneficiary.Single(x => x.Id == BeneficiaryID.Value);
//                    Context.Beneficiary.Remove(beneficiary);
//                    Context.SaveChanges();
//                    BeneficiaryID = null;
//                }
//            }
//        }


//        /// <summary>
//        ///     Returns the ID of the new dummy user
//        /// </summary>
//        public int CreateOrGetDummyUser(out string Email)
//        {
//            if (UserID != null)
//            {
//                Email = this.Email;
//                return UserID.Value;
//            }

//            Guid userEmail = Guid.NewGuid();

//            using (var Context = NewContext())
//            {
//                // Create and Insert User
//                var user = new User
//                {
//                    CreatedOn = DateTime.Now,
//                    Email = Email = userEmail.ToString(),
//                    FullName = "Utilizador Teste",
//                    IsActive = true,
//                    Password = "passw0rd",
//                    TempGuid = new Guid().ToString(),
//                    UpdatedOn = DateTime.Now,
//                    Salt = ""
//                };

//                var userRole = new UserRole
//                {
//                    FK_User = user,
//                    RoleId = 4,                 // medico
//                    IsActive = true,
//                    CreatedOn = DateTime.Now
//                };

//                Context.User.Add(user);
//                Context.UserRole.Add(userRole);
//                Context.SaveChanges();

//                UserID = user.Id;
//                return UserID.Value;

//            }
//        }


//        /// <summary>
//        ///     Deletes the dummy user from db
//        /// </summary>
//        public void DeleteDummyUser()
//        {
//            if (UserID != null)
//            {
//                using (var Context = NewContext())
//                {
//                    var User = Context.User.Single(x => x.Id == UserID.Value);

//                    User.Refs_UserRole.ToList().ForEach(ur => Context.UserRole.Remove(ur));

//                    Context.User.Remove(User);
//                    Context.SaveChanges();

//                    UserID = null;
//                }
//            }
//        }

//        public static PortalDoencaCronicaDB NewContext()
//        {
//            var context = new PortalDoencaCronicaDB();

//            return context;
//        }


//        public static void SetThreadPrincipal(string email)
//        {
//            if (email == null)
//                throw new ArgumentNullException("email");

//            IIdentity i = new GenericIdentity(email);
//            Thread.CurrentPrincipal = new CustomPrincipal(new CustomIdentity(i));
//        }

//        public static int CountElements<T>(PortalDoencaCronicaDB context) where T : class
//        {
//            return
//                ((DbSet<T>)
//                    context.GetType()
//                        .GetProperty(typeof(T).Name, BindingFlags.Public | BindingFlags.Instance)
//                        .GetValue(context, null)).Count();
//        }


//        public static T InitializeDefaults<T>(T obj)
//        {
//            if (obj == null)
//                throw new ArgumentNullException("obj");

//            Random rand = new Random();

//            foreach (var x in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList())
//            {
//                Type propertyType = x.PropertyType;
//                int v = 1;

//                if (propertyType == typeof(bool))
//                {
//                    x.SetValue(obj, true);
//                    continue;
//                }

//                if (propertyType == typeof(byte))
//                {
//                    x.SetValue(obj, (byte)v);
//                    continue;
//                }

//                if (propertyType == typeof(short))
//                {
//                    x.SetValue(obj, (short)v);
//                    continue;
//                }

//                if (propertyType == typeof(float))
//                {
//                    x.SetValue(obj, (float)v);
//                    continue;
//                }

//                if (propertyType == typeof(int))
//                {
//                    x.SetValue(obj, v);
//                    continue;
//                }


//                if (propertyType == typeof(decimal))
//                {
//                    x.SetValue(obj, v);
//                    continue;
//                }

//                if (propertyType == typeof(long))
//                {
//                    x.SetValue(obj, v);
//                    continue;
//                }

//                if (propertyType == typeof(DateTime))
//                {
//                    x.SetValue(obj, DateTime.Now);
//                    continue;
//                }

//                if (propertyType == typeof(Guid))
//                {
//                    x.SetValue(obj, Guid.NewGuid());
//                    continue;
//                }

//                if (propertyType == typeof(string))
//                {
//                    string s = Guid.NewGuid().ToString();
//                    x.SetValue(obj, s);
//                    continue;
//                }

//                x.SetValue(obj, null);
//            };

//            return obj;
//        }
//    }
//}
