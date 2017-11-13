﻿using System.Threading.Tasks;
using Common;
using Entity;

namespace DataSync
{
    public class DataProcesserForContract : IDataProcesser<Contract>
    {
        private static DataProcesserForContract _instance;
        private static readonly object Locker = new object();

        private DataProcesserForContract()
        {
        }

        public static DataProcesserForContract Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new DataProcesserForContract();
                    }
                return _instance;
            }
        }

        public async Task<bool> Save(Contract entity)
        {
            var response = await WebApi.Post("api/TMSYX/BaseSync/SaveContract", entity);
            return Utility.IsResponseSuccess(response);
        }
    }
}