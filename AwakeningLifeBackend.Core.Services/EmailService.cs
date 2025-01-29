using AwakeningLifeBackend.Core.Services.Abstractions;
using Resend;

namespace AwakeningLifeBackend.Core.Services;

public class EmailService : IEmailService
{
    private readonly IResend _resend;

    public EmailService(IResend resend)
    {
        _resend = resend;
    }

    public async Task SendEmailAsync(string recipient, string passwordResetLink)
    {
        var message = new EmailMessage();
        message.From = "no-reply@accounts.equanimity-solutions.com";
        message.To.Add(recipient);
        message.Subject = "Password Reset Request - Equanimity Accounts";
        message.HtmlBody = $@"
<!DOCTYPE html>
<html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='format-detection' content='telephone=no, date=no, address=no, email=no'>
    <meta name='x-apple-disable-message-reformatting'>
    <meta name='color-scheme' content='light dark'>
    <meta name='supported-color-schemes' content='light dark'>
    <title>Password Reset - Equanimity</title>
    <!--[if mso]>
    <noscript>
        <xml>
            <o:OfficeDocumentSettings>
                <o:PixelsPerInch>96</o:PixelsPerInch>
            </o:OfficeDocumentSettings>
        </xml>
    </noscript>
    <![endif]-->
</head>
<body style='margin: 0; padding: 0; background-color: #f5f4f4; font-family: Arial, sans-serif; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%;'>
    <!-- Preview Text -->
    <div style='display: none; max-height: 0px; overflow: hidden;'>
        Reset your password for your Equanimity account - this link will expire in 24 hours.
        &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
    </div>
    
    <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; border: 0; border-spacing: 0; background: #ffffff;'>
                    <!-- Header -->
                    <tr>
                        <td style='background-color: #345053; padding: 30px; border-radius: 8px 8px 0 0;' align='center'>
                            <h1 style='color: #f5f4f4; margin: 0; font-size: 24px; font-family: Arial, sans-serif;'>Password Reset Request</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        A password reset has been requested for your account. If you did not make this request, please ignore this email.
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 35px;'>
                                        To reset your password, click the button below:
                                    </td>
                                </tr>
                                <tr>
                                    <td align='center' style='padding-bottom: 35px;'>
                                        <!--[if mso]>
                                        <v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='{passwordResetLink}' style='height:45px;v-text-anchor:middle;width:200px;' arcsize='10%' strokecolor='#66ac9d' fillcolor='#66ac9d'>
                                            <w:anchorlock/>
                                            <center style='color:#ffffff;font-family:Arial,sans-serif;font-size:16px;font-weight:bold;'>Reset Password</center>
                                        </v:roundrect>
                                        <![endif]-->
                                        <!--[if !mso]><!-->
                                        <a href='{passwordResetLink}' style='background-color: #66ac9d; border: 1px solid #66ac9d; border-radius: 6px; color: #ffffff; display: inline-block; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold; line-height: 45px; text-align: center; text-decoration: none; width: 200px; -webkit-text-size-adjust: none;'>Reset Password</a>
                                        <!--<![endif]-->
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; line-height: 1.5;'>
                                        If the button doesn't work, copy and paste this link into your browser:<br>
                                        <a href='{passwordResetLink}' style='color: #66ac9d; text-decoration: underline; word-break: break-all;'>{passwordResetLink}</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f5f4f4; padding: 30px; border-radius: 0 0 8px 8px;' align='center'>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.Now.Year} Equanimity. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        message.Headers = new Dictionary<string, string>
        {
            { "List-Unsubscribe", "<mailto:unsubscribe@accounts.equanimity-solutions.com?subject=unsubscribe>" },
            { "Precedence", "bulk" },
            { "X-Auto-Response-Suppress", "OOF, AutoReply" }
        };

        await _resend.EmailSendAsync(message);
    }
}
