/**
 * Author: Kray Nguyen
 * CSS436
 * Program 4
 * a full-stack program utilizing asp.net web framework to support load, delete, and query operations. The operations interacted with cloud 
 * services such as Azure blob and cosmosdb and return a response to users. More details in the design document
 */ 
using System;
using System.Net;
using Azure.Storage.Blobs.Models;
using System.Collections;
using System.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.WindowsAzure.Storage;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace namesquery{
    public partial class _Default : Page {
        // cosmos + blob config values
        private static readonly string inputUrl = @"https://css490.blob.core.windows.net/lab4/input.txt";
        private static readonly string dbName = "namequeries";
        private static readonly string dbContainer = "nqcontainer";
        private static readonly string dbURI = "https://namequeries.documents.azure.com:443/";
        private static readonly string dbKey = "4xVRc1Aveu6AwR9EmSsYQ8z9g484CbdaIJEo48kTBlzt1f9721QtRqIGlRSZlrfPOPFC5dRdsTFgnbPbVRwzAw==";
        private static readonly string blobConnection = "DefaultEndpointsProtocol=https;AccountName=namequerystorage;AccountKey=kz2LA1838OOr8aKbGDE/xeJsUBFJLQ/3gZfzF2/QPFb81nG3AO6bY4ha0aHQyASpLTQTUHa1npxEHU7efzg+ZA==;EndpointSuffix=core.windows.net";
        private static readonly string storageContainer = "nqcontainerblob";
        private static readonly string storageName = "test.txt";
        private static ArrayList data_list = new ArrayList();
        private static bool clickLoad, clickDelete, clickQuery = false;
        
        // perform first name last name query upon user input. Try creating sql statement based on input to see if statement is valid then 
        // attempt to connect and retrieve from cosmos database 
        private async Task Query(){
            // config database connection if sql retrival is valid
            string LastName = LastNameField.Value.TrimEnd().ToLower();
            string FirstName = FirstNameField.Value.TrimEnd().ToLower();
            bool inputValid= false;
            bool inputFound = false;
            string SQL = "SELECT * FROM " + dbContainer + " WHERE " + dbContainer + ".lastname = ";
            if (LastName.Length != 0 && FirstName.Length != 0){
                SQL += "'" + LastName + "'" + " AND " + dbContainer + ".firstname = " + "'" + FirstName + "'";
            } else if (LastName.Length != 0){
                SQL += "'" + LastName + "'";
            } else if (FirstName.Length != 0){
                SQL = "SELECT * FROM " + dbContainer + " WHERE " + dbContainer + ".firstname = " + "'" + FirstName + "'";
            } else {
                inputValid = true;
            }
            try {
                if (inputValid){
                    QueryMessage.InnerText = "inputs not found";
                    return;
                } else {
                    QueryDefinition qd = new QueryDefinition(SQL);
                    CosmosClient cosmosClient = new CosmosClient(dbURI, dbKey);
                    Database cosmosDB = await cosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
                    Container cosmosContainer = cosmosDB.GetContainer(dbContainer);
             
                    var iterator = cosmosContainer.GetItemQueryIterator<dynamic>(qd);
                    var itIterate = await iterator.ReadNextAsync();

                    foreach (var it in itIterate){
                        string temp_str = it.id;
                        string[] tempArr = temp_str.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tempArr.Length != 0){
                            TextArea2.InnerText += tempArr[1] + " " + tempArr[0] + " " + it.attributes + "\n";
                            inputFound = true;
                        }
                    }
                }
                if (!inputFound){
                    QueryMessage.InnerText = "User input not found";
                } else {
                    QueryMessage.InnerText = "User input found";
                }
            }
            catch (CosmosException){
                QueryErrorMessage.InnerText = "ComosException when trying to query data";
            }
        }
        
        // handles event when user click delete button
        protected void deleteButton(object sender, EventArgs e){
            clickDelete = true;
            Page_Load(sender, e);
        }

        // handles event when user click load button
        protected void loadButton(object sender, EventArgs e){
            clickLoad = true;
            Page_Load(sender, e);
        }

        // handles event when user click query
        protected void queryButton(object sender, EventArgs e){
            clickQuery = true;
            Page_Load(sender, e);
        }

        // parse string for inputing and loading data from cosmosdb
        private string ParseString(string url){
            var textDownloads = (new WebClient()).DownloadString(url);
            string eachLine;
            string finalString = "";
            
            using (var strg = new StringReader(textDownloads))
            {
                
                while ((eachLine = strg.ReadLine()) != null)
                {
                    finalString += eachLine + "&";
                }
            }
            return finalString;
        }

        // helper function to upload data from data url to blob storage
        private async Task uploadData(){
            try{
                string dataText = "";
                var blobAccount = CloudStorageAccount.Parse(blobConnection);
                var blobClient = blobAccount.CreateCloudBlobClient();
                var blobContainer = blobClient.GetContainerReference(storageContainer);
                var blobReference = blobContainer.GetBlockBlobReference(storageName);
                blobReference.Properties.ContentType = "text/plain";
                foreach (string it in data_list){
                    dataText += it + "\n";
                }
                await blobReference.UploadTextAsync(dataText);
            }
            catch (Exception){
                LoadErrorMessage.InnerText = "Exception when uploading data";
            }
        }
        private async Task DeleteData(){
            try{
                CosmosClient CosmosClient = new CosmosClient(dbURI, dbKey);
                Database CosmosDB = await CosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
                Container CosmosContainer = CosmosDB.GetContainer(dbContainer);
                ContainerResponse response = await CosmosContainer.DeleteContainerAsync();
                data_list.Clear();
                await uploadData();
                DeleteMessage.InnerText = "data cleared success";
            }
            catch (CosmosException){
                DeleteErrorMessage.InnerText = "cosmosdb exception while deleting data";
            }
        }
        
        // upload the data to CosmosDB providing text data, cosmos URI and key. Data has formating of last name-first name follow
        // by various attributes
        private async Task UploadData(string data){
            string key = "/lastname";
            CosmosClient CosmosClient = new CosmosClient(dbURI, dbKey);
            Database CosmosDb = await CosmosClient.CreateDatabaseIfNotExistsAsync(dbName);
            Container CosmosContainer = await CosmosDb.CreateContainerIfNotExistsAsync(dbContainer, key, 400);
            bool upload_success = false;
            try{
                if (data.Length == 0){throw new IOException();}
                string[] strArray = data.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var it in strArray){
                    try{
                    string[] finalArr = it.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (finalArr.Length < 2){throw new IndexOutOfRangeException();}
                    
                    string FirstName = finalArr[1];
                    string LastName = finalArr[0];
                    string attributes = "";

                    for (var i = 2; i < finalArr.Length; i++){
                        if (i == finalArr.Length - 1){
                            attributes += finalArr[i];
                            continue;
                        }
                        attributes += finalArr[i] + " ";
                    }
                        dynamic testItem = new 
                        { id = FirstName + " " + LastName, 
                            lastname = LastName.ToLower(), 
                            firstname = FirstName.ToLower(),
                            attributes = attributes };
                        var response = await CosmosContainer.CreateItemAsync(testItem);
                        string content = LastName + " " + FirstName + " " + attributes;
                        TextArea1.InnerText += content + "\n";
                        data_list.Add(content);
                        upload_success = true;
                    }
                    catch (CosmosException)
                    {
                        LoadErrorMessage.InnerText = "clear data before loading duplicated data";
                        continue;
                    }
                }
            }
            catch (IOException){
                LoadErrorMessage.InnerText = "data loading exception";
                return;
            }
            if (upload_success){
                LoadMessage.InnerText = "success load";
                await uploadData(); 
            }
        }

        // utilize async programming to refresh page after one of the operations (Load, Clear, Query) is processed
        protected async void Page_Load(object sender, EventArgs e)
        {
            // front end messages
            TextArea1.InnerText = " "; TextArea2.InnerText = "";
            DeleteMessage.InnerText = ""; DeleteErrorMessage.InnerText = "";
            QueryMessage.InnerText = ""; QueryErrorMessage.InnerText = "";
            LoadMessage.InnerText = ""; LoadErrorMessage.InnerText = "";

            // delete data from cosmosdb container
            if (clickDelete) {
                var deleteTask = DeleteData();
                await Task.WhenAll(deleteTask);
                clickDelete = false;
            }
            // parse text then upload to cosmosdb
            if (clickLoad){
                string parseData = ParseString(inputUrl);
                var loadTask = UploadData(parseData);
                await Task.WhenAll(loadTask);
                clickLoad = false;
            }
            if (clickQuery){
                var queryTask = Query();
                await Task.WhenAll(queryTask);
                clickQuery = false;
            }
        }
    }
}