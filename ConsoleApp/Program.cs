using Grpc.Core;
using Grpc.Net.Client;
using GrpcService;
using System.Threading.Channels;

namespace ConsoleApp;

public class Program
{
    static async Task Main(string[] args)
    {

        var requestGetAll = new CustomerGetAllRequest();
        var getOne = new CustomerRequest { Id = 1 };
        var addedCustomer = new CustomerDTORequest { FirstName = "Nadezhda", Age = 20, Email = "example.com", LastName = "Bichun" , IsBlocked=false};
        var blockedUser = new CustomerRequest { Id = 1 };

        // Set the base address of your gRPC server
        string baseAddress = "http://localhost:5085";

        var channel = GrpcChannel.ForAddress(baseAddress);
        var client = new Customers.CustomersClient(channel);

        Console.WriteLine("Get all not blocked users: ");
        using (var call = client.GetAll(requestGetAll))
        {
            while (await call.ResponseStream.MoveNext())
            {
                var customer = call.ResponseStream.Current;
                Console.Write(customer.FirstName + " ");
            }
        }

        try
        {
            var userOne = client.Get(getOne);
        Console.WriteLine($"\nGet user with id 1: {userOne.FirstName} {userOne.LastName}, {userOne.Age}");
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

        try
        {
            var addCustomer = client.Create(addedCustomer);
            Console.WriteLine("Successfull add new customer");
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }

        try
        {
            var blockUser = client.Block(blockedUser);
            Console.WriteLine($"Successfull block customer {blockedUser.Id}");
        }
        catch(Exception ex) { Console.WriteLine(ex.Message); }




        Console.WriteLine("Get all not blocked users: ");
        using (var call = client.GetAll(requestGetAll))
        {
            while (await call.ResponseStream.MoveNext())
            {
                var customer = call.ResponseStream.Current;
                Console.Write($"{customer.FirstName} {customer.LastName}, {customer.Age}");
            }
        }



        Console.ReadLine();
    }
}