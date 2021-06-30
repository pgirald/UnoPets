using MiUnoApp.Services.Interface;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.ObjectModel;
using MiUnoApp.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace MiUnoApp.Services
{
    class HttpConsumer
    {

        private const string Root = "https://unoapi20210630150833.azurewebsites.net/api/";//"https://localhost:5001/api/";

        private JsonSerializerSettings settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

        private HttpClient CreateClient(bool addToken = true)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler
                .DangerousAcceptAnyServerCertificateValidator;
            HttpClient client=new HttpClient(handler);
            if (addToken)
            {
                if (CurrentTokenSingleton.CurrentToken == null)
                {
                    throw new InvalidOperationException("The application does not have defined a current token");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", CurrentTokenSingleton.CurrentToken.Token);
            }
            return client;
        }

        private StringContent CreateJsonContent(object obj)
        {
            string json = JsonConvert
                       .SerializeObject(obj, settings);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }


        public async Task<ResponseToken> LoginAsync(string email, string password)
        {
            using (var client = CreateClient(false))
            {
                StringContent content = CreateJsonContent(new Credentials() { email = email, password = password });
                HttpResponseMessage response = await client
                    .PostAsync(new Uri(Root + "Users/Login"), content);
                response.EnsureSuccessStatusCode();
                return JsonConvert
                    .DeserializeObject<ResponseToken>(await response.Content.ReadAsStringAsync());
            }

        }

        public async Task<bool> LogOutAsync()
        {
            using(var client = CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync(Root + "Users/LogOut");
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<ResponseToken> RegisterUserAsync(RegisterData data)
        {
            using (var client = CreateClient(false))
            {
                StringContent content = CreateJsonContent(data);
                HttpResponseMessage response = await client.PostAsync(Root + "Users", content);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert
                        .DeserializeObject<ResponseToken>(await response.Content.ReadAsStringAsync());
                }
                return null;
            }
        }

        public async Task<List<Pet>> GetPetsAsync(User currentUser)
        {
            using (var client = CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync(Root + "Pets/Customer/" + currentUser.Id);
                return JsonConvert
                    .DeserializeObject<List<Pet>>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<List<Specie>> GetSpeciesAsync()
        {
            using (var client = CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync(Root + "Pets/Species");
                return JsonConvert.DeserializeObject<List<Specie>>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<Pet> AddPetAsync(Pet pet)
        {
            using (var client = CreateClient())
            {
                StringContent content = CreateJsonContent(pet);
                HttpResponseMessage response = await client.PostAsync(Root + "Pets", content);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<Pet>(await response.Content.ReadAsStringAsync());
                }
                return null;
            }
        }

        public async Task<bool> DeletePetAsync(int petId)
        {
            using (var client = CreateClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(Root + "Pets/" + petId);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<Pet> UpdatePetAsync(Pet pet)
        {
            using (var client = CreateClient())
            {
                StringContent content = CreateJsonContent(pet);
                HttpResponseMessage response = await client.PutAsync(Root + "Pets/" + pet.Id, content);
                if (response.IsSuccessStatusCode)
                {
                    return pet;
                }
                return null;
            }
        }

        public async Task<User> UpdateUserAsync(UserData data)
        {
            using (var client = CreateClient())
            {
                StringContent content = CreateJsonContent(data);
                HttpResponseMessage response = await client.PutAsync(Root + "Users", content);
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
                }
                return null;
            }
        }
    }
}
