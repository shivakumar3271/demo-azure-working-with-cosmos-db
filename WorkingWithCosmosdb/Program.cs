using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace WorkingWithCosmosdb
{
    class Program
    {
        private const string EndpointUrl = "insert_your_url_endpoint";
        private const string PrimaryKey = "insert_your_primary_key";
        private DocumentClient client;

        static void Main(string[] args)
        {
			Console.WriteLine("Demo working with Azure Cosmos DB");
			Console.WriteLine("by Avinash Seth <avinash.seth@outlook.com");
            try
            {
                Program p = new Program();
                p.GetStartedDemo().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("We are done");
                Console.ReadKey();
            }
        }
        private async Task GetStartedDemo()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "StudentDB" });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("StudentDB"), new DocumentCollection { Id = "StudentCollection" });
            Students avinash = new Students
            {
                Name = "Avinash",
                Gender = "Male",
                Id = "1"
            };
            await this.CreateStudentDocumentIfNotExists("StudentDB", "StudentCollection", avinash);
        }
        private async Task CreateStudentDocumentIfNotExists(string databaseName, string collectionName, Students student)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, student.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), student);
                }
                else
                {
                    throw;
                }
            }
        }
    }
    public class Students
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Id { get; set; }
    }
}
