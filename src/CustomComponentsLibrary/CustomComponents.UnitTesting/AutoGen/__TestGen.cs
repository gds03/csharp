using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.UnitTesting.AutoGen
{
    public static class __TestGen
    {
        const bool generateWithingComments = true;
        static readonly string[] submitPattern = { "subm", "inser", "updat" };
        static readonly string[] excludedServices = {
                                                        // as we advance and do the tests
                                                        // "AdmissionService",
                                                        // "AppointmentService",

                                                        // Statically setted 
                                                        "ApplicationService",
                                                        "BaseService", 


                                                    };

        const string BusinessLayerAssemblyName = "BusinessDLLname";
        const string serviceSearchPattern = "*Service.cs";

        const string inputBusinessDLLFilePath = @"BusinessDLLPath";
        const string inputServicesDiretory = @"ServicesFolderPath";
        const string outputTestsDiretory = @"GeneratedFolderPath";


        public static void Run()
        {
            int errorCount = 0;
            var directoryInfo = new DirectoryInfo(inputServicesDiretory);
            var serviceFiles = directoryInfo.GetFiles(serviceSearchPattern);

            foreach (var fileInfo in serviceFiles)
            {
                string testClassFileName = fileInfo.Name.Replace(".cs", "Test.cs");     // the name of the test
                string testClassType = testClassFileName.Replace(".cs", "");            // test type name
                string serviceName = testClassType.Replace("Test", "");             // service name
                string entityType = serviceName.Replace("Service", "");           // entity name 

                if (excludedServices.Any(s => serviceName.ToLower().Trim().IndexOf(s.ToLower().Trim()) >= 0))
                    continue;   // ignore and do not generate a test class for this

                FileStream testFs = null;
                StringBuilder bufferedString = new StringBuilder();

                try
                {
                    string finalFileName = Path.Combine(outputTestsDiretory, testClassFileName);
                    testFs = File.Create(finalFileName);

                    // Set usings.
                    WriteUsings(bufferedString);
                    WriteNameSpace(bufferedString);
                    WriteOpenBracet(bufferedString, 0);

                    StartClassTag(testClassType, bufferedString);
                    WriteOpenBracet(bufferedString, 1);

                    WriteMembers(serviceName, bufferedString);
                    WriteMethods(serviceName, entityType, bufferedString);

                    WriteCloseBracet(bufferedString, 1);       // ends class
                    WriteCloseBracet(bufferedString, 0);       // end namespace

                    WriteToFile(testFs, bufferedString.ToString());
                }

                catch (Exception ex)
                {
                    errorCount++;

                    if (testFs != null)
                        WriteErrorMessage(testFs, ex.Message);
                }

                finally
                {
                    if (testFs != null)
                        // finally close file handlers.
                        testFs.Close();

                }
            }

            Console.WriteLine("Found {0} errors", errorCount.ToString());
        }

        static void WriteErrorMessage(FileStream fs, string errorMsg)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(" +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ ");
            sb.AppendLine(" +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ ");
            sb.AppendLine(" +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ ");

            sb.AppendLine(" +++++++++++++++++++++++++++ Error +++++++++++++++++++++++++++++++++ ");

            sb.AppendLine(" +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ ");
            sb.AppendLine(" +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ ");
            sb.AppendLine(" +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ ");

            sb.AppendLine();
            sb.AppendLine(errorMsg);

            WriteToFile(fs, sb.ToString());
        }



        static void WriteToFile(FileStream fs, string text)
        {
            if (generateWithingComments)
                text = text.Replace("\n", "\n //");

            byte[] data = ASCIIEncoding.UTF8.GetBytes(text);
            fs.Write(data, 0, data.Length);
            fs.Flush();                     // should remove this later.
        }



        static void WriteUsings(StringBuilder sb)
        {
            string text =
@"
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;            
using System.Reflection;
";

            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        static void WriteNameSpace(StringBuilder sb)
        {
            string text = @"namespace YOURNAMESPACE
";

            // WriteToFile(fs, text);
            sb.Append(text);
        }

        static void WriteOpenBracet(StringBuilder sb, int tabs)
        {
            StringBuilder s = new StringBuilder();
            while (tabs-- > 0)
                s.Append("\t");

            s.Append("{");

            //  WriteToFile(fs, text);
            sb.AppendLine(s.ToString());
        }

        static void WriteCloseBracet(StringBuilder sb, int tabs)
        {
            StringBuilder s = new StringBuilder();
            while (tabs-- > 0)
                s.Append("\t");

            s.Append("}");

            //  WriteToFile(fs, text);
            sb.AppendLine(s.ToString());
        }

        static void StartClassTag(string ClassTestName, StringBuilder sb)
        {
            string text = string.Format(@"
    [TestClass]
    public class {0}
", ClassTestName);

            // WriteToFile(fs, text);
            sb.Append(text);
        }

        static void WriteMembers(string serviceName, StringBuilder sb)
        {
            string text = string.Format(@"
        readonly {0} Service = new {0}( () => new YOURREPOSITORY() );
        readonly Configs Config = new Configs();

        int dummyInsertedID;
        ", serviceName);

            // WriteToFile(fs, text);
            sb.Append(text);
        }


        static void WriteMethods(string serviceName, string entityType, StringBuilder sb)
        {
            WriteInitialize(sb);
            WriteCleanup(sb);
            WriteCreateContextEntity(entityType, sb);
            WriteRemoveById(entityType, sb);
            WriteGetById(entityType, sb);
            WriteGetsRegion(entityType, sb);
            List<List<string>> submitObjectsPropertiesStructure = WriteSubmitObjects(serviceName, sb);

            WriteInsertAndUpdateSections(entityType, submitObjectsPropertiesStructure, sb);
            WriteAssertSections(entityType, submitObjectsPropertiesStructure, sb);
        }

        private static void WriteGetsRegion(string entityType, StringBuilder sb)
        {
            string text = @"
        
        #region Gets

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void " + entityType + @"_GetInvalid1()
        {
            Service.GetByBenificiaryId(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidValueException))]
        public void " + entityType + @"_GetInvalid2()
        {
            Service.GetByBenificiaryId(0);
        }

        [TestMethod]
        public void " + entityType + @"_GetInvalidHighId()
        {
            var result = Service.GetByBenificiaryId(1000000);
            Assert.AreEqual(0, result.Count);
        }

        // This method should execute after the insertion.
        [TestMethod]
        public void " + entityType + @"_GetValid()
        {
            var contextEntities = Service.GetByBenificiaryId(Config.CreateOrGetDummyBeneficiary());

            " + entityType + @" contextGet = contextEntities.Items.First();
            " + entityType + @" contextEf = GetById(contextGet.Id);

            Compare" + entityType + @"s(contextGet, contextEf);
            Assert.IsTrue(contextEntities.Count > 0);
        }
        #endregion";
            sb.AppendLine(text);
        }

        static void WriteInsertAndUpdateSections(string entityType, List<List<string>> submitObjectsPropertiesStructure, StringBuilder sb)
        {
            WriteInsert(entityType, submitObjectsPropertiesStructure, sb);
            WriteUpdate(entityType, submitObjectsPropertiesStructure, sb);
        }



        static void WriteAssertSections(string entityType, List<List<string>> submitObjectsPropertiesStructure, StringBuilder sb)
        {
            WriteSubmitAsserts(entityType, submitObjectsPropertiesStructure, sb);
            WriteCompareAsserts(entityType, sb);
        }





        static void WriteInsert(string entityType, List<List<string>> submitObjectsPropertiesStructure, StringBuilder sb)
        {
            int idx = 1;
            foreach (List<string> properties in submitObjectsPropertiesStructure)
            {
                string text = @"
        [TestMethod]
        public void " + entityType + "_Insert" + idx + @"()
        {
            SubmitObject" + idx + " s = new SubmitObject" + idx + @"();
            Configs.InitializeDefaults(s);

            " + entityType + @" InsertedEntity = null;
            
            try {
                
                int id = Service.Submit(
                    null,               // <---------------------------  Insert
                    Config.CreateOrGetDummyBeneficiary(),
                    " + FormatParametersForMethod("s", properties) + @"
                );
                
                InsertedEntity = GetById(id);
                Assert.IsNotNull(InsertedEntity);

                // Compare the values inserted with the db values
                Compare" + entityType + "Submit" + idx + @"(InsertedEntity, s);
            }

            finally {
                RemoveById(InsertedEntity.Id);
            }
        }";
                sb.Append(text);
            }
        }



        static void WriteUpdate(string entityType, List<List<string>> submitObjectsPropertiesStructure, StringBuilder sb)
        {
            int idx = 1;
            foreach (List<string> properties in submitObjectsPropertiesStructure)
            {
                string parametersChain = FormatParametersForMethod("s", properties);
                parametersChain = parametersChain.Replace(",", "=\n");
                string text = @"
        [TestMethod]
        public void " + entityType + "_Update" + idx + @"()
        {
            SubmitObject" + idx + " s = new SubmitObject" + idx + @"();
            Configs.InitializeDefaults(s);
            
            /*
            " + parametersChain +
@"            
            */
             int id = Service.Submit(
                    dummyInsertedID,               // <---------------------------  Update
                    Config.CreateOrGetDummyBeneficiary(),
                    " + FormatParametersForMethod("s", properties) + @"
                );
                
                var x = GetById(dummyInsertedID);

                // Compare the values inserted with the db values
                Compare" + entityType + "Submit" + idx + @"(x, s, true);
        }";
                sb.Append(text);
            }
        }


        static string FormatParametersForMethod(string submitObjectVariableName, List<string> properties)
        {
            var sbuilder = properties.Aggregate(new StringBuilder(), (sb, p) => sb.Append(submitObjectVariableName + "." + p + ", "));
            return sbuilder.Remove(sbuilder.Length - 2, 2).ToString();
        }




        static void WriteSubmitAsserts(string entityType, List<List<string>> submitObjectsPropertiesStructure, StringBuilder sb)
        {
            sb.AppendLine(@"
        //
        // Assert sections

        ");


            for (int idx = 0; idx < submitObjectsPropertiesStructure.Count; idx++)
            {
                sb.AppendLine(@"

        /// <summary>
        ///     Typically here is where you compare the submited data with db data that previosly inserted/updated.
        /// </summary>
        void Compare" + entityType + "Submit" + (idx + 1) + "( " + entityType + " x, SubmitObject" + (idx + 1) + " st, bool update = false) {");

                sb.AppendLine(@"
            if( x == null )
                Assert.Fail();

            if( st == null )
                Assert.Fail();

            //
            // TODO: Compare mappings here

        }");
            }




            sb.AppendLine(@"
        //
        // EO Assert sections

        ");
        }


        static void WriteCompareAsserts(string entityType, StringBuilder sb)
        {
            sb.AppendLine(@"
        /// <summary>
        ///     Typically here is where you compare the data from db and from the get your the service.
        /// </summary>
        void Compare" + entityType + "s( " + entityType + " a, " + entityType + " b ) {");

            sb.AppendLine(@"
            if ( a == null )
                Assert.Fail();

            if ( b == null )
                Assert.Fail();

            //
            // TODO: Compare Mappings 


        }");
        }


        static void WriteInitialize(StringBuilder sb)
        {
            string text = @"
        [TestInitialize]
        public void Initialize()
        {
            string email;
            Config.CreateOrGetDummyUser(out email);

            // Set the principal and create a new user
            Configs.SetThreadPrincipal(email);

            // Create dummy benef
            Config.CreateOrGetDummyBeneficiary();

            // Create a dummy entity
            CreateContextEntityDummy();            
        }";
            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        static void WriteCleanup(StringBuilder sb)
        {
            string text = @"
        [TestCleanup]
        public void Clean()
        {
            // Remove dummy entity
            RemoveById(dummyInsertedID);

            // Remove dummy benef
            Config.DeleteDummyBenificiary();

            // Remove dummy user
            Config.DeleteDummyUser();
        }";
            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        static void WriteCreateContextEntity(string entityType, StringBuilder sb)
        {
            string text = @"
        void CreateContextEntityDummy()
        {
            " + entityType + @" a = new " + entityType + @"();
            Configs.InitializeDefaults(a);

            int beneficiaryID = Config.CreateOrGetDummyBeneficiary();
            using ( var Context = Configs.NewContext() )
            {
                a.BeneficiaryId = beneficiaryID;
                Context." + entityType + @".Add(a);
                Context.SaveChanges();
            }

            // set the id
            dummyInsertedID = a.Id;
        }";

            //  WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        static void WriteRemoveById(string entityType, StringBuilder sb)
        {
            string text = @"
        bool RemoveById(int id)
        {
            " + entityType + @" a = null;
            using ( var context = Configs.NewContext() )
            {
                a = context." + entityType + @".FirstOrDefault(x => x.Id == id);
                context." + entityType + @".Remove(a);
                context.SaveChanges();
            }

            return (a != null);
        }";

            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        //
        // AT THE END

        static void WriteGetById(string entityType, StringBuilder sb)
        {
            string text = @"
        " + entityType + @" GetById(int id)
        {

            return Configs.NewContext()." + entityType + @"
                                       //.Include(x => x.)        // TODO with specific includes
                                       .Single(x => x.Id == id);
            

        }";

            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        static void WriteCompareContextSubmit(string entityType, StringBuilder sb)
        {
            string text = @"
        void Compare" + entityType + @"Submit(" + entityType + @" a, SubmitObject st, bool update = false)
        {
            if ( a == null )
                Assert.Fail();

            if ( st == null )
                Assert.Fail();

            //
            // TODO with specific asserts.

        }";

            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }

        static void WriteCompareContexts(string entityType, StringBuilder sb)
        {
            string text = @"
        void Compare" + entityType + @"s(" + entityType + @" a, " + entityType + @" b)
        {
            if ( a == null )
                Assert.Fail();

            if ( b == null )
                Assert.Fail();

            //
            // TODO with specific asserts.

        }";

            // WriteToFile(fs, text);
            sb.AppendLine(text);
        }





        static Type GetServiceContextType(string serviceName)
        {
            Assembly asm = Assembly.LoadFrom(inputBusinessDLLFilePath);
            return asm.DefinedTypes.Where(ti => ti.Name.Contains(serviceName)).Single().UnderlyingSystemType;
        }

        /// <summary>
        ///     Return how many submit object are generated and their propertyNames
        /// </summary>
        static List<List<string>> WriteSubmitObjects(string serviceName, StringBuilder sbOutter)
        {
            Type ServiceType = GetServiceContextType(serviceName);
            var serviceMethods = ServiceType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            List<MethodInfo> submitMethods = new List<MethodInfo>();
            List<List<string>> result = new List<List<string>>();

            foreach (var mi in serviceMethods)
            {
                if (submitPattern.Any(x => mi.Name.ToLower().IndexOf(x.ToLower().Trim()) >= 0))
                    submitMethods.Add(mi);
            }

            if (submitMethods.Count > 0)
            {
                sbOutter.AppendLine(@"
        
        //
        // Submit Auto-Generated Objects for this service


                ");

            }

            // Inside each method of submit, build a object that contain parameters
            int idx = 1;
            foreach (var submitMethod in submitMethods)
            {
                List<string> submitProperties = new List<string>();
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(@"
        class SubmitObject" + idx++ + " {");
                foreach (var pi in submitMethod.GetParameters())
                {
                    submitProperties.Add(pi.Name);
                    string paramType = GetTypeForParam(pi.ParameterType);
                    string text = @"
            public " + paramType + " " + pi.Name + " { get; set; }";
                    sb.Append(text);
                }

                sb.AppendLine(@"
        }");
                // WriteToFile(fs, sb.ToString());
                sbOutter.Append(sb.ToString());
                result.Add(submitProperties);
            }


            if (submitMethods.Count > 0)
            {
                sbOutter.AppendLine(@"
        
        //
        // EO - Submit Auto-Generated Objects for this service


                ");
            }

            return result;
        }

        static string GetTypeForParam(Type type)
        {
            // 
            // Format e.g.: System.Nullable`1[System.Int32] to System.Nullable<System.Int32>

            if (type.GenericTypeArguments != null && type.GenericTypeArguments.Length > 0)
                return type.ToString().Replace("`1[", "<").Replace("]", ">");

            // return normal
            return type.ToString();
        }
    }
}
