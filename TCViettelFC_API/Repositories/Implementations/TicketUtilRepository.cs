using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QRCoder;
using TCViettelFC_API.Models;
using TCViettelFC_API.Repositories.Interfaces;

namespace TCViettelFC_API.Repositories.Implementations
{
    public class TicketUtilRepository : ITicketUtilRepository
    {
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Cloudinary _cloudinary;

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
        public async Task<int> SendTicketViaEmailAsync(List<int> ticketIds, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return 0;
            }
            string htmlHeader = @"
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

            string htmlFooter = @"
        </div>
    </body>
    </html>";

            string htmlBody = htmlHeader;
            QRCodeGenerator qr = new QRCodeGenerator();
            foreach (var ticketId in ticketIds)
            {
                var ticket = await _context.OrderedTickets.Include(x => x.Match).Include(x => x.Area).FirstOrDefaultAsync(x => x.Id == ticketId);  // Assuming this function gets the OrderedTicket by id

                if (ticket == null)
                    continue;
                string ticketUrl = $"http://192.168.1.134:7004/Entry/VerifyTicket/{ticket.Id}";

                QRCodeData data = qr.CreateQrCode(ticketUrl, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(data);

                byte[] qrCodeImage = qrCode.GetGraphic(20);
                var imageUrl = await UploadQrCodeImageToServerAsync(qrCodeImage, ticket.Id);

                string ticketHtml = $@"
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
                htmlBody += ticketHtml;
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

