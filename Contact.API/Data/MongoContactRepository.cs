using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dtos;
using Contact.API.Models;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class MongoContactRepository : IContactRepository
    {
        private readonly ContactContext _contactContext;
        public MongoContactRepository(ContactContext contactContent)
        {
            _contactContext = contactContent;
        }

        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task<bool> UpdateContactInfoAsync(BaseUserInfo userInfo, CancellationToken cancellationToken)
        {
            var contactBook = (await _contactContext.ContactBooks.FindAsync(c => c.UserId == userInfo.UserId, null, cancellationToken))
                .FirstOrDefault();

            if (contactBook == null)
            {
                return true;
            }
            var contactIds = contactBook.Contacts.Select(c => c.UserId);

            var filter = Builders<Contactbook>.Filter.And(
                 Builders<Contactbook>.Filter.In(c => c.UserId, contactIds),
                 Builders<Contactbook>.Filter.ElemMatch(c => c.Contacts, contact => contact.UserId == userInfo.UserId)
                );

            var update = Builders<Contactbook>.Update
                .Set("Contacts.&.Name", userInfo.Name)
                .Set("Contacts.&.Avatar", userInfo.Avatar)
                .Set("Contacts.&.Company", userInfo.Company)
                .Set("Contacts.&.Title", userInfo.Title);

            var UpdateResult = _contactContext.ContactBooks.UpdateMany(filter, update);

            return UpdateResult.MatchedCount == UpdateResult.ModifiedCount;
        }


        /// <summary>
        /// 添加联系人信息
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AddContactAsync(int userId, BaseUserInfo contact, CancellationToken cancellationToken)
        {
            if (_contactContext.ContactBooks.Count(c => c.UserId == userId) == 0)
            {
                await _contactContext.ContactBooks.InsertOneAsync(new Contactbook { UserId = userId });
            }


            var filter = Builders<Contactbook>.Filter.Eq(c => c.UserId, userId);

            var update = Builders<Contactbook>.Update.AddToSet(c => c.Contacts, new Models.Contact
            {
                UserId = contact.UserId,
                Name = contact.Name,
                Company = contact.Company,
                Avatar = contact.Avatar,
                Title = contact.Title
            });

            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
        }

        public async Task<List<Models.Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken)
        {
            var contactBook = (await _contactContext.ContactBooks.FindAsync(c => c.UserId == userId)).FirstOrDefault();
            if (contactBook != null)
            {
                return contactBook.Contacts;
            }
            else
            {
                return new List<Models.Contact>();
            }
        }

        /// <summary>
        /// 给好友标签
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="tags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> TagContactAsync(int userId,int contactId,List<string> tags, CancellationToken cancellationToken)
        {
            var filter = Builders<Contactbook>.Filter.And(
             Builders<Contactbook>.Filter.Eq(c => c.UserId, userId),
             Builders<Contactbook>.Filter.Eq("Contacts.UserId",contactId)
            );

            var update = Builders<Contactbook>.Update
                 .Set("Contacts.$.Tags", tags);

            var result = await _contactContext.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;
        }
    }
}
