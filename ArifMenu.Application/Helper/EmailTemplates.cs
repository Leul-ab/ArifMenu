using System;

namespace ArifMenu.Application.Helper
{
    public static class EmailTemplates
    {
        public static string GenerateOtpHtml(string userName, string otp)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <title>ArifMenu OTP</title>
</head>
<body style='margin:0;padding:0;background-color:#f4f4f4;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;'>
  <div style='max-width:600px;margin:40px auto;background-color:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 0 10px rgba(0,0,0,0.1);'>
    <div style='background-color:#1a8e3b;padding:20px;text-align:left;color:#ffffff;'>
      <h1 style='margin:0;'>ArifMenu</h1>
    </div>

    <div style='padding:30px;'>
      <h3 style='margin-top:0;color:#333333;'>Hello {userName},</h3>
      <p style='color:#333;'>You recently requested to reset your password.</p>
      <p style='color:#333;'>Please use the following One-Time Password (OTP) to reset it:</p>

      <div style='margin:30px 0;padding:20px;background-color:#f8f8f8;border:1px solid #e0e0e0;border-radius:8px;text-align:center;'>
        <span style='font-size:28px;font-weight:bold;color:#1a8e3b;letter-spacing:4px;'>{otp}</span>
      </div>

      <p style='color:#555;'>This OTP is valid for <strong>10 minutes</strong>. Do not share it with anyone.</p>
    </div>

    <div style='text-align:center;padding:20px;font-size:12px;color:#999;background-color:#fafafa;border-top:1px solid #eee;'>
      <div>Arifpay Technology</div>
      <div><strong style='color:#333;'>Arifpay Financial Technologies</strong> — Built by and for Ethiopia 🇪🇹</div>
    </div>
  </div>
</body>
</html>";
        }

    }
}