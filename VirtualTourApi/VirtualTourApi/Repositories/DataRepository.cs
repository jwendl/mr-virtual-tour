using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VirtualTourApi.Interfaces;
using VirtualTourApi.Models;

namespace VirtualTourApi.Repositories
{
    public class DataRepository<TModel>
        : IDataRepository<TModel>
        where TModel : class
    {
        readonly IDocumentClient documentClient;
        public string DatabaseId { get; private set; }
        public string CollectionId { get; private set; }

        public DataRepository(IOptions<DocumentSettings> options)
        {
            var settings = options.Value;
            DatabaseId = settings.DatabaseId;
            CollectionId = typeof(TModel).Name;
            documentClient = new DocumentClient(settings.DocumentEndpoint, settings.DocumentKey);
        }

        public async Task<TModel> FetchItemAsync(string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
            var document = await documentClient.ReadDocumentAsync(documentUri);
            return document as TModel;
        }

        public async Task<IEnumerable<TModel>> FetchItemsAsync()
        {
            var documentQuery = documentClient.CreateDocumentQuery<TModel>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), new FeedOptions { MaxItemCount = -1 })
                .AsDocumentQuery();

            var results = new List<TModel>();
            while (documentQuery.HasMoreResults)
            {
                results.AddRange(await documentQuery.ExecuteNextAsync<TModel>());
            }

            return results;
        }

        public async Task<IEnumerable<TModel>> FindItemsAsync(Expression<Func<TModel, bool>> predicate)
        {
            var documentQuery = documentClient.CreateDocumentQuery<TModel>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<TModel>();
            while (documentQuery.HasMoreResults)
            {
                results.AddRange(await documentQuery.ExecuteNextAsync<TModel>());
            }

            return results;
        }

        public async Task<TModel> CreateItemAsync(TModel item)
        {
            var documentUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var model = await documentClient.CreateDocumentAsync(documentUri, item);
            return model as TModel;
        }

        public async Task<TModel> CreateItemIfNotExistsAsync(TModel item)
        {
            var documentUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var model = await documentClient.UpsertDocumentAsync(documentUri, item);
            return model as TModel;
        }

        public async Task<TModel> UpdateItemAsync(string id, TModel item)
        {
            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
            var model = await documentClient.ReplaceDocumentAsync(documentUri, item);
            return model as TModel;
        }

        public async Task DeleteItemAsync(string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
            await documentClient.DeleteDocumentAsync(documentUri);
        }
    }
}
