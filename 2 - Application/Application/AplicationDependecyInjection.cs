using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddApplicationDependency(this IServiceCollection services)
        {
            // Obtém todos os assemblies carregados no domínio atual da aplicação
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Itera sobre cada assembly para buscar tipos exportados
            foreach (var assembly in assemblies)
            {
                // Verifica se o nome do assembly contém "Application"
                if (assembly.FullName.Contains("Application"))
                {
                    // Busca todos os tipos que possuem "Handler" no nome e que sejam classes não abstratas
                    var handlers = assembly.ExportedTypes
                        .Where(type => type.Name.Contains("Handler") && type.IsClass && !type.IsAbstract)
                        .ToList();

                    // Para cada handler encontrado, realiza o registro
                    handlers.ForEach(handler =>
                    {
                        // Tenta encontrar a interface correspondente ao handler (ex.: IMyHandler para MyHandler)
                        var interfaceType = handler.GetInterfaces().FirstOrDefault(i => i.Name == $"I{handler.Name}");

                        if (interfaceType != null)
                        {
                            // Registra a interface como serviço e a classe como implementação
                            services.TryAddScoped(interfaceType, handler);
                        }
                        else
                        {
                            // Se não encontrar uma interface, registra a classe para ela mesma
                            services.TryAddScoped(handler, handler);
                        }
                    });
                }
            }

            // Adiciona automaticamente o AutoMapper com todos os assemblies encontrados
            services.AddAutoMapper(assemblies);

            return services;
        }
    }
}
