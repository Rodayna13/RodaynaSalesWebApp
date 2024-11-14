using Autofac;
using Autofac.Integration.Mvc;
using Core.Interfaces;
using Core.UseCases;
using Infrastructure.Data;
using Infrastructure.Repositories;
using System.Web.Mvc;

namespace SalesWebApp.App_Start
{
    public class AutofacConfig
    {
        public static void Configure()
        {
            var builder = new ContainerBuilder();

            // Register AppDbContext
            builder.RegisterType<AppDbContext>().InstancePerRequest();


            // Register services and repositories

            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerRequest();

            builder.RegisterType<CategoryService>().AsSelf();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>();

            builder.RegisterType<ItemService>().AsSelf();
            builder.RegisterType<ItemRepository>().As<IItemRepository>();

            builder.RegisterType<CustomerService>().AsSelf();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();

            builder.RegisterType<InvoiceService>().AsSelf();
            builder.RegisterType<InvoiceRepository>().As<IInvoiceRepository>();

            builder.RegisterType<InvoiceItemService>().AsSelf();
            builder.RegisterType<InvoiceItemRepository>().As<IInvoiceItemRepository>();


            // Register UseCases
            builder.RegisterType<LoginUseCase>().InstancePerRequest();
            builder.RegisterType<RegisterUseCase>().InstancePerRequest();


            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Build the Autofac container
            var container = builder.Build();

            // Set the MVC dependency resolver to use Autofac
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}