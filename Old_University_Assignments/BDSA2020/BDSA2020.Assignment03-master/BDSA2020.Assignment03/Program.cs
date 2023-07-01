using BDSA2020.Assignment03.Entities;
using System;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BDSA2020.Assignment03
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(typeof(Program).Assembly)
                .Build();
/*
                var connectionString = configuration.GetConnectionString("ConnectionString");

                //Console.WriteLine(connectionString);

                using var connection = new SqlConnection(connectionString);

                connection.Open();

                var cmdText = "select * from tasktags";

                using var command = new SqlCommand(cmdText, connection);

                using var reader = command.ExecuteReader();

                while(reader.Read()) Console.WriteLine(reader["Id"]);
            */

            Console.WriteLine("start");

            var context = new Entities.kanbanContext();
            foreach (var user in context.User)
            {
                Console.WriteLine(user.Name + ">>");
                foreach(var temp in user.Task) {
                    Console.WriteLine(temp.Title);
                }
            }


            foreach (var task in context.Task)
            {
                Console.WriteLine(task.AssignedToNavigation?.Name);
            }



            Console.WriteLine("slut");
        }
    }
}
