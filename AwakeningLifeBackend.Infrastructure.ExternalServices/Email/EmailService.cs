using Resend;

namespace AwakeningLifeBackend.Infrastructure.ExternalServices;

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
        message.From = "no-reply@info.awakeninglife.ai";
        message.To.Add(recipient);
        message.Subject = "Reset Your Password - Awakening Life";
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
    <title>Reset Your Password - Awakening Life</title>
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
        Reset your password for your Awakening Life account - this link will expire in 24 hours.
        &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
    </div>
    
    <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; border: 0; border-spacing: 0; background: #ffffff;'>
                    <!-- Header -->
                    <tr>
                        <td style='background-color: #345053; padding: 30px; border-radius: 8px 8px 0 0;' align='center'>
                            <h1 style='color: #f5f4f4; margin: 0; font-size: 24px; font-family: Arial, sans-serif;'>Reset Your Password</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        We received a request to reset your password for your Awakening Life journey. If you didn't make this request, you can safely ignore this email.
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 35px;'>
                                        To reset your password and continue your path of self-discovery, click the button below:
                                    </td>
                                </tr>
                                <tr>
                                    <td align='center' style='padding-bottom: 35px;'>
                                        <!--[if mso]>
                                        <v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='{passwordResetLink}' style='height:45px;v-text-anchor:middle;width:200px;' arcsize='10%' strokecolor='#4F46E5' fillcolor='#4F46E5'>
                                            <w:anchorlock/>
                                            <center style='color:#ffffff;font-family:Arial,sans-serif;font-size:16px;font-weight:bold;'>Reset Password</center>
                                        </v:roundrect>
                                        <![endif]-->
                                        <!--[if !mso]><!-->
                                        <a href='{passwordResetLink}' style='background-color: #4F46E5; border: 1px solid #4F46E5; border-radius: 6px; color: #ffffff; display: inline-block; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold; line-height: 45px; text-align: center; text-decoration: none; width: 200px; -webkit-text-size-adjust: none;'>Reset Password</a>
                                        <!--<![endif]-->
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; line-height: 1.5;'>
                                        If the button doesn't work, copy and paste this link into your browser:<br>
                                        <a href='{passwordResetLink}' style='color: #4F46E5; text-decoration: underline; word-break: break-all;'>{passwordResetLink}</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; line-height: 1.5; padding-top: 30px; border-top: 1px solid #f0f0f0; margin-top: 30px;'>
                                        <p>This link will expire in 24 hours for security reasons.</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f5f4f4; padding: 30px; border-radius: 0 0 8px 8px;' align='center'>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0 0 10px 0;'>
                                ""The journey inward is the journey forward.""
                            </p>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.Now.Year} Awakening Life. All rights reserved.
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
            { "List-Unsubscribe", "<mailto:unsubscribe@info.awakeninglife.ai?subject=unsubscribe>" },
            { "Precedence", "bulk" },
            { "X-Auto-Response-Suppress", "OOF, AutoReply" }
        };

        await _resend.EmailSendAsync(message);
    }

    public async Task SendEmailConfirmationAsync(string recipient, string confirmationLink)
    {
        var message = new EmailMessage();
        message.From = "no-reply@info.awakeninglife.ai";
        message.To.Add(recipient);
        message.Subject = "Confirm Your Email - Begin Your Awakening Journey";
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
    <title>Confirm Your Email - Awakening Life</title>
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
        One step closer to beginning your journey of self-discovery with Awakening Life.
        &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
    </div>
    
    <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; border: 0; border-spacing: 0; background: #ffffff;'>
                    <!-- Header -->
                    <tr>
                        <td style='background-color: #345053; padding: 30px; border-radius: 8px 8px 0 0;' align='center'>
                            <h1 style='color: #f5f4f4; margin: 0; font-size: 24px; font-family: Arial, sans-serif;'>Confirm Your Email</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        Thank you for creating your Awakening Life account. We're excited to guide you on your journey toward self-realization.
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        To begin your journey, please confirm your email address:
                                    </td>
                                </tr>
                                <tr>
                                    <td align='center' style='padding-bottom: 35px;'>
                                        <a href='{confirmationLink}' style='background-color: #10B981; border: 1px solid #10B981; border-radius: 6px; color: #ffffff; display: inline-block; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold; line-height: 45px; text-align: center; text-decoration: none; width: 200px;'>Confirm Email</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; line-height: 1.5;'>
                                        If the button doesn't work, copy and paste this link into your browser:<br>
                                        <a href='{confirmationLink}' style='color: #10B981; text-decoration: underline; word-break: break-all;'>{confirmationLink}</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-top: 30px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4; border-radius: 8px;'>
                                            <tr>
                                                <td style='padding: 20px; text-align: center;'>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; line-height: 1.5; margin: 0;'>
                                                        ""The journey of a thousand miles begins with a single step.""
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f5f4f4; padding: 30px; border-radius: 0 0 8px 8px;' align='center'>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.Now.Year} Awakening Life. All rights reserved.
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
            { "List-Unsubscribe", "<mailto:unsubscribe@info.awakeninglife.ai?subject=unsubscribe>" },
            { "Precedence", "bulk" },
            { "X-Auto-Response-Suppress", "OOF, AutoReply" }
        };

        await _resend.EmailSendAsync(message);
    }

    public async Task SendSubscriptionCanceledEmailAsync(string recipient, string resubscribeLink)
    {
        var message = new EmailMessage();
        message.From = "no-reply@info.awakeninglife.ai";
        message.To.Add(recipient);
        message.Subject = "Your Awakening Life Journey - Subscription Update";
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
    <title>Subscription Update - Awakening Life</title>
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
        Your Awakening Life subscription has been paused. Your journey can continue whenever you're ready.
        &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
    </div>
    
    <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; border: 0; border-spacing: 0; background: #ffffff;'>
                    <!-- Header -->
                    <tr>
                        <td style='background-color: #345053; padding: 30px; border-radius: 8px 8px 0 0;' align='center'>
                            <h1 style='color: #f5f4f4; margin: 0; font-size: 24px; font-family: Arial, sans-serif;'>Your Journey Is Paused</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        We've processed your request to pause your Awakening Life subscription. Your account has been updated successfully.
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-bottom: 30px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4; border-radius: 8px;'>
                                            <tr>
                                                <td style='padding: 20px; text-align: center;'>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; line-height: 1.5; margin: 0;'>
                                                        ""Every ending is a new beginning. The path remains whenever you're ready to return.""
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        We understand that life's journey has many paths. Should you wish to continue your exploration of self-realization with us, you can reactivate your subscription at any time:
                                    </td>
                                </tr>
                                <tr>
                                    <td align='center' style='padding-bottom: 35px;'>
                                        <!--[if mso]>
                                        <v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='{resubscribeLink}' style='height:45px;v-text-anchor:middle;width:220px;' arcsize='10%' strokecolor='#8B5CF6' fillcolor='#8B5CF6'>
                                            <w:anchorlock/>
                                            <center style='color:#ffffff;font-family:Arial,sans-serif;font-size:16px;font-weight:bold;'>Continue My Journey</center>
                                        </v:roundrect>
                                        <![endif]-->
                                        <!--[if !mso]><!-->
                                        <a href='{resubscribeLink}' style='background-color: #8B5CF6; border: 1px solid #8B5CF6; border-radius: 6px; color: #ffffff; display: inline-block; font-family: Arial, sans-serif; font-size: 16px; font-weight: bold; line-height: 45px; text-align: center; text-decoration: none; width: 220px; -webkit-text-size-adjust: none;'>Continue My Journey</a>
                                        <!--<![endif]-->
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; line-height: 1.5;'>
                                        If you have any questions about your subscription or would like to share feedback about your experience, we're here to help at <a href='mailto:support@awakeninglife.ai' style='color: #8B5CF6; text-decoration: underline;'>support@awakeninglife.ai</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f5f4f4; padding: 30px; border-radius: 0 0 8px 8px;' align='center'>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.Now.Year} Awakening Life. All rights reserved.
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
            { "List-Unsubscribe", "<mailto:unsubscribe@info.awakeninglife.ai?subject=unsubscribe>" },
            { "Precedence", "bulk" },
            { "X-Auto-Response-Suppress", "OOF, AutoReply" }
        };

        await _resend.EmailSendAsync(message);
    }

    public async Task SendWaitlistConfirmationEmailAsync(string recipient)
    {
        var message = new EmailMessage();
        message.From = "no-reply@info.awakeninglife.ai";
        message.To.Add(recipient);
        message.Subject = "Welcome to the Awakening Life Journey";
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
    <title>Welcome to Awakening Life</title>
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
        Your journey to self-realization begins now. We'll notify you as soon as your access is ready.
        &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
    </div>
    
    <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; border: 0; border-spacing: 0; background: #ffffff;'>
                    <!-- Header -->
                    <tr>
                        <td style='background-color: #345053; padding: 30px; border-radius: 8px 8px 0 0;' align='center'>
                            <h1 style='color: #f5f4f4; margin: 0; font-size: 24px; font-family: Arial, sans-serif;'>Your Journey Awaits</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        Thank you for joining the Awakening Life waitlist. You've taken the first step on a profound journey toward self-realization.
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-bottom: 30px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4; border-radius: 8px;'>
                                            <tr>
                                                <td style='padding: 20px; text-align: center;'>
                                                    <h3 style='color: #345053; font-family: Arial, sans-serif; margin-top: 0; margin-bottom: 15px; font-size: 18px;'>What's Next on Your Path</h3>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; line-height: 1.5; margin: 0;'>
                                                        We're preparing your access to guided journeys that will help you discover your true nature through:
                                                    </p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-bottom: 30px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                            <tr>
                                                <td style='padding: 0 10px 20px 10px; width: 50px; vertical-align: top;'>
                                                    <div style='width: 40px; height: 40px; border-radius: 50%; background-color: rgba(79, 70, 229, 0.1); display: inline-block; text-align: center; line-height: 40px;'>
                                                        <span style='color: #4F46E5; font-size: 18px;'>✦</span>
                                                    </div>
                                                </td>
                                                <td style='padding: 0 0 20px 0;'>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; font-weight: bold; margin: 0 0 5px 0;'>Intention</p>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>Begin with clear purpose to directly experience your true nature</p>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 0 10px 20px 10px; width: 50px; vertical-align: top;'>
                                                    <div style='width: 40px; height: 40px; border-radius: 50%; background-color: rgba(236, 72, 153, 0.1); display: inline-block; text-align: center; line-height: 40px;'>
                                                        <span style='color: #EC4899; font-size: 18px;'>◇</span>
                                                    </div>
                                                </td>
                                                <td style='padding: 0 0 20px 0;'>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; font-weight: bold; margin: 0 0 5px 0;'>Openness</p>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>Create space for insights to emerge naturally without judgment</p>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 0 10px 20px 10px; width: 50px; vertical-align: top;'>
                                                    <div style='width: 40px; height: 40px; border-radius: 50%; background-color: rgba(16, 185, 129, 0.1); display: inline-block; text-align: center; line-height: 40px;'>
                                                        <span style='color: #10B981; font-size: 18px;'>✧</span>
                                                    </div>
                                                </td>
                                                <td style='padding: 0 0 20px 0;'>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; font-weight: bold; margin: 0 0 5px 0;'>Communication</p>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>Express your experiences authentically for deeper release</p>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 0 10px 0 10px; width: 50px; vertical-align: top;'>
                                                    <div style='width: 40px; height: 40px; border-radius: 50%; background-color: rgba(139, 92, 246, 0.1); display: inline-block; text-align: center; line-height: 40px;'>
                                                        <span style='color: #8B5CF6; font-size: 18px;'>⟳</span>
                                                    </div>
                                                </td>
                                                <td style='padding: 0;'>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 15px; font-weight: bold; margin: 0 0 5px 0;'>Surrender</p>
                                                    <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>Release attachment to outcomes, trusting the process of discovery</p>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        We'll notify you as soon as your access is ready. In the meantime, take a moment each day to pause and notice who is aware of your thoughts and experiences.
                                    </td>
                                </tr>
                                <tr>
                                    <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding-bottom: 25px;'>
                                        Your journey to self-realization has already begun.
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f5f4f4; padding: 30px; border-radius: 0 0 8px 8px;' align='center'>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0 0 10px 0;'>
                                The truth is not far away; it is ever present.
                            </p>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.Now.Year} Awakening Life. All rights reserved.
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

    public async Task SendContactFormEmailAsync(string recipientEmail, string name, string email, string subject, string message)
    {
        var emailMessage = new EmailMessage();
        emailMessage.From = "no-reply@info.awakeninglife.ai";
        emailMessage.To.Add(recipientEmail);
        emailMessage.Subject = $"New Contact Form Submission: {subject}";
        emailMessage.HtmlBody = $@"
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
    <title>New Contact Form Submission</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f5f4f4; font-family: Arial, sans-serif; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0; background: #f5f4f4;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; border: 0; border-spacing: 0; background: #ffffff;'>
                    <!-- Header -->
                    <tr>
                        <td style='background-color: #345053; padding: 30px; border-radius: 8px 8px 0 0;' align='center'>
                            <h1 style='color: #f5f4f4; margin: 0; font-size: 24px; font-family: Arial, sans-serif;'>New Contact Form Submission</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                <tr>
                                    <td style='padding-bottom: 20px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                            <tr>
                                                <td style='width: 150px; color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; font-weight: bold; padding: 10px; background-color: #f5f4f4; border-radius: 4px 0 0 4px;'>
                                                    Name:
                                                </td>
                                                <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding: 10px; background-color: #f9f9f9; border-radius: 0 4px 4px 0;'>
                                                    {name}
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-bottom: 20px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                            <tr>
                                                <td style='width: 150px; color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; font-weight: bold; padding: 10px; background-color: #f5f4f4; border-radius: 4px 0 0 4px;'>
                                                    Email:
                                                </td>
                                                <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding: 10px; background-color: #f9f9f9; border-radius: 0 4px 4px 0;'>
                                                    <a href='mailto:{email}' style='color: #4F46E5; text-decoration: underline;'>{email}</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-bottom: 20px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                            <tr>
                                                <td style='width: 150px; color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; font-weight: bold; padding: 10px; background-color: #f5f4f4; border-radius: 4px 0 0 4px;'>
                                                    Subject:
                                                </td>
                                                <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding: 10px; background-color: #f9f9f9; border-radius: 0 4px 4px 0;'>
                                                    {subject}
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-bottom: 20px;'>
                                        <table role='presentation' style='width: 100%; border-collapse: collapse; border: 0; border-spacing: 0;'>
                                            <tr>
                                                <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; font-weight: bold; padding: 10px; background-color: #f5f4f4; border-radius: 4px 4px 0 0;'>
                                                    Message:
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='color: #345053; font-family: Arial, sans-serif; font-size: 16px; line-height: 1.5; padding: 20px; background-color: #f9f9f9; border-radius: 0 0 4px 4px; white-space: pre-wrap;'>
                                                    {message}
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding-top: 20px; border-top: 1px solid #f0f0f0;'>
                                        <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                            This message was sent from the Awakening Life contact form at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f5f4f4; padding: 30px; border-radius: 0 0 8px 8px;' align='center'>
                            <p style='color: #345053; font-family: Arial, sans-serif; font-size: 14px; margin: 0;'>
                                &copy; {DateTime.Now.Year} Awakening Life. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

        emailMessage.Headers = new Dictionary<string, string>
        {
            { "X-Auto-Response-Suppress", "OOF, AutoReply" }
        };

        await _resend.EmailSendAsync(emailMessage);
    }
}