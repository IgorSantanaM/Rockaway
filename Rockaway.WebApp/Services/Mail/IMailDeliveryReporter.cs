namespace Rockaway.WebApp.Services.Mail {
	public interface IMailDeliveryReporter {
		Task ReporteSuccessAsync(Guid orderId);
		Task ReporteFailureAsync(Guid orderId, string error);
	}
}
