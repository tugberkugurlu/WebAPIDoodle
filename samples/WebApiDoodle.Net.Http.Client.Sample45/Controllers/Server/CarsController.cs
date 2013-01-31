using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiDoodle.Net.Http.Client.Model;
using WebApiDoodle.Net.Http.Client.Sample45.Filters;
using WebApiDoodle.Net.Http.Client.Sample45.Models;
using WebApiDoodle.Net.Http.Client.Sample45.RequestCommands;

namespace WebApiDoodle.Net.Http.Client.Sample45.Controllers.Server {

    [InvalidModelStateFilter]
    public class CarsController : ApiController {

        private readonly CarsContext _carsCtx = new CarsContext();
	
        // GET /api/cars
        public PaginatedDto<Car> Get(PaginatedRequestCommand cmd) {

            return _carsCtx.GetAll(cmd.Page, cmd.Take).ToPaginatedDto();
        }

        // GET /api/cars/{id}
        public Car GetCar(int id) {

            var carTuple = _carsCtx.GetSingle(id);

            if (!carTuple.Item1) {

                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }

            return carTuple.Item2;
        }

        // POST /api/cars
        public HttpResponseMessage PostCar(Car car) {

            var createdCar = _carsCtx.Add(car);
            var response = Request.CreateResponse(HttpStatusCode.Created, createdCar);
            response.Headers.Location = new Uri(
                Url.Link("ServerHttpRoute", new { id = createdCar.Id }));

            return response;
        }

        // PUT /api/cars/{id}
        public Car PutCar(int id, Car car) {

            car.Id = id;

            if (!_carsCtx.TryUpdate(car)) {

                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }

            return car;
        }

        // DELETE /api/cars/{id}
        public HttpResponseMessage DeleteCar(int id) {

            if (!_carsCtx.TryRemove(id)) {

                var response = Request.CreateResponse(HttpStatusCode.NotFound);
                throw new HttpResponseException(response);
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}