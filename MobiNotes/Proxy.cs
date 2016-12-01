using MobiNotes.DataModel;
using PortableRest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using System.Threading.Tasks;

namespace MobiNotes
{
    public class Proxy
    {
        private const string URL = "http://apinotes.azurewebsites.net/";

         public static async Task<Note> PostNote(Note note)
         {
             var client = new RestClient { BaseUrl = URL };
             RestRequest request = new RestRequest("notes/", HttpMethod.Post);
             request.ContentType = ContentTypes.Json;
             request.AddParameter(note);
             var response = await client.ExecuteAsync<Note>(request);

             return response;
         }
         public static List<Note> GetNotes()
         {
             var client = new RestClient { BaseUrl = URL };
        //     RestRequest request = new RestRequest("notes/", HttpMethod.Get);
            RestRequest request = new RestRequest("notes/?user=092149-091459-091472", HttpMethod.Get);
            var response = client.ExecuteAsync<List<Note>>(request);
             return response.Result;
         }
     }
    }
