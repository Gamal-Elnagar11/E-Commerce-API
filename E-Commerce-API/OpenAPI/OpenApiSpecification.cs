using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace E_Commerce_API.OpenAPI
{
    public class OpenApiSpecification : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
           // System.Diagnostics.Debug.WriteLine($"DEBUG: OperationId is: {operation.OperationId}");
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null)
            {
                var controllerName = descriptor.ControllerName;
                // عشان نشيل كلمة "Controller" من الاسم لو عايز
                var tagName = controllerName.Replace("Controller", "");
                operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = tagName } };
            }
            var docSettings = new Dictionary<string, (string Summary, string Description, int[] StatusCodes)>
        {
                // Account Controller
            { "Register", ("User Registration", "User Registration New User To Get Access Token", [200, 400,500]) },
            { "Login", ("Login", "Authenticates a User And Return JWT Token", [200,400,404, 500]) },
            { "AddRole", ("Add Role", "Assigne a Spasific Role a User To Admin", [200,400,401,404,403, 500]) },
            { "AllUsers", ("Get Users", "Get All Users In The System", [200,403,400,401,404, 500]) },
            { "DeleteUser", ("Delete User", "Delete User From System", [200,403,400,401,404, 500]) },

             // Cart Controller
            { "MyCart", ("Get My Cart", "Retrieves the contents of the current shopping cart", [200,401,404, 500]) },
            { "add-item-to-cart", ("Add Item To Cart", "Adds a specific product to the shopping cart", [200,400,401,404, 500]) },
            { "update-item-in-cart", ("Update Item", "Update a specific product to the shopping cart", [200,400,401,404, 500]) },
            { "remove-item-from-cart", ("Remove Item ", "Delete a specific product to the shopping cart", [200,400,401,404, 500]) },
            { "clear", ("Clear ALl Cart", "Clear All Products From Cart", [200,400,401,404, 500]) },

              
              // Category Controller
            { "Get All Category With Products", ("Get All Category With Products", "Get All Category With Products Every Category has More Products", [200,404, 500]) },
            { "Get All Category", (" Get All Category", "Display All Category From System", [200,404, 500]) },
            { "Search Category", ("Search Category", "Search By Any Charachter And Return All Contaic Char", [200,404,500]) },
            { "Search With Products", ("Search With Products", "Display All Category With Products Form System", [200,404, 500]) },
            { "AddCategory", ("AddCategory", "Display All Product that exist", [200,403,401, 500]) },
            { "UpdateCategory", ("Update Category", "Update Category By Id", [200,400,403,401,404, 500]) },
            { "DeleteCategory", ("Delete Category", "Delete Category By Id", [200,400,403,401,404, 500]) },
            { "GetById", ("Get Category By Id", "Get Category By Id", [200,400,404, 500]) },


            // FAQ Controller
            { "Quastions", ("Get All Quastions", "Get All Quations From System", [200,404, 500]) },
            { "Answer-ID", ("Get Answer By Id", "Get Spacific Answer By Id", [200,404, 500]) },
            { "AddQ", ("Add Quastion", "Add Quastion And Answer To System", [200,400,403, 500]) },
            { "UpdateQ", ("Update Quastion", "Update Quation And Answer", [200,400,403,404, 500]) },
            { "DeleteQ", ("Delete Quastion", "Delete Quastion And Answer From System", [200,400,403,404, 500]) },



              // Feedback Controller

            { "AddFB", ("Add Feedback", "Add Feedback to system", [200,400,401, 500]) },
            { "GetFB", ("Get All Feedback", "Get All Feedbacks From System", [200,404, 500]) },
            { "DeleteFB", ("Delete Feedback", "Delete Feedback By Id From System", [200,400,403,404,401, 500]) },




 


 
        };

            if (operation.OperationId != null && docSettings.TryGetValue(operation.OperationId, out var settings))
            {
                operation.Summary = settings.Summary;
                operation.Description = settings.Description;
                operation.Responses.Clear();
                foreach (var code in settings.StatusCodes)
                {
                    operation.Responses.Add(code.ToString(), new OpenApiResponse { Description = GetDescription(code) });
                }
            }

        }


        private string GetDescription(int code) => code switch
        {
            200 => " Successfuly",
            400 => " Bad Request",
            401 => "UnAuthorize",
            404 => "Not Found",
            _ => "Server Error"
        };
    }
}

