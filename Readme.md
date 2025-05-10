## M-Pesa Integration in ASP.NET Core MVC

This project demonstrates how to integrate **Safaricom M-Pesa Daraja API** into an **ASP.NET Core MVC** application with an interactive frontend using JavaScript.

---

###  Features

* 🔐 Register C2B Confirmation & Validation URLs  
* 💰 Simulate C2B Payments  
* 📲 Trigger M-Pesa Express (STK Push) via user-entered phone number  

---

### 📁 Project Structure

```
/M-Pesa-Integration/
│
├── Controllers/
│   └── MpesaController.cs           # Handles M-Pesa API requests
│
├── Data/
│   └── ApplicationDbContext.cs      
│
├── Models/
│   └── MpesaC2B.cs                  # Represents C2B data structure
│
├── Services/
│   └── MpesaSettings.cs             # Stores API credentials and endpoints
│
├── Views/
│   ├── Shared/
│   └── Home/
│       └── Index.cshtml             # Main frontend view
│   └── _ViewImports.cshtml
│   └── _ViewStart.cshtml
│
├── wwwroot/
│   └── js/                          # (Optional) Location for JavaScript files
│
├── appsettings.json                # Contains Safaricom API credentials
├── Program.cs                      # Entry point
├── mpesaintegration.csproj         # Project file
└── mpesaintegration.sln            # Solution file
```

---

### 🔧 Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/Lenonkoech/mpesaintergration.git
   cd mpesaintegration
   ```

2. **Configure credentials** in `appsettings.json`:

   ```json
   {
    "Mpesa": {
    "ConsumerKey": "YOUR_CONSUMER_KEY",
    "ConsumerSecret": "YOUR_CONSUMER_SECRET",
    "ShortCode": "your_short_code",
    "Msisdn": "your_Msisdn",
    "ValidationURL": "https://mydomain.com/validation",
    "ConfirmationURL": "https://mydomain.com/confirmation",
    "BillRefNumber": "null",
    "CommandID": "CustomerBuyGoodsOnline",
    "BusinessShortCode": "your_BS-ShortCode",
    "PassKey": "YOUR_LNM_PASSKEY_HERE",
    "CallbackURL": "https://mydomain.com/pat",
    "TransactionType": "CustomerPayBillOnline",
    "AccountReference": "Test",
    "TransactionDesc": "Test"
     }
   }
   ```

3. **Run the application**

   ```bash
   dotnet run
   ```

4. **Navigate to**

   ```
   https://localhost:{port}/Mpesa/Index
   ```

---

### Phone Number Input Logic

* Accepted formats: `07XXXXXXXX` or `01XXXXXXXX`  
* Frontend validation using regex  
* Converted to international format `2547XXXXXXXX` or `2541XXXXXXXX` before submission  

---

### 🧪 Endpoints

| Route                  | Method | Description                              |
|------------------------|--------|------------------------------------------|
| `/register-urls`       | GET    | Register confirmation & validation URLs |
| `/makePayment`         | POST   | Simulate a C2B payment                  |
| `/mpesaExpressPayment` | POST   | Trigger STK push to user phone          |

---

### 📚 Notes

* Use [Daraja Sandbox](https://developer.safaricom.co.ke/daraja/apis/post/safaricom-sandbox) credentials for testing.  
* To receive live callbacks, expose your localhost using tools like [Ngrok](https://ngrok.com).  

---