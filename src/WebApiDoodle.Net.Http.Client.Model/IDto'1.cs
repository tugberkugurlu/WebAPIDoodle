
namespace WebApiDoodle.Net.Http.Client.Model {
    
    public interface IDto<TId> {

        TId Id { get; set; }
    }
}