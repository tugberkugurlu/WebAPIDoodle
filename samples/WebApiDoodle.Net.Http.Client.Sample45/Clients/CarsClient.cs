using System.Net.Http;
using System.Threading.Tasks;
using WebApiDoodle.Net.Http.Client.Model;
using WebApiDoodle.Net.Http.Client.Sample45.Models;
using WebApiDoodle.Net.Http.Client.Sample45.RequestCommands;

namespace WebApiDoodle.Net.Http.Client.Sample45.Clients {

    /// <summary>
    /// HTTP API Client for Cars resource.
    /// </summary>
    public class CarsClient : HttpApiClient<Car, int>, ICarsClient {

        private const string BaseUriTemplate = "api/cars";
        private const string HttpRequestErrorFormat = "Response status code does not indicate success: {0} ({1})";

        public CarsClient(HttpClient httpClient) 
            : base(httpClient) {
        }

        /// <summary>
        /// Gets the cars list.
        /// </summary>
        /// <param name="paginationCmd"></param>
        /// <returns></returns>
        /// <exception cref="WebApiDoodle.Net.Http.Client.HttpApiRequestException">
        /// The request has completed with a non-success status code.
        /// </exception>
        public async Task<PaginatedDto<Car>> GetCars(PaginatedRequestCommand paginationCmd) {

            using (var apiResponse = await base.GetAsync(BaseUriTemplate, paginationCmd)) {

                if (apiResponse.IsSuccess) {

                    return apiResponse.Model;
                }

                throw new HttpApiRequestException(
                    string.Format(HttpRequestErrorFormat, (int)apiResponse.Response.StatusCode, apiResponse.Response.ReasonPhrase),
                    apiResponse.Response.StatusCode, apiResponse.HttpError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carId"></param>
        /// <returns></returns>
        /// <exception cref="WebApiDoodle.Net.Http.Client.HttpApiRequestException">
        /// The request has completed with a non-success status code.
        /// </exception>
        public async Task<Car> GetCar(int carId) {

            using (var apiResponse = await base.GetSingleAsync(string.Concat(BaseUriTemplate, "/{id}"), carId)) {

                if (apiResponse.IsSuccess) {

                    return apiResponse.Model;
                }

                throw new HttpApiRequestException(
                    string.Format(HttpRequestErrorFormat, (int)apiResponse.Response.StatusCode, apiResponse.Response.ReasonPhrase),
                    apiResponse.Response.StatusCode, apiResponse.HttpError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        /// <exception cref="WebApiDoodle.Net.Http.Client.HttpApiRequestException">
        /// The request has completed with a non-success status code.
        /// </exception>
        public async Task<Car> AddCar(Car car) {

            using (var apiResponse = await base.PostAsync(BaseUriTemplate, car)) {

                if (apiResponse.IsSuccess) {

                    return apiResponse.Model;
                }

                throw new HttpApiRequestException(
                    string.Format(HttpRequestErrorFormat, (int)apiResponse.Response.StatusCode, apiResponse.Response.ReasonPhrase),
                    apiResponse.Response.StatusCode, apiResponse.HttpError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carId"></param>
        /// <param name="car"></param>
        /// <returns></returns>
        /// <exception cref="WebApiDoodle.Net.Http.Client.HttpApiRequestException">
        /// The request has completed with a non-success status code.
        /// </exception>
        public async Task<Car> UpdateCar(int carId, Car car) {

            using (var apiResponse = await base.PutAsync(string.Concat(BaseUriTemplate, "/{id}"), carId, car)) {

                if (apiResponse.IsSuccess) {

                    return apiResponse.Model;
                }

                throw new HttpApiRequestException(
                    string.Format(HttpRequestErrorFormat, (int)apiResponse.Response.StatusCode, apiResponse.Response.ReasonPhrase),
                    apiResponse.Response.StatusCode, apiResponse.HttpError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="carId"></param>
        /// <returns></returns>
        /// <exception cref="WebApiDoodle.Net.Http.Client.HttpApiRequestException">
        /// The request has completed with a non-success status code.
        /// </exception>
        public async Task RemoveCar(int carId) {

            using (var apiResponse = await base.DeleteAsync(string.Concat(BaseUriTemplate, "/{id}"), carId)) {

                if (!apiResponse.IsSuccess) {

                    throw new HttpApiRequestException(
                        string.Format(HttpRequestErrorFormat, (int)apiResponse.Response.StatusCode, apiResponse.Response.ReasonPhrase),
                        apiResponse.Response.StatusCode, apiResponse.HttpError);
                }
            }
        }
    }
}