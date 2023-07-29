using System.Collections.Generic;
using AutoMapper;
using Grpc.Core;

namespace GrpcService;

public class CustomersService : Customers.CustomersBase
{
    private IMapper _mapper;
    public CustomersService(IMapper mapper)
    {
        _mapper = mapper;
    }

    private static List<CustomerResponse> customers = new List<CustomerResponse>
        {
            new CustomerResponse { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Age = 30, IsBlocked = false },
            new CustomerResponse { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Age = 25, IsBlocked = true },
        };
    public override async Task GetAll(CustomerGetAllRequest request,
        IServerStreamWriter<CustomerResponse> responseStream, ServerCallContext context)
    {

        foreach (var customer in customers.Where(x=>x.IsBlocked != true).ToList())
            await responseStream.WriteAsync(customer);
    }
    public override async Task<CustomerResponse> Get(CustomerRequest request, ServerCallContext context)
    {
        var response = new CustomerResponse();
        if (request.Id > customers.Count) throw new Exception("No such customer in database");
        

        response = customers.Take(request.Id).FirstOrDefault();
        if(response.IsBlocked) throw new Exception("User was blocked");
        return await Task.FromResult(response);
    }
    public override async Task<CustomerResponse> Create(CustomerDTORequest request, ServerCallContext context)
    {
        var createdCustomer = _mapper.Map<CustomerResponse>(request);
        createdCustomer.Id = customers.Count + 1;
        customers.Add(createdCustomer);

        return await Task.FromResult(createdCustomer);
    }

    public override async Task<CustomerResponse> Update(CustomerResponse request, ServerCallContext context)
    {
        var customer = customers.FirstOrDefault(x => x.Id == request.Id)?? throw new Exception("No user with such id");
        if(customer.IsBlocked) throw new Exception("User was blocked");
        var updatedCustomer = _mapper.Map<CustomerResponse>(request);
        customer = updatedCustomer;
        return await Task.FromResult(new CustomerResponse());
        
    }

    public override async Task<CustomerResponse> Block(CustomerRequest request, ServerCallContext context)
    {
        var customer = customers.FirstOrDefault(x => x.Id == request.Id) ?? throw new Exception("No user with such id");
        if (customer.IsBlocked) throw new Exception("User already was blocked");

        customer.IsBlocked = true;

        return await Task.FromResult(customer);
    }

}
