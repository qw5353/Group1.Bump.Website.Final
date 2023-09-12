using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MimeKit;

namespace webapi.Helpers
{
	public class Email
	{
		public string? Receiver { get; set; }
		public string? Subject { get; set; }
		public string? Body { get; set; }
		public bool isHtml { get; set; } = false;
	}
	public class EmailHelper : IEmailHelper
	{
		private readonly string? _account;
		private readonly string? _password;
		public EmailHelper(IConfiguration config)
		{
			_account = config["EmailConfig:Account"];
			_password = config["EmailConfig:Password"];
		}

		private MimeMessage CreateMessage(Email email)
		{
			var message = new MimeMessage();
			

			message.From.Add(new MailboxAddress("Bump", "bump.estore.service@gmail.com"));
			message.To.Add(new MailboxAddress("", email.Receiver));

			message.Subject = email.Subject;

			var mailBody = new BodyBuilder();

			if(email.isHtml)
			{
				mailBody.HtmlBody = email.Body;
			} else
			{
				mailBody.TextBody = email.Body;
			}
			
			
			message.Body = mailBody.ToMessageBody();

			return message;
		}

		public void SendEmail(Email email)
		{
			using (var client = new SmtpClient())
			{
				var message = CreateMessage(email);
				var host = "smtp.gmail.com";
				var port = 587;

				client.Connect(host, port, SecureSocketOptions.StartTls);
				client.Authenticate(_account, _password);

				client.Send(message);

				client.Disconnect(true);
			}
		}

		public void MemberConfirmEmail(string email, string url)
		{
			var text = new Email
			{
				Receiver = email,
				Subject = "【Bump Climbing】立即驗證Bump會員，享受攀岩樂趣!",
				#region EmailContext
				Body = $@"<div style=""margin:0;padding:0"" bgcolor=""#F0F0F0"" marginwidth=""0"" marginheight=""0"">
    <table border=""0"" width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#F0F0F0;"">
        <tbody>
            <tr>
                <td align=""center"" valign=""top"" bgcolor=""#F0F0F0"" style=""background-color:#f0f0f0""> <br>
                    <table border=""0"" width=""600"" cellpadding=""0"" cellspacing=""0"" style=""width:600px;max-width:600px"">
                        <tbody>
                            <tr>
                                <td align=""left""
                                    style=""padding-left:24px;padding-right:24px;padding-top:12px;padding-bottom:12px;background-color:#ffffff"">
                                    <div
                                        style=""min-height:auto;padding:15px;text-align:center;max-height:150px;max-width:100%"">
                                        <img style=""max-height:50px;max-width:100%""
                                            src=""https://cdn.discordapp.com/attachments/984493638149107750/1145287279586328626/siteLOGO.png"">
                                    </div> <br>
                                    <div style=""height:1px;border-bottom:1px solid #cccccc;clear:both""></div> <br>
                                    <div
                                        style=""font-family:Helvetica,Arial,sans-serif;font-size:14px;line-height:20px;text-align:left;color:#333333"">
                                        <p>一鍵驗證您的帳號！</p>
                                        <p>嗨，<a href=""mailto:{email}"">{email}</a></p>
                                        <p>感謝您成為 Bump Climbing會員，請在 24
                                            小時內點擊下方按鈕來驗證您的電郵帳號，
                                            逾期連結將失效。
                                        </p>
                                        <div style=""margin:10px 0""> <a
                                                style=""background-color:rgb(230, 169, 71);color:#ffffff;padding:10px 30px;text-decoration:none;text-align:center;border-radius:3px;text-transform:uppercase;margin:30px 0;display:block""
                                                href=""{url}"">
                                                按下認證信箱!</a> </div>
                                    </div> <br>
                                </td>
                            </tr>
                            <tr>
                                <td align=""left""
                                    style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;line-height:16px;color:#aaaaaa;padding-left:24px;padding-right:24px"">
                                    <br><br> <strong>Bump Climbing!</strong> <br><br>
                                    這封電郵是寄到<a href=""{email}"">{email}</a>。<br> 您會收到這封電郵是由於您使用這個電郵地址註冊成為 Bump Climbing 攀岩趣的會員。 <br><br> <br><br>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
</div>",
				#endregion
				isHtml = true
			};
			SendEmail(text);
		}

		public void ChangePasswordEmail(string name, string email, string changeUrl, string reSendurl)
		{
			var text = new Email
			{
				Receiver = email,
				Subject = "【Bump Climbing】重置您的密碼",
				#region EmailContext
				Body = $@"    <div style=""margin:0;padding:0"" bgcolor=""#F0F0F0"" marginwidth=""0"" marginheight=""0"">
        <table border=""0"" width=""100%"" height=""100%"" cellpadding=""0"" cellspacing=""0"" bgcolor=""#F0F0F0;"">
            <tbody>
                <tr>
                    <td align=""center"" valign=""top"" bgcolor=""#F0F0F0"" style=""background-color:#f0f0f0""> <br>
                        <table border=""0"" width=""600"" cellpadding=""0"" cellspacing=""0""
                            style=""width:600px;max-width:600px"">
                            <tbody>
                                <tr>
                                    <td align=""left""
                                        style=""padding-left:24px;padding-right:24px;padding-top:12px;padding-bottom:12px;background-color:#ffffff"">
                                        <div
                                            style=""min-height:auto;padding:15px;text-align:center;max-height:150px;max-width:100%"">
                                            <img style=""max-height:50px;max-width:100%""
                                                src=""https://cdn.discordapp.com/attachments/984493638149107750/1145287279586328626/siteLOGO.png"">
                                        </div> <br>
                                        <div style=""height:1px;border-bottom:1px solid #cccccc;clear:both""></div> <br>
                                        <div
                                            style=""font-family:Helvetica,Arial,sans-serif;font-size:14px;line-height:20px;text-align:left;color:#333333"">
                                            <p>您好 {name}!</p>
                                            <p>要重新設定您的密碼，請點一下以下的連結。您將會連到一個網頁，<wbr>讓您設定新的密碼。</p>
                                            <p>如果您並未嘗試重新設定密碼，請不必擔心，可略過這封電郵。 在您點選連結設定密碼前，您的密碼不會改變。</p>
                                            <p>**提醒您：若此信件在垃圾信件匣，請將信件移至收件匣，<wbr>更變密碼連結才可使用。**</p>
                                            <div style=""margin:10px 0""> <a
                                                    style=""background-color:#4CAF50;color:#fff;padding:10px 30px;text-decoration:none;text-align:center;border-radius:3px;text-transform:uppercase;margin:30px 0;display:block""
                                                    href=""{changeUrl}"">
                                                    變更密碼 </a>
                                                <p>密碼設定連結將於24小時內失效，若要重新申請，請按<a
                                                        href=""{reSendurl}""
                                                        style=""background-color:rgb(230, 169, 71);color:rgb(255, 255, 255);padding:8px 8px;text-decoration:none;border-radius:20px;text-transform:uppercase;margin:10px 0;"">此</a>重新設定<wbr>，並立即至電郵信箱確認。
                                                </p>
                                            </div>
                                        </div><br>
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""left""
                                        style=""font-family:Helvetica,Arial,sans-serif;font-size:12px;line-height:16px;color:#aaaaaa;padding-left:24px;padding-right:24px"">
                                        <br><br> <strong>Bump Climbing!</strong> <br><br>
                                        這封電郵是寄到<a href=""mailto:{email}"">{email}</a>。<br>
                                        您會收到這封電郵是由於您使用這個電郵地址註冊成為Bump Climbing 攀岩趣 的會員。 <br><br> <br><br>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>",
				#endregion
				isHtml = true
			};
            SendEmail(text);
		}
	}
}
