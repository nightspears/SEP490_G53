using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QRCoder;
using System.Net.Mail;
using System.Net;
using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;
using TCViettelFC_API.Dtos.CheckOut;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class TicketUtilRepository : ITicketUtilRepository
    {
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Cloudinary _cloudinary;
        private string htmlHeader = @"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Football Tickets</title>
        <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f7f7f7;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .ticket {
            width: 300px;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            overflow: hidden;
            padding: 20px;
            position: relative;
        }
        .header {
            background-color: #e60000;
            padding: 10px;
            text-align: center;
            color: #fff;
        }
        .header h2 {
            margin: 0;
            font-size: 18px;
        }
        .header p {
            margin: 0;
            font-size: 12px;
        }
        .match-details {
            text-align: center;
            padding: 15px 0;
            border-bottom: 1px solid #ddd;
        }
        .match-details p {
            margin: 5px 0;
        }
        .match-details .date {
            font-size: 16px;
            font-weight: bold;
        }
        .match-details .teams {
            font-size: 18px;
            font-weight: bold;
        }
        .ticket-info {
            display: flex;
            justify-content: space-between;
            padding: 15px 0;
        }
        .ticket-info div {
            text-align: center;
            font-size: 14px;
        }
        .qr-code {
            text-align: center;
            margin: 10px 0;
        }
        .qr-code img {
            width: 100px;
            height: 100px;
        }
        .footer {
            text-align: center;
            margin-top: 10px;
            font-size: 12px;
            color: #888;
        }
        .sponsor-logos {
            display: flex;
            justify-content: space-between;
            margin-top: 10px;
        }
        .sponsor-logos img {
            width: 50px;
        }
        .ticket-container{
width:300px;
}
    </style>
    </head>
    <body>
        <div class='ticket-container'>
    ";
        private string htmlFooter = @"
        </div>
    </body>
    </html>";
        private readonly Sep490G53Context _context;
        public TicketUtilRepository(IOptions<CloudinarySettings> config, IEmailService emailService, IHttpContextAccessor contextAccessor, Sep490G53Context context)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _emailService = emailService;
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }
        public async Task<List<OrderedTicket>> GetOrderedTicketsByOrderId(int orderId)
        {
            return await _context.OrderedTickets.Include(x => x.Area).Include(x => x.Match).Where(x => x.OrderId == orderId).ToListAsync();
        }
        public async Task<List<OrderedSuppItem>> GetOrderedSuppByOrderId(int orderId)
        {
            return await _context.OrderedSuppItems.Include(x => x.Item).Where(x => x.OrderId == orderId).ToListAsync();
        }
        public async Task<OrderedTicket> GetOrderedTicketByIdAsync(int id)
        {
            return await _context.OrderedTickets.Include(x => x.Area).Include(x => x.Match).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<int> VerifyTicketAsync(OrderedTicket ticket)
        {
            if (ticket.Status == 0)
            {
                ticket.Status = 1;
                await _context.SaveChangesAsync();
                return 1;
            }
            return 0;
        }
        public async Task<List<VerifySupDto>> VerifyItemAsync(List<OrderedSuppItem> item)
        {
            List<VerifySupDto> res = new();
            foreach (var item1 in item)
            {
                if (item1.Status == 0)
                {
                    item1.Status = 1;
                    res.Add(new VerifySupDto
                    {
                        ItemName = item1.Item.ItemName,
                        Price = (decimal)item1.Price!,
                        Quantity = (int)item1.Quantity!
                    });
                    await _context.SaveChangesAsync();
                }
            }
            return res;

        }
        public async Task SendOrderConfirmationEmailAsync(CreateOrderRequest request)
        {
            // Extract relevant details from the request
            string customerName = request.Customer.FullName ?? "Customer";
            string customerEmail = request.Customer.Email;
            string customerPhone = request.Customer.Phone ?? "N/A";
            string orderCode = request.OrderProduct.OrderCode;
            DateTime orderDate = request.OrderProduct.OrderDate;
            decimal totalAmount = request.Payment.TotalAmount;
            decimal shipmentFee = request.OrderProduct.ShipmentFee ?? 0;

            string shippingAddress = $"{request.Address.DetailedAddress}, " +
                                      $"{request.Address.WardName}, {request.Address.DistrictName}, " +
                                      $"{request.Address.CityName}";

            // Generate product details in HTML format
            string productDetails = "<ul style='list-style-type:none; padding: 0; margin: 0;'>";
            foreach (var detail in request.OrderProductDetails)
            {
                decimal productTotalPrice = detail.Price * detail.Quantity;
                string formattedPrice = detail.Price.ToString("C0", new CultureInfo("vi-VN"));
                string formattedTotalPrice = productTotalPrice.ToString("C0", new CultureInfo("vi-VN"));

                productDetails += $@"
<li style='padding-bottom: 10px; display: flex; align-items: center;'>
  <img src='{(!string.IsNullOrEmpty(detail.Avatar) ? detail.Avatar : "https://via.placeholder.com/50")}' 
       alt='{detail.ProductName}' 
       style='width: 50px; height: 50px; margin-right: 10px;'>
  <div>
    <p class='mb-0'>Tên Sản Phẩm: {detail.ProductName}</p>";

                if (detail.PlayerId > 0)
                {
                    productDetails += $"<p class='mb-0'>Mã Code In Số Áo: {detail.PlayerId}</p>";
                }

                if (!string.IsNullOrEmpty(detail.CustomShirtNumber) && !string.IsNullOrEmpty(detail.CustomShirtName))
                {
                    productDetails += $@"
<p class='mb-0'>Customer In Số Áo: {detail.CustomShirtName}</p>
<p class='mb-0'>Customer In Tên Áo: {detail.CustomShirtNumber}</p>";
                }

                productDetails += $@"
<p class='mb-0'>Kích Cỡ: {detail.Size}</p>
<p class='mb-0'>Số lượng: {detail.Quantity}</p>
<span>Đơn Giá: {formattedPrice}</span><br>                         
<span>Tổng: {formattedPrice} x {detail.Quantity} = {formattedTotalPrice}</span>
  </div>
</li>";
            }
            productDetails += "</ul>";

            // Format monetary values as VND
            string formattedTotalAmount = totalAmount.ToString("C0", new CultureInfo("vi-VN"));
            string formattedShipmentFee = shipmentFee.ToString("C0", new CultureInfo("vi-VN"));

            // Begin constructing the email HTML body
            string htmlBody = $@"
<html>
    <head>
        <style>
            body {{
                font-family: 'Arial', sans-serif;
                color: #333;
                line-height: 1.6;
                font-size: 16px;
                margin: 0;
                padding: 0;
                background-color: #f4f4f4;
            }}
            .container {{
                width: 100%;
                max-width: 600px;
                margin: 0 auto;
                background-color: #ffffff;
                padding: 20px;
                border-radius: 8px;
                border: 1px solid #ddd;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            }}
            .header {{
                text-align: center;
                background-color: #4CAF50;
                color: #fff;
                padding: 15px;
                font-size: 18px;
                font-weight: bold;
                border-radius: 8px 8px 0 0;
            }}
            .section {{
                margin-top: 20px;
                font-size: 14px;
            }}
            .section td {{
                padding: 8px;
                vertical-align: top;
            }}
            .order-summary, .payment-info {{
                background-color: #fff;
                border: 1px solid #ddd;
                border-radius: 8px;
                margin-bottom: 20px;
            }}
            .total {{
                font-weight: bold;
                font-size: 18px;
                color: #4CAF50;
                padding: 10px;
                background-color: #f1f1f1;
                text-align: right;
            }}
            a {{
                color: #4CAF50;
                text-decoration: none;
            }}
            .footer {{
                margin-top: 30px;
                text-align: center;
                font-size: 14px;
                color: #777;
            }}
            @media only screen and (max-width: 600px) {{
                body {{ font-size: 14px; }}
                .container {{ padding: 10px; }}
                .header {{ font-size: 16px; padding: 12px; }}
                .section td {{ display: block; width: 100%; padding: 5px 0; }}
                .order-summary, .payment-info {{ padding: 10px; }}
                .total {{ font-size: 16px; }}
                .footer {{ font-size: 12px; padding-top: 10px; }}
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <p>Đơn hàng xác nhận</p>
            </div>
            
            <p>Xin chào {customerName},</p>
            <p>Cảm ơn Anh/chị đã đặt hàng tại Shop TCVIETTELFC!</p>
            <p>Đơn hàng của Anh/chị đã được tiếp nhận, chúng tôi sẽ nhanh chóng liên hệ với Anh/chị.</p>

            <div class='section'>
                <table>
                    <tr>
                        <td><strong>Thông tin mua hàng</strong></td>
                        <td><strong>Địa chỉ nhận hàng</strong></td>
                    </tr>
                    <tr>
                        <td>{customerName}<br>{customerEmail}<br>{customerPhone}</td>
                        <td>{shippingAddress}</td>
                    </tr>
                </table>
            </div>

            <div class='section'>
                <table>
                    <tr>
                        <td><strong>Phương thức thanh toán</strong></td>
                        <td><strong>Phương thức vận chuyển</strong></td>
                    </tr>
                    <tr>
                        <td>
                            Thanh toán qua VNPAY<br>
                            STK: 1019873492<br>
                            NH: SHB (Sài Gòn - Hà Nội)<br>
                            Chủ TK: Công ty TC_VIETTELFC
                        </td>
                        <td>Đơn Vị Vận Chuyển sẽ thu</td>
                    </tr>
                </table>
            </div>

            <div class='order-summary'>
                <table>
                    <tr>
                        <td><strong>Thông tin đơn hàng</strong></td>
                    </tr>
                    <tr>
                        <td>Mã đơn hàng: {orderCode}</td>
                    </tr>
                    <tr>
                        <td>Ngày đặt hàng: {orderDate:dd/MM/yyyy}</td>
                    </tr>
                </table>
            </div>

            {productDetails}

            <div class='order-summary'>
                <table>
                    <tr>
                        <td><strong>Tổng tiền hàng hóa</strong></td>
                        <td><strong>Phí vận chuyển</strong></td>
                        <td><strong>Thành tiền</strong></td>
                    </tr>
                    <tr>
                         <td>{(totalAmount - shipmentFee).ToString("C0", new CultureInfo("vi-VN"))}</td>
                        <td>{formattedShipmentFee}</td>
                        <td class='total'>{formattedTotalAmount}</td>
                    </tr>
                </table>
            </div>

            <p>Nếu Anh/chị có bất kỳ câu hỏi nào, xin liên hệ với chúng tôi tại <a href='mailto:nguyenquyduong291103@gmail.com'>Shop@TC_Viettel.com</a></p>

            <p>Trân trọng,<br>Ban quản trị cửa hàng Shop TC_Viettel</p>
        </div>

        <div class='footer'>
            <p>Shop TC_Viettel FC</p>
            <p>Cảm ơn bạn đã tin tưởng mua sắm tại cửa hàng chúng tôi.</p>
        </div>
    </body>
</html>";

            // Send the email using the _emailService
            await _emailService.SendEmailAsync(customerEmail, "Xác nhận đơn hàng - Shop TCVIETTELFC", htmlBody);
        }
        public async Task<int> SendTicketViaEmailAsync(int orderId, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return 0;
            }
            var tickets = await GetOrderedTicketsByOrderId(orderId);
            if (tickets.Count() <= 0) return 0;
            var supp = await GetOrderedSuppByOrderId(orderId);






            string htmlBody = htmlHeader;
            QRCodeGenerator qr = new QRCodeGenerator();
            foreach (var ticket in tickets)
            {
                string ticketUrl = $"http://192.168.1.134:7004/Entry/VerifyTicket/{ticket.Id}";

                QRCodeData data = qr.CreateQrCode(ticketUrl, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(data);

                byte[] qrCodeImage = qrCode.GetGraphic(20);
                var imageUrl = await UploadQrCodeImageToServerAsync(qrCodeImage, ticket.Id);
                string ticketHtml;
                if (ticket.Area.Stands.Equals("C") || ticket.Area.Stands.Equals("D"))
                {
                    ticketHtml = $@"
            <div class='ticket-container'>
 <div class='ticket'>
     <div class='header'>
         <h2>CÔNG TY TNHH MTV THỂ THAO VIETTEL</h2>
         <p>Giải bóng đá Vô địch Quốc gia</p>
         <p>V.LEAGUE 1 - 2024</p>
     </div>
     <div class='match-details'>
         <p class='date'>21-10-2024 | 19:00</p>
         <p class='teams'>Thể Công–Viettel FC - SLNA FC</p>
         <p>{ticket.Match.StadiumName}</p>
     </div>
    <table class=""ticket-info"" width=""300px"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width: 100%; border-collapse: collapse; text-align: center; table-layout: fixed;"">
    <tr>
        <td style=""border-right: 1px solid #ddd; width: 135px; padding: 5px;"">
            <p style=""margin: 0;"">Khán đài</p>
            <p style=""margin: 0;""><strong>{ticket.Area.Stands}</strong></p>
        </td>
        <td style=""width: 135px; padding: 5px;"">
            <p style=""margin: 0;"">Cửa</p>
            <p style=""margin: 0;""><strong>{ticket.Area.Section}</strong></p>
        </td>
    </tr>
</table>

     <div class='qr-code'>
         <img src='{imageUrl}' alt='QR Code'>
     </div>
     <div class='footer'>
         <p>Giá vé: {ticket.Price}</p>
     </div>
 </div>
     </div>";
                }
                else
                {
                    ticketHtml = $@"
            <div class='ticket-container'>
 <div class='ticket'>
     <div class='header'>
         <h2>CÔNG TY TNHH MTV THỂ THAO VIETTEL</h2>
         <p>Giải bóng đá Vô địch Quốc gia</p>
         <p>V.LEAGUE 1 - 2024</p>
     </div>
     <div class='match-details'>
         <p class='date'>21-10-2024 | 19:00</p>
         <p class='teams'>Thể Công–Viettel FC - SLNA FC</p>
         <p>{ticket.Match.StadiumName}</p>
     </div>
    <table class=""ticket-info"" width=""300px"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width: 100%; border-collapse: collapse; text-align: center; table-layout: fixed;"">
    <tr>
        <td style=""border-right: 1px solid #ddd; width: 90px; padding: 5px;"">
            <p style=""margin: 0;"">Khán đài</p>
            <p style=""margin: 0;""><strong>{ticket.Area.Stands}</strong></p>
        </td>
        <td style=""border-right: 1px solid #ddd; width: 90px; padding: 5px;"">
            <p style=""margin: 0;"">Tầng</p>
            <p style=""margin: 0;""><strong>{ticket.Area.Floor}</strong></p>
        </td>
        <td style=""width: 90px; padding: 5px;"">
            <p style=""margin: 0;"">Cửa</p>
            <p style=""margin: 0;""><strong>{ticket.Area.Section}</strong></p>
        </td>
    </tr>
</table>

     <div class='qr-code'>
         <img src='{imageUrl}' alt='QR Code'>
     </div>
     <div class='footer'>
         <p>Giá vé: {ticket.Price}</p>
     </div>
 </div>
     </div>";
                }

                htmlBody += ticketHtml;
            }
            if (supp.Count > 0)
            {
                string suppUrl = $"http://192.168.1.134:7004/Entry/VerifySupItem/{orderId}";
                QRCodeData suppData = qr.CreateQrCode(suppUrl, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode suppQrCode = new PngByteQRCode(suppData);

                byte[] suppQrCodeImage = suppQrCode.GetGraphic(20);
                var suppImageUrl = await UploadQrCodeImageToServerAsync(suppQrCodeImage, orderId);

                string suppHtml = $@"
            <div class='ticket-container'>
                <div class='ticket'>
                    <div class='header'>
                        <h2>Mã QR đồ mua kèm</h2>
                    </div>
                    <div class='qr-code'>
                        <img src='{suppImageUrl}' alt='QR Code'>
                    </div>
                </div>
            </div>";

                htmlBody += suppHtml;
            }
            htmlBody += htmlFooter;
            await _emailService.SendEmailAsync(email, "Thông tin vé", htmlBody);
            return 1;
        }
        private async Task<string> UploadQrCodeImageToServerAsync(byte[] qrCodeImage, int ticketId)
        {
            using (var stream = new MemoryStream(qrCodeImage))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription($"ticket-{ticketId}.png", stream),
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                return uploadResult.SecureUrl.ToString();
            }
        }


    }
}

