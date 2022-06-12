using Dapper;
using DapperConsoleDemo.BaseModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace DapperConsoleDemo
{
    class Program
    {
        // Create connection to process for all methods
        static IDbConnection baseConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["BaseDBConnection"].ConnectionString);
        static IDbConnection mainConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MainDBConnection"].ConnectionString);
        static void Main(string[] args)
        {
            // Main examples with all basic features, function, properties of Dapper
            BaseDapperExamplesDemo();
        }

        public static void BaseDapperExamplesDemo()
        {
            Console.WriteLine("******* Start Demo Example Now! *********");

            // Get All data and print
            SummaryCountByExecuteScalar();
            //QueryAllDataSimple();

            ////Query simple data by pass a param
            //Customer customerRecord = QuerySimpleDataById();

            ////Insert data sample
            //InsertSingleDataSimple();

            //// Update/Delete a record data
            //UpdateEmailDataRecord();
            //DeleteCustomerRecord();

            //// Parameters
            //ParameterInDapperExamples();

            /// Utilities
            //Async - Transaction in Dapper
            //Task.WaitAll(DapperAsyncExamples());
            //DapperBufferExamples();
            //// Query by execute store procedure
            //GetFullDataByStoreProcedure();

            //// Advance Dapper
            //// Call multiple queries with many tables
            //GetMultipleTables();


            // Multiple-Mapping with Dapper
            //QueryAllBillsMutipleMappingOneToMany();



            //// Call and execute store procedure with dynamic parameters
            //ExecuteStoreProcedureWithDynamicParams();

            /// Bulk Operations with Dapper Plus
            //BulkInsertDataExamples();

            Console.WriteLine($"---- End -----");
        }

        #region Base methods, functions, features Dapper
        private static void SummaryCountByExecuteScalar()
        {
            string query = "Select count(*) From Customers";
            using (var db = baseConnection)
            {
                var count = db.ExecuteScalar(query);

                Console.WriteLine($"Database have {count} customers : ");
            }
        }

        private static void QueryAllDataSimple()
        {
            string query = "Select * From Customers";
            using (var db = baseConnection)
            {
                List<Customer> allCustomerData = db.Query<Customer>(query).ToList();

                Console.WriteLine($"Database have {allCustomerData.Count} customers : ");
                foreach (var customer in allCustomerData)
                {
                    Console.WriteLine($"First name: {customer.FirstName} --- Last name: {customer.LastName} --- Email: {customer.Email}");
                }
            }

        }

        private static Customer QuerySimpleDataById(int customerId = -1)
        {
            using (var db = baseConnection)
            {
                string query = "Select * From Customers where Id=@id";
                Console.WriteLine($"Please input Id need to query data:");
                int cusId = (customerId == -1) ? Convert.ToInt32(Console.ReadLine()) : customerId;
                var customer = db.QueryFirstOrDefault<Customer>(query, new { id = cusId });
                if (customer == null)
                {
                    Console.WriteLine($"---- Do not exist customer with this ID -----");
                    return null;
                }
                else
                {
                    Console.WriteLine($"First name: {customer.FirstName} --- Last name: {customer.LastName} --- Email: {customer.Email}");
                    return customer;
                }
            }
        }

        private static void GetMultipleTables()
        {
            using (var db = baseConnection)
            {
                Console.WriteLine($"Please input Id need to query data:");
                var id = Console.ReadLine();
                var query = "Select * from Customers where Id = @id ; Select * from Bills where customerId = @id;";
                var multipleResults = db.QueryMultiple(query, new { id = id });
                var customer = multipleResults.Read<Customer>().SingleOrDefault();
                var bills = multipleResults.Read<Bill>().ToList();

                if (customer != null && bills != null)
                {
                    customer.Bills.AddRange(bills);
                }

                Console.WriteLine($"Customer Name : {customer.FirstName + customer.LastName}");
                foreach (var bill in customer.Bills)
                    Console.WriteLine($"Bill info: {bill.Id} -- {bill.Price} -- {bill.Quantity} -- {bill.Color}");
            }

        }

        private static void InsertSingleDataSimple()
        {
            using (var db = baseConnection)
            {
                var customerInfo = new Customer();
                Console.WriteLine($"Please input First name:");
                customerInfo.FirstName = Console.ReadLine();
                Console.WriteLine($"Please input Last name:");
                customerInfo.LastName = Console.ReadLine();
                Console.WriteLine($"Please input Email:");
                customerInfo.Email = Console.ReadLine();

                string query = "Insert into Customers (FirstName, LastName, Email) values(@firstName, @lastName, @email); Select Cast (Scope_Identity() as int)";

                int customerId = db.Execute(query, customerInfo);
                Console.WriteLine($"Added customer successfully");
            }

        }

        private static void DeleteCustomerRecord()
        {
            using (var db = baseConnection)
            {
                Console.WriteLine($"Please input Id need to delete");
                var id = Console.ReadLine();
                int Id = int.Parse(id);
                var customer = QuerySimpleDataById(Id);
                if (customer != null)
                {
                    string query = "Delete Customers where Id = @id";
                    db.Execute(query, new { id = Id });
                    Console.WriteLine($"Delete this customer record successfully!");
                }
            }
        }

        private static void GetFullDataByStoreProcedure()
        {
            using (var db = baseConnection)
            {
                Console.WriteLine($"Please input Id need to query data:");
                var id = Console.ReadLine();

                using (var resultData = db.QueryMultiple("sp_GetCustomer_Bill", new { id = id }, commandType: CommandType.StoredProcedure))
                {
                    var customer = resultData.Read<Customer>().SingleOrDefault();
                    var bills = resultData.Read<Bill>().ToList();
                    if (customer != null && bills != null)
                    {
                        customer.Bills.AddRange(bills);
                    }
                    Console.WriteLine($"Customer Name : {customer.FirstName + customer.LastName}");
                    foreach (var bill in customer.Bills)
                        Console.WriteLine($"Bill info: {bill.Id} -- {bill.Price} -- {bill.Quantity} -- {bill.Color}");
                }
            }
        }

        private static void UpdateEmailDataRecord()
        {
            using (var db = baseConnection)
            {
                Console.WriteLine($"Please input Id need to update email data:");
                var id = Console.ReadLine();
                int Id = int.Parse(id);
                var customer = QuerySimpleDataById(Id);
                if (customer != null)
                {
                    Console.WriteLine($"Input an Email to update this customer record:");
                    var newEmail = Console.ReadLine();
                    var param = new DynamicParameters();
                    param.Add("id", Id);
                    param.Add("email", newEmail);
                    string query = "Update Customers set Email = @email where Id = @id";
                    db.Execute(query, param);

                    Console.WriteLine($"Update this customer record successfully!");
                }
            }
        }


        private static async Task DapperAsyncTransactionExamples()
        {
            Console.WriteLine($"Please input First name:");
            string fName = Console.ReadLine();
            Console.WriteLine($"Please input Last name:");
            string lName = Console.ReadLine();
            string query = $"Insert into Customers(FirstName, LastName, Email) values(@firstName, @lastName, '') ";

            using (var db = baseConnection)
            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    await db.ExecuteAsync(query, new { firstName = fName, lastName = lName }, commandType: CommandType.Text, transaction: transaction);
                    await Task.Delay(1000);
                    transaction.Commit();
                    Console.WriteLine("Created new customer successfully");

                }
                catch (Exception e) // Rollback operation transaction if have any error or exception
                {
                    transaction.Rollback();
                }
            }
        }
        private static void DapperBufferExamples()
        {
            string query = $"Select top 5 * from Customers";
            using (var db = baseConnection)
            {
                var dataResult = db.Query<Customer>(query, commandType: CommandType.Text, buffered: false);
                dataResult = dataResult.Where(x => x.Id == 3);
                var result = dataResult.ToList();
                if (result.Count > 0)
                    Console.WriteLine($"Found {result.Count} customers");
            }
        }

        private static void ExecuteStoreProcedureWithDynamicParams()
        {
            var customer = new Customer();
            Console.WriteLine($"Please input First name:");
            customer.FirstName = Console.ReadLine();
            Console.WriteLine($"Please input Last name:");
            customer.LastName = Console.ReadLine();
            Console.WriteLine($"Please input Email:");
            customer.Email = Console.ReadLine();

            var parameters = new DynamicParameters();
            parameters.Add("@id", customer.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            parameters.Add("@firstName", customer.FirstName);
            parameters.Add("@lastName", customer.LastName);
            parameters.Add("@email", customer.Email);
            using (var db = baseConnection)
            {
                db.Execute("sp_SaveCustomer", parameters, commandType: CommandType.StoredProcedure);
                customer.Id = parameters.Get<int>("@id");

                Console.WriteLine("New Contact Created With ID {0} ", customer.Id);
            }
        }

        //private static void QueryAllBillsMutipleMappingOneToMany()
        //{
        //    string query = @"select Code, Color, Quantity, Price, c.CustomerId, c.FirstName, c.LastName from Bills b
        //                    inner join Customers c on c.Id = b.CustomerId";

        //    var bills = db.Query<Bill, Bill, Customer>(query, (bill, customer) =>
        //       {
        //           bill.CustomerId = customer.Id;
        //           return bill;
        //       },
        //       splitOn: "CustomerId");

        //    bills.ToList().ForEach(b => Console.WriteLine($"Bill code:{b.Code}"));

        //    Console.ReadLine();
        //}

        private static void ParameterInDapperExamples()
        {
            using (var db = baseConnection)
            {
                // Parameter with string Type
                string query1 = $"Select * from Customers where LastName = @name";
                var param1 = new { name = new DbString { Value = "Kelvin", IsFixedLength = false, IsAnsi = true } };
                var dataResult = db.QueryFirstOrDefault<Customer>(query1, param1);

                // Parameter with Anonymous Type
                string query2 = $"Select * from Customers where FirstName = @firstName and LastName = @lastName";
                var param2 = new { firstName = "Kelvin", lastName = "Lee" };
                dataResult = db.QueryFirstOrDefault<Customer>(query2, param2);

                // Parameter with Dynamic Type
                string query3 = $"Select * from Customers where CustomerId = @customerId";
                var param3 = new DynamicParameters();
                param3.Add("@customerId", 1);
                dataResult = db.QueryFirstOrDefault<Customer>(query3, param3);

                if (dataResult != null)
                {
                    Console.WriteLine($"Found data customer: {dataResult.FirstName} {dataResult.LastName} ");
                }
            }
        }

        private static void BulkInsertDataExamples()
        {
            using (var db = baseConnection)
            {
                // Insert only single item
                DapperPlusManager.Entity<Customer>().Table("Customers");
                var singleData = new List<Customer> {
                new Customer{FirstName = "CustomerA", LastName = "Bulk_001"},
                new Customer{FirstName = "CustomerB", LastName = "Bulk_002"},
                };
                db.BulkInsert(singleData);

                // Insert only multiple item with relation other
                DapperPlusManager.Entity<Customer>().Table("Customers").Identity(x => x.Id);
                DapperPlusManager.Entity<Bill>().Table("Bills").Identity(x => x.Id);
                var multiData = new List<Customer> {
                    new Customer{FirstName = "Test Customer", LastName="BulkMulti_001",
                    Bills = new List<Bill>{
                            new Bill{Code = "HD_001", Color = "Yellow", Quantity = 15, Price = 250} ,
                            new Bill{Code = "HD_002", Color = "Red", Quantity = 11, Price = 270}},
                }};
                db.BulkInsert(multiData).ThenForEach(x =>
                {
                    foreach (var item in x.Bills)
                    {
                        item.CustomerId = x.Id;
                    }
                }).ThenBulkInsert(x => x.Bills);

                Console.WriteLine($"Bulk insert data successfully");
            }
        }
        #endregion
    }
}
