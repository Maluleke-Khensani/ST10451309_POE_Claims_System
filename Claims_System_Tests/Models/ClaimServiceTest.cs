using Xunit;
using Claims_System.Models;
using Claims_System.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using Claims_System.Areas.Identity.Data;


namespace Claims_System_Tests.Models
{
    public class ClaimServiceTest
    { }
}
        // ✅ Helper method to create an in-memory SQLite DB
      
        // This simulates a real database without touching the actual DB
        
        /*
        
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Filename=:memory:") // in-memory SQLite
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.OpenConnection();  // must open connection for SQLite memory
            context.Database.EnsureCreated();   // make sure the table is created
            return context;
        }

        [Fact]
        public async Task CreateClaimAsync_Should_Add_Claim()
        {
            // Arrange: set up context and service
            var context = GetDbContext();
            var service = new ClaimService(context);

            // Arrange: create a sample claim to add
            var claim = new LecturerClaim
            {
                EmployeeNumber = 1001,
                Username = "lecturer1",
                FullName = "John Doe",
                ModuleName = "Maths 101",
                Month = 1,
                Year = 2025,
                Submitted = DateTime.Now,
                HoursWorked = 10,
                Rate = 500,
                CoordinatorStatus = "Pending",
                ManagerStatus = "Pending",
                Notes = "Test note"
            };

            // Act: try to add the claim via the service
            var result = await service.CreateClaimAsync(claim, null, null);

            // Assert: check that the claim was added successfully
            Assert.True(result); // service returned success
            Assert.Equal(1, context.LecturerClaims.Count()); // DB now has one claim
        }

        [Fact]
        public async Task DeleteClaimAsync_Should_Remove_Claim()
        {
            // Arrange: set up context and service
            var context = GetDbContext();
            var service = new ClaimService(context);

            // Arrange: add a claim to delete
            var claim = new LecturerClaim
            {
                EmployeeNumber = 2001,
                Username = "lecturer2",
                FullName = "Jane Doe",
                ModuleName = "Physics 201",
                Month = 2,
                Year = 2025,
                Submitted = DateTime.Now,
                HoursWorked = 8,
                Rate = 400,
                CoordinatorStatus = "Pending",
                ManagerStatus = "Pending",
                Notes = "Delete test"
            };

            context.LecturerClaims.Add(claim);
            await context.SaveChangesAsync(); // save to in-memory DB

            // Act: delete the claim via the service
            var result = await service.DeleteClaimAsync(claim.ClaimId);

            // Assert: claim was removed
            Assert.True(result); // service returned success
            Assert.Empty(context.LecturerClaims); // DB has no claims left
        }

        [Fact]
        public async Task GetAllClaimsAsync_Should_Return_Claims()
        {
            // Arrange: set up context and service
            var context = GetDbContext();
            var service = new ClaimService(context);

            // Arrange: add a claim to test retrieval
            context.LecturerClaims.Add(new LecturerClaim
            {
                EmployeeNumber = 3001,
                Username = "lecturer3",
                FullName = "Mark Smith",
                ModuleName = "Chemistry 301",
                Month = 3,
                Year = 2025,
                Submitted = DateTime.Now,
                HoursWorked = 12,
                Rate = 450,
                CoordinatorStatus = "Pending",
                ManagerStatus = "Pending",
                Notes = "List test"
            });

            await context.SaveChangesAsync();

            // Act: retrieve all claims via service
            var claims = await service.GetAllClaimsAsync();

            // Assert: check that we got the claim
            Assert.NotNull(claims); // should not be null
            Assert.Single(claims);   // should have exactly one claim
        }
    }
}
*/