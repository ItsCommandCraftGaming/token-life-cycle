using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenLifecycle.Application.UseCases.RegisterUser
{
    public class RegisterUserResponse
    {
        public string Message { get; set; }

        public string UserId { get; set; }

        public bool Ok { get; set; }

        public RegisterUserResponse()
        {
            
        }

        public RegisterUserResponse(string userId)
        {
            if (ObjectId.TryParse(userId, out var converted))
            {
                Message = "User created succesfully";
                UserId = userId;
                Ok = true;
            }
            else
            {
                Message = "Failed to save user";
                UserId = null;
                Ok = false;
            }
        }
    }
}
