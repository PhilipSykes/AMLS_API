namespace Common.Notification.Email;

public static class EmailTemplates
{
    public static readonly string Reservation = """
                                                <!DOCTYPE html>
                                                <html>
                                                <head>
                                                    <style>
                                                        body {
                                                            font-family: Arial, sans-serif;
                                                        }
                                                    </style>
                                                </head>
                                                <body>
                                                
                                                <h1>Hello {UserName}!</h1>
                                                <h3>This is to confirm your booking of {Media}</h3>
                                                
                                                <p>You can pick up your item(s) at {Time} from {Location}</p>
                                                
                                                </body>
                                                </html>
                                                """;
        
    public static readonly string Login = """
                                          <!DOCTYPE html>
                                          <html>
                                          <head>
                                              <style>
                                                  body {
                                                      font-family: Arial, sans-serif;
                                                  }
                                              </style>
                                          </head>
                                          <body>
                                          
                                          <h1>Hello {UserName}!</h1>
                                          <h3>Account Login</h3>
                                          
                                          <p>This is to let you know that your account has been logged into at {time}</p>
                                          <p>If this wasn't you, please let us know.</p>
                                          
                                          </body>
                                          </html>
                                          """;
        
    public static readonly string Borrow = """
                                           <!DOCTYPE html>
                                           <html>
                                           <head>
                                               <style>
                                                   body {
                                                       font-family: Arial, sans-serif;
                                                   }
                                               </style>
                                           </head>
                                           <body>
                                           
                                           <h1>Hello {UserName}!</h1>
                                           <h3>This is to confirm your reservation of {Media}</h3>
                                           <p>Reservation start date:{startDate}</p>
                                           <p>Reservation end date:{endDate}</p>
                                           <p>You can pick up your item(s) at from {Location} on {startDate}</p>
                                           
                                           </body>
                                           </html>
                                           """;
}