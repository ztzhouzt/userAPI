using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class ContactContext
    {
        private IMongoDatabase _database;
        private IMongoCollection<Contactbook> _collection;
        private AppSettings _appSettings;

        public ContactContext(IOptionsSnapshot<AppSettings> settings)
        {
            _appSettings = settings.Value;
            var client = new MongoClient(_appSettings.MongoContactConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(_appSettings.MongoContactDatabase);
            }
        }

        private void CheckAndCreateCollection(string _collectionName)
        {
            var collectionList = _database.ListCollections().ToList();
            var collectionNames = new List<string>();

            collectionList.ForEach(b => collectionNames.Add(b["name"].AsString));
            if (!collectionNames.Contains(_collectionName))
            {
                _database.CreateCollection(_collectionName);
            }
        }

        /// <summary>
        /// 用户通讯录
        /// </summary>
        public IMongoCollection<Contactbook> ContactBooks
        {
            get
            {
                CheckAndCreateCollection("ContactBooks");
                return _database.GetCollection<Contactbook>("ContactBooks");
            }
        }

        /// <summary>
        /// 好友申请列表
        /// </summary>
        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get
            {
                CheckAndCreateCollection("ContactApplyRequest");
                return _database.GetCollection<ContactApplyRequest>("ContactApplyRequest");
            }
        }

    }
}
