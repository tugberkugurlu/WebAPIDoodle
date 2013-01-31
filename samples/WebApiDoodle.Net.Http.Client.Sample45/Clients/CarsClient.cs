using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using WebApiDoodle.Net.Http.Client;
using WebApiDoodle.Net.Http.Client.Model;
using WebApiDoodle.Net.Http.Client.Sample45.Models;
using WebApiDoodle.Net.Http.Client.Sample45.RequestCommands;

namespace WebApiDoodle.Net.Http.Client.Sample45.Clients {

    public class CarsClient : HttpApiClient<Car, int>, ICarsClient {

        // TODO: Handle exceptions & unsuccessful responses.

        private const string BaseUriTemplate = "api/cars";
        public CarsClient(HttpClient httpClient) 
            : base(httpClient) {
        }

        public async Task<PaginatedDto<Car>> GetCars(PaginatedRequestCommand paginationCmd) {

            var apiResponse = await base.GetAsync(BaseUriTemplate, paginationCmd);
            return apiResponse.Model;
        }

        public async Task<Car> GetCar(int carId) {

            var apiResponse = await base.GetSingleAsync(string.Concat(BaseUriTemplate, "/{id}"), carId);
            return apiResponse.Model;
        }

        public async Task<Car> AddCar(Car car) {

            var apiResponse = await base.PostAsync(BaseUriTemplate, car);
            return apiResponse.Model;
        }

        public async Task<Car> UpdateCar(int carId, Car car) {

            var apiResponse = await base.PutAsync(string.Concat(BaseUriTemplate, "/{id}"), carId, car);
            return apiResponse.Model;
        }

        public async Task RemoveCar(int carId) {

            await base.DeleteAsync(string.Concat(BaseUriTemplate, "/{id}"), carId);
        }
    }
}