namespace WooService.Workers;

using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WooCommerceNET.WooCommerce.v3;
using WooService.Contexts;
using WooService.Models;
using WooService.Providers;
using WooService.Utils;

public class WooServiceWorker(ILogger<WooServiceWorker> logger, AppSettings appsets, WooCommerceContext context, WooProvider wooProvider, SmtpClient emailService)
{
    private readonly ILogger<WooServiceWorker> _logger = logger;
    private readonly WooCommerceContext _context = context;
    private readonly WooProvider _wooProvider = wooProvider;
    private readonly SmtpClient _emailService = emailService;
    private readonly AppSettings _appSettings = appsets;

    /// <summary>
    /// Procesa los pedidos de WooCommerce según el estado especificado.
    /// </summary>
    /// <param name="OrderStatus">Estado del pedido a procesar.</param>
    /// <returns>Nada</returns>
    public async Task ProcessOrders(string OrderStatus)
    {
        try
        {
            var orders = await _wooProvider.ObtenerPedidos(OrderStatus);

            foreach (var order in orders)
            {
                await ProcessOrder(order);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing orders");
            await NotifyAdministrator(_appSettings.EMAILUser, _appSettings.EMAILPass, _appSettings.EMAILUser, "Error processing orders", ex.ToString());
        }
    }

    private async Task ProcessOrder(Order order)
    {

        // (Cliente? Cliente, string msgError) = await ProcesarDatosCliente(order);
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Register order in Woo_Order table
            var wooOrder = new WooOrder
            {
                WooJson = JsonConvert.SerializeObject(order),
                WooId = order.id,
                WooPaymentMethod = order.payment_method,
                WooStatus = order.status,
                WooKey = order.order_key,
                OrderId = GenerateLocalOrderId(),
                WooCustomerId = order.customer_id,
                AxCustomerCode = await GetAxCustomerCode(order.customer_id),
                Operated = false,
                SyncProduct = false,
                GenerateHeaderJson = false,
                GenerateDetailJson = false,
                WooDate = order.date_created,
                Date = DateTime.Now
            };

            _context.WooOrders.Add(wooOrder);
            await _context.SaveChangesAsync();

            // Register order in Order table
            var orderEntity = new Order
            {
                WooOrderId = wooOrder.Id,
                CompanyId = GetCompanyId(),
                CustomerId = wooOrder.AxCustomerCode,
                AddressId = await GetDeliveryAddressId(order),
                HeaderJson = GenerateHeaderJson(order),
                DetailJson = GenerateDetailJson(order),
                Operate = true,
                OperatedAx = false,
                HeaderGenerationDateTime = DateTime.Now,
                DetailGenerationDateTime = DateTime.Now,
                Correlative = await _context.Orders.CountAsync() + 1
            };

            _context.Orders.Add(orderEntity);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "{Message}", $"Error processing order {order.id}");
            await NotifyAdministrator(_appSettings.EMAILUser, _appSettings.EMAILPass, _appSettings.EMAILUser, $"Error processing order {order.id}", ex.ToString());
        }
    }

    private async Task NotifyAdministrator(string ServiceEmail, string ServiceEmailPassword, string emailAdmin, string asunto, string body)
    {
        // await _emailService.SendEmailAsync(WooCommerceContext, subject, body);
    }

    // Implement the following methods according to your business logic:
    // GenerateLocalOrderId(), GetAxCustomerCode(), GetCompanyId(), GetDeliveryAddressId(), GenerateHeaderJson(), GenerateDetailJson()

}