using Demo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddMutationConventions(new MutationConventionOptions { ApplyToAllMutations = true });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGraphQL();

app.Run();

public partial class Program
{
}