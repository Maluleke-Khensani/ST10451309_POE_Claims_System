using Claims_System.Controllers;
using Claims_System.Models;
using Claims_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims_System_Tests.Controllers
{
    public class LecturerControllerTests
    { }
}
        /*
        // Helper to create a controller instance with mocked session and service
        private LecturerController GetController(IClaimService service, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var controller = new LecturerController(service,userManager);

            // Set up fake HTTP context so we can simulate a session
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Custom in-memory session for testing
            controller.HttpContext.Session = new DummySession();
            controller.HttpContext.Session.SetString("Username", "lecturer1"); // simulate logged-in user

            return controller;
        }

        //ensures that when a lecturer opens their dashboard, they only see their own claims.”
        [Fact]
        public async Task Index_Returns_View_With_UserClaims()
        {
            // Using in-memory database to avoid messing with real DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("LecturerControllerDB1")
                .Options;
            var context = new ApplicationDbContext(options);

            // Sample claims: one for our user, one for someone else
            var claims = new List<LecturerClaim>
            {
                new LecturerClaim { ClaimId = 1, Username = "lecturer1", FullName = "John Doe", Submitted = System.DateTime.Now },
                new LecturerClaim { ClaimId = 2, Username = "other", FullName = "Jane" }
            };

            // Mock the service so it returns our sample claims
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetAllClaimsAsync()).ReturnsAsync(claims);

            // Get the controller with mocked session and service
            var controller = GetController(mockService.Object, context);

            // Call Index action
            var result = await controller.Index() as ViewResult;

            // Make sure the model is the type we expect
            var model = Assert.IsAssignableFrom<IEnumerable<LecturerClaim>>(result.Model);

            // Only claims for the logged-in user should appear
            Assert.Single(model);
        }


        //lecturer opens the claim form, the controller returns the correct view and a new LecturerClaim mode
        [Fact]
        public void ClaimForm_Returns_View_With_Model()
        {
            // Mock service for this test
            var mockService = new Mock<IClaimService>();

            // In-memory DB so we can pass a context to controller
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("LecturerControllerDB2")
                .Options;
            var controller = GetController(mockService.Object, new ApplicationDbContext(options));

            // Call ClaimForm action
            var result = controller.ClaimForm() as ViewResult;

            // Make sure we got a view back
            Assert.NotNull(result);

            // The model should be a new LecturerClaim instance
            Assert.IsType<LecturerClaim>(result.Model);
        }


        //The controller correctly returns NotFound when a claim with a given ID doesn’t exist.
        [Fact]
        public async Task ViewClaim_Returns_NotFound_If_Claim_Missing()
        {
            
            var mockService = new Mock<IClaimService>();           // Mock service to simulate missing claim
            mockService.Setup(s => s.GetClaimByIdAsync(It.IsAny<int>()))
                       .ReturnsAsync((LecturerClaim?)null);

            // In-memory DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("LecturerControllerDB3")
                .Options;
            var controller = GetController(mockService.Object, new ApplicationDbContext(options));

           
            var result = await controller.ViewClaim(99);      // Call ViewClaim with an ID that doesn't exist

            
            Assert.IsType<NotFoundResult>(result);  // Expect a NotFoundResult because the claim isn't in DB
        }
    }


    // Fake in-memory session for controller testing
    public class DummySession : ISession
    {
        // Store session values in a dictionary
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public IEnumerable<string> Keys => _sessionStorage.Keys;
        public string Id => "dummy";
        public bool IsAvailable => true;

        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
    }
}
*/