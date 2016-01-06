using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faker;
using Newtonsoft.Json;
using PCLStorage;
using Xamarin.Forms;

namespace Customers
{
    public class CustomerDataSource : IDataSource<Customer>
    {
        public CustomerDataSource(bool simulateNetworkLatency = true)
        {
            if (!simulateNetworkLatency)
                _LatencyTimeSpan = TimeSpan.Zero;

            _RootFolder = FileSystem.Current.LocalStorage;
        }

        const string _FileName = "customers.json";

        IFolder _RootFolder;

        TimeSpan _LatencyTimeSpan = TimeSpan.FromSeconds(1.5);

        bool _IsInitialized;

        List<Customer> _Customers;

        async Task Initialize()
        {
            if (!(await FileExists(_RootFolder, _FileName)))
            {
                await CreateFile(_RootFolder, _FileName);
            }

            if (String.IsNullOrWhiteSpace((await GetFileContents(await GetFile(_RootFolder, _FileName)))))
            {
                _Customers = GenerateCustomers();

                await WriteFile(_RootFolder, _FileName, JsonConvert.SerializeObject(_Customers));
            }
            else
            {
                _Customers = JsonConvert.DeserializeObject<List<Customer>>((await GetFileContents(await GetFile(_RootFolder, _FileName))));    
            }

            _IsInitialized = true;
        }

        public const int CustomerCount = 300;

        static List<Customer> GenerateCustomers(int count = CustomerCount)
        {
            var customers = new List<Customer>();

            for (int i = 0; i < count; i++)
            {
                var firstName = Name.First();
                var lastName = Name.Last();
                var phone = Phone.CellNumber().Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace(" ", "").Substring(0, 10);

                customers.Add(new Customer()
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = firstName,
                        LastName = lastName,
                        Email = Internet.Email(String.Format($"{firstName} {lastName}")),
                        Company = Company.Name(),
                        Department = Company.CatchPhrase().ConvertToTitleCase(),
                        Phone = phone,
                        Street = Address.StreetAddress(),
                        Unit = Address.SecondaryAddress(),
                        City = Address.City(),
                        State = Address.StateAbbreviation(),
                        PostalCode = Address.ZipCode(),
                        Country = "USA",
                        ImageUrl = Avatar.Image(),
                        SmallPhotoUrl = Avatar.Image(null, "150x150")
                    });
            }

            return customers;
        }

        static async Task<bool> FileExists(IFolder folder, string fileName)
        {
            return await Task.FromResult<bool>((await folder.CheckExistsAsync(fileName)) == ExistenceCheckResult.FileExists);
        }

        static async Task<IFile> CreateFile(IFolder folder, string fileName)
        {
            return await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
        }

        static async Task<IFile> GetFile(IFolder folder, string fileName)
        {
            return await folder.GetFileAsync(fileName);
        }

        static async Task WriteFile(IFolder folder, string fileName, string fileContents)
        {
            var file = await GetFile(folder, fileName);

            await file.WriteAllTextAsync(fileContents);
        }

        static async Task<string> GetFileContents(IFile file)
        {
            return await file.ReadAllTextAsync();
        }

        Task Latency
        {
            get
            {
                var random = new Random();
                var ms = random.Next((int)_LatencyTimeSpan.TotalMilliseconds);
                return Task.Delay(ms);
            }
        }

        #region IDataSource implementation

        public async Task SaveItem(Customer item)
        {
            if (_Customers.Any(c => c.Id == item.Id))
            {
                var existingCustomer = _Customers.Single(x => x.Id == item.Id);

                var index = _Customers.IndexOf(existingCustomer);

                _Customers[index] = item;
            }
            else
            {
                _Customers.Add(item);

                await WriteFile(_RootFolder, _FileName, JsonConvert.SerializeObject(_Customers));
            }
        }

        public async Task DeleteItem(string id)
        {
            var existingCustomer = _Customers.SingleOrDefault(x => x.Id == id);

            if (existingCustomer != null)
            {
                _Customers.Remove(existingCustomer);

                await WriteFile(_RootFolder, _FileName, JsonConvert.SerializeObject(_Customers));
            }
        }

        public async Task<Customer> GetItem(string id)
        {
            return _Customers.SingleOrDefault(x => x.Id == id);
        }

        public async Task<ICollection<Customer>> GetItems(int start = 0, int count = 100, string query = "")
        {
            if (!_IsInitialized)
                await Initialize();

            await Latency;

            return 
                CustomerDataSourceHelper
                .BasicQueryFilter(_Customers, query)
                .Skip(start)
                .Take(count)
                .ToList();
        }

        #endregion
    }

    public static class CustomerDataSourceHelper
    {
        static int MatchScore(Customer c, string query)
        {
            return new[]
            {
                $"{c.FirstName} {c.LastName}",
                c.Email,
                c.Company,
            }.Sum(label => MatchScore(label, query));
        }

        static int MatchScore(string data, string query)
        {
            int score = query.Length;

            if (string.IsNullOrEmpty(data))
                return 0;

            data = data.ToLower();
            if (!data.Contains(query))
                return 0;

            if (data == query)
                score += 2;
            else if (data.StartsWith(query))
                score++;

            return score;
        }

        public static IEnumerable<Customer> BasicQueryFilter(IEnumerable<Customer> source, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return source.OrderBy(e => e.LastName ?? "");
            }

            query = query.ToLower();
            return source
                .Select(c => Tuple.Create(MatchScore(c, query), c))
                .Where(c => c.Item1 != 0)
                .OrderByDescending(e => e.Item1)
                .Select(c => c.Item2);
        }
    }

    public static class StringExtensions
    {
        public static string ConvertToTitleCase(this string input)
        {
            return DependencyService.Get<ILocalization>().ToTitleCase(input);
        }
    }
}

