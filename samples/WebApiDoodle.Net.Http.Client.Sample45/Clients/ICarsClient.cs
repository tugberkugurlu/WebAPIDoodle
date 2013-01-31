using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApiDoodle.Net.Http.Client.Model;
using WebApiDoodle.Net.Http.Client.Sample45.Models;
using WebApiDoodle.Net.Http.Client.Sample45.RequestCommands;

namespace WebApiDoodle.Net.Http.Client.Sample45.Clients {

    public interface ICarsClient {

        Task<PaginatedDto<Car>> GetCars(PaginatedRequestCommand paginationCmd);
        Task<Car> GetCar(int carId);
        Task<Car> AddCar(Car car);
        Task<Car> UpdateCar(int carId, Car car);
        Task RemoveCar(int carId);
    }
}