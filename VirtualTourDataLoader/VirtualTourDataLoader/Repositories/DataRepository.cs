using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using VirtualTourApi.Interfaces;

namespace VirtualTourApi.Repositories
{
    public class DataRepository<TModel>
        : IDataRepository<TModel>
        where TModel : class
    {
        readonly IDocumentClient documentClient;
        public string DatabaseId { get; private set; }
        public string CollectionId { get; private set; }

        public DataRepository(Uri endpointUri, string key)
        {
            documentClient = new DocumentClient(endpointUri, key);
        }

        public async Task InitializeDatabaseAsync(string databaseId, string collectionId)
        {
            DatabaseId = databaseId;
            CollectionId = collectionId;
            await CreateDatabaseIfNotExistsAsync(databaseId);
            await CreateCollectionIfNotExistsAsync(databaseId, collectionId);
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

        private async Task CreateDatabaseIfNotExistsAsync(string databaseId)
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync(string databaseId, string collectionId)
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(databaseId), new DocumentCollection { Id = collectionId }, new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
