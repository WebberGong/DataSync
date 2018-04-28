using Common;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSync
{
    public class Relationships
    {
        public static List<Contract> Contracts => DataSynchronizer<Contract>.SourceEntities;

        public static List<Customer> Customers => DataSynchronizer<Customer>.SourceEntities;

        public static List<Seller> Sellers => DataSynchronizer<Seller>.SourceEntities;

        public static List<Carrier> Carriers => DataSynchronizer<Carrier>.SourceEntities;

        public static List<Truck> Trucks => DataSynchronizer<Truck>.SourceEntities;

        public static void AddCooperationCorp()
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var customer in Customers)
                {
                    var obj = new
                    {
                        mTaxNumber = customer.TaxNumber,
                        mCorpName = customer.Name,
                        sTaxNumbers = new List<string> { customer.TaxNumber },
                        sCorpNames = new List<string> { customer.Name }
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj));
                }
                foreach (var seller in Sellers)
                {
                    var obj = new
                    {
                        mTaxNumber = seller.TaxNumber,
                        mCorpName = seller.Name,
                        sTaxNumbers = new List<string> { seller.TaxNumber },
                        sCorpNames = new List<string> { seller.Name }
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj));
                }
                foreach (var carrier in Carriers)
                {
                    var obj = new
                    {
                        mTaxNumber = carrier.TaxNumber,
                        mCorpName = carrier.Name,
                        sTaxNumbers = new List<string> { carrier.TaxNumber },
                        sCorpNames = new List<string> { carrier.Name }
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj));
                }
                Task.WaitAll(tasks.ToArray());

                tasks = new List<Task>();
                foreach (var customer in Customers)
                {
                    var sellerTaxNumbers = Sellers.Select(x => x.TaxNumber).ToList();
                    var carrierTaxNumbers = Carriers.Select(x => x.TaxNumber).ToList();
                    var taxNumbers = new List<string>();
                    taxNumbers.AddRange(sellerTaxNumbers);
                    taxNumbers.AddRange(carrierTaxNumbers);

                    var sellerCorpNames = Sellers.Select(x => x.Name).ToList();
                    var carrierCorpNames = Carriers.Select(x => x.Name).ToList();
                    var corpNames = new List<string>();
                    corpNames.AddRange(sellerCorpNames);
                    corpNames.AddRange(carrierCorpNames);

                    var obj = new
                    {
                        mTaxNumber = customer.TaxNumber,
                        mCorpName = customer.Name,
                        sTaxNumbers = taxNumbers,
                        sCorpNames = corpNames
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj));
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        public static async Task AddCooperationCorpForCustomer(Customer customer)
        {
            try
            {
                var tasks = new List<Task>();
                var obj1 = new
                {
                    mTaxNumber = customer.TaxNumber,
                    mCorpName = customer.Name,
                    sTaxNumbers = new List<string> { customer.TaxNumber },
                    sCorpNames = new List<string> { customer.Name }
                };
                tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj1));
                await Task.WhenAll(tasks);

                tasks = new List<Task>();
                var sellerTaxNumbers = Sellers.Select(x => x.TaxNumber).ToList();
                var carrierTaxNumbers = Carriers.Select(x => x.TaxNumber).ToList();
                var taxNumbers = new List<string>();
                taxNumbers.AddRange(sellerTaxNumbers);
                taxNumbers.AddRange(carrierTaxNumbers);

                var sellerCorpNames = Sellers.Select(x => x.Name).ToList();
                var carrierCorpNames = Carriers.Select(x => x.Name).ToList();
                var corpNames = new List<string>();
                corpNames.AddRange(sellerCorpNames);
                corpNames.AddRange(carrierCorpNames);

                var obj2 = new
                {
                    mTaxNumber = customer.TaxNumber,
                    mCorpName = customer.Name,
                    sTaxNumbers = taxNumbers,
                    sCorpNames = corpNames
                };
                tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj2));
                await Task.WhenAll(tasks);
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        public static async Task AddCooperationCorpForSeller(Seller seller)
        {
            try
            {
                var tasks = new List<Task>();
                var obj1 = new
                {
                    mTaxNumber = seller.TaxNumber,
                    mCorpName = seller.Name,
                    sTaxNumbers = new List<string> { seller.TaxNumber },
                    sCorpNames = new List<string> { seller.Name }
                };
                tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj1));
                await Task.WhenAll(tasks);

                tasks = new List<Task>();
                foreach (var customer in Customers)
                {
                    var obj = new
                    {
                        mTaxNumber = customer.TaxNumber,
                        mCorpName = customer.Name,
                        sTaxNumbers = new List<string> { seller.TaxNumber },
                        sCorpNames = new List<string> { seller.Name }
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj));
                }
                await Task.WhenAll(tasks);
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        public static async Task AddCooperationCorpForCarrier(Carrier carrier)
        {
            try
            {
                var tasks = new List<Task>();
                var obj1 = new
                {
                    mTaxNumber = carrier.TaxNumber,
                    mCorpName = carrier.Name,
                    sTaxNumbers = new List<string> { carrier.TaxNumber },
                    sCorpNames = new List<string> { carrier.Name }
                };
                tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj1));
                await Task.WhenAll(tasks);

                tasks = new List<Task>();
                foreach (var customer in Customers)
                {
                    var obj = new
                    {
                        mTaxNumber = customer.TaxNumber,
                        mCorpName = customer.Name,
                        sTaxNumbers = new List<string> { carrier.TaxNumber },
                        sCorpNames = new List<string> { carrier.Name }
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCooperationCorp", obj));
                }
                await Task.WhenAll(tasks);

                tasks = new List<Task>();
                var obj2 = new
                {
                    taxNumber = carrier.TaxNumber,
                    corpName = carrier.Name,
                    truckNumbers = Trucks.Select(x => x.Number)
                };
                tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCorpCars", obj2));
                await Task.WhenAll(tasks);
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        public static async Task AddCorpCars(Truck truck)
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var carrier in Carriers)
                {
                    var obj = new
                    {
                        taxNumber = carrier.TaxNumber,
                        corpName = carrier.Name,
                        truckNumbers = new List<string> { truck.Number }
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCorpCars", obj));
                }
                await Task.WhenAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }

        public static void AddCorpCars()
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var carrier in Carriers)
                {
                    var obj = new
                    {
                        taxNumber = carrier.TaxNumber,
                        corpName = carrier.Name,
                        truckNumbers = Trucks.Select(x => x.Number)
                    };
                    tasks.Add(WebApi.SimplePostAsync("api/TMSYX/BaseSync/AddCorpCars", obj));
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                Utility.LogFatalOrThrowException(ex);
            }
        }
    }
}