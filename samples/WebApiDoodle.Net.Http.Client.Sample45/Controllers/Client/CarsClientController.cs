using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiDoodle.Net.Http.Client.Model;
using WebApiDoodle.Net.Http.Client.Sample45.Clients;
using WebApiDoodle.Net.Http.Client.Sample45.Filters;
using WebApiDoodle.Net.Http.Client.Sample45.Models;
using WebApiDoodle.Net.Http.Client.Sample45.RequestCommands;

namespace WebApiDoodle.Net.Http.Client.Sample45.Controllers.Client {

    /// <summary>
    /// This is the client controller. This should have been an ASP.NET MVC controller,
    /// a WPF application or any other .NET application. It just doesn't matter.
    /// We used a Web API controller here to act as a client.
    /// </summary>
    [InvalidModelStateFilter]
    public class CarsClientController : ApiController {

        private readonly ICarsClient _carsClient;
        public CarsClientController(ICarsClient carsClient) {

            _carsClient = carsClient;
        }

        // GET /carsclient/cars
        public async Task<PaginatedDto<Car>> Get(PaginatedRequestCommand cmd) {

            var cars = await _carsClient.GetCars(cmd);
            return cars;
        }

        // GET /carsclient/cars/{id}
        public Task<Car> GetCar(int id) {

            return _carsClient.GetCar(id);
        }

        // POST /carsclient/cars
        public async Task<HttpResponseMessage> PostCar(Car car) {

            var createdCar = await _carsClient.AddCar(car);
            var response = Request.CreateResponse(HttpStatusCode.Created, createdCar);
            response.Headers.Location = new Uri(
                Url.Link("ClientHttpRoute", new { id = createdCar.Id }));

            return response;
        }

        // PUT /carsclient/cars/{id}
        public async Task<Car> PutCar(int id, Car car) {

            var updatedCar = await _carsClient.UpdateCar(id, car);
            return updatedCar;
        }

        // DELETE /carsclient/cars/{id}
        public async Task<HttpResponseMessage> DeleteCar(int id) {

            await _carsClient.RemoveCar(id);
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}