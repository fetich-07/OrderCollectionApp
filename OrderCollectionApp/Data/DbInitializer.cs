using OrderCollectionApp.Models;

namespace OrderCollectionApp.Data
{
    public static class DbInitializer
    {
        //Данный класс позволяет заполнить таблицу с Поставщиками
        public static void Initialize(AppDbContext context)
        {
            var providers = new Provider[]
            {
            new Provider{Name="Поставщик 1"},
            new Provider{Name="Поставщик 2"},
            new Provider{Name="Поставщик 3"},
            new Provider{Name="Поставщик 4"},
            new Provider{Name="Поставщик 5"},
            };
            foreach (Provider p in providers)
            {
                if (!context.Providers.Any(pr => pr.Name == p.Name))
                    context.Providers.Add(p);
            }
            context.SaveChanges();
        }
    }
}

