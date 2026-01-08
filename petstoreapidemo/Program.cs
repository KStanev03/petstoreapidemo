using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using petstoreapidemo;

namespace petstoreapidemo

{
    class Program
    {
        static async Task Main(string[] args)
        {
            var http = new HttpClient { BaseAddress = new Uri("https://petstore3.swagger.io/api/v3") };
            var client = new PetStoreClient(http);

            Console.WriteLine("--- PetStore Demo ---\n");

            await PetOperations(client);
            await StoreOperations(client);
            await UserOperations(client);

            Console.WriteLine("\nAll requests completed.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static async Task PetOperations(PetStoreClient client)
        {
            Console.WriteLine("=== PET METHODS ===");

            long testPetId = 10;

            // ADD NEW PET
            try
            {
                Console.WriteLine("[1/8] AddPetAsync - Adding a new pet");
                var newPet = new Pet
                {
                    Id = testPetId,
                    Name = "Dog1",
                    Status = PetStatus.Available,
                    Category = new Category { Id = 1, Name = "Dogs" },
                    PhotoUrls = new List<string> { "https://cdn.pixabay.com/photo/2016/02/19/15/46/labrador-retriever-1210559_960_720.jpg" },
                    Tags = new List<Tag> { new Tag { Id = 1, Name = "Tag1" }, new Tag { Id = 2, Name = "Tag2" } }
                };
                var addedPet = await client.AddPetAsync(newPet);
                Console.WriteLine($"Pet added successfully! ID: {addedPet.Id}, Name: {addedPet.Name}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"AddPetAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // GET PET BY ID
            Pet retrievedPet = null;
            try
            {
                Console.WriteLine("[2/8] GetPetByIdAsync - Retrieving pet by ID");
                retrievedPet = await client.GetPetByIdAsync(testPetId);
                Console.WriteLine($"Pet retrieved: ID={retrievedPet.Id}, Name={retrievedPet.Name}, Status={retrievedPet.Status}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"GetPetByIdAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // UPDATE PET
            try
            {
                if (retrievedPet != null)
                {
                    Console.WriteLine("[3/8] UpdatePetAsync - Updating pet information");
                    retrievedPet.Name = "NewDog1";
                    retrievedPet.Status = PetStatus.Pending;
                    var updatedPet = await client.UpdatePetAsync(retrievedPet);
                    Console.WriteLine($"Pet updated! New name: {updatedPet.Name}, New status: {updatedPet.Status}");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"UpdatePetAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // UPDATE PET WITH FORM
            try
            {
                Console.WriteLine("[4/8] UpdatePetWithFormAsync");
                var formUpdatedPet = await client.UpdatePetWithFormAsync(testPetId, "UpdatedNewDog1", "available");
                Console.WriteLine($"Pet updated with form! Name: {formUpdatedPet.Name}, Status: {formUpdatedPet.Status}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"UpdatePetWithFormAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // FIND PETS BY STATUS
            try
            {
                Console.WriteLine("\n[5/8] FindPetsByStatusAsync - Finding available pets");
                var availablePets = await client.FindPetsByStatusAsync(Status.Available);

                if (availablePets == null)
                {
                    Console.WriteLine("Received null response");
                }
                else if (availablePets.Count == 0)
                {
                    Console.WriteLine("Found 0 available pets");
                }
                else
                {
                    Console.WriteLine($"Found {availablePets.Count} available pets");
                    foreach (var pet in availablePets.Take(3))
                    {
                        Console.WriteLine($"  ID: {pet.Id}, Name: {pet.Name}, Tags: {pet.Tags?.Count ?? 0}");
                    }
                    if (availablePets.Count > 3)
                        Console.WriteLine($"  and {availablePets.Count - 3} more");
                }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"FindPetsByStatusAsync failed (HTTP {ex.StatusCode}): {ex.Message}");
                Console.WriteLine($"Response: {ex.Response?.Substring(0, Math.Min(200, ex.Response?.Length ?? 0))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FindPetsByStatusAsync failed: {ex.Message}");
            }
            await Task.Delay(1000);

            // FIND PETS BY TAGS
            try
            {
                Console.WriteLine("[6/8] FindPetsByTagsAsync - Finding pets by tags");
                var taggedPets = await client.FindPetsByTagsAsync(new[] { "Tag1", "Tag2" });
                Console.WriteLine($"Found {taggedPets.Count} pets with specified tags");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"FindPetsByTagsAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // UPLOAD FILE
            try
            {
                Console.WriteLine("[7/8] UploadFileAsync - Uploading pet images");
                using var imageStream = new MemoryStream(new byte[] { 0x89, 0x50, 0x4E, 0x47 });
                await client.UploadFileAsync(testPetId, "Dog photo", imageStream);
                Console.WriteLine("Image uploaded!");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"UploadFileAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // DELETE PET
            try
            {
                Console.WriteLine("[8/8] DeletePetAsync - Deleting the test pet");
                await client.DeletePetAsync(api_key: null, testPetId);
                Console.WriteLine("Pet deleted successfully!");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"DeletePetAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);
        }

        static async Task StoreOperations(PetStoreClient client)
        {
            Console.WriteLine("\n=== STORE METHODS ===");

            // GET INVENTORY
            try
            {
                Console.WriteLine("[1/4] GetInventoryAsync");
                var inventory = await client.GetInventoryAsync();
                Console.WriteLine($"Inventory retrieved! {inventory.Count} categories:");
                foreach (var item in inventory.Take(5))
                {
                    Console.WriteLine($" - {item.Key}: {item.Value}");
                }
                if (inventory.Count > 5)
                    Console.WriteLine($" and {inventory.Count - 5} more.");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"GetInventoryAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // PLACE ORDER
            long orderId = 0;
            try
            {
                Console.WriteLine("[2/4] PlaceOrderAsync");
                var newOrder = new Order
                {
                    Id = orderId,
                    PetId = 1,
                    Quantity = 2,
                    ShipDate = DateTimeOffset.Now.AddDays(3),
                    Status = OrderStatus.Placed,
                    Complete = false
                };
                var placedOrder = await client.PlaceOrderAsync(newOrder);
                orderId = placedOrder.Id;
                Console.WriteLine($"Order placed! ID: {placedOrder.Id}, Status: {placedOrder.Status}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"PlaceOrderAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // GET ORDER BY ID
          
                orderId = 1;
                try
                {
                    Console.WriteLine("[3/4] GetOrderByIdAsync");
                    var retrievedOrder = await client.GetOrderByIdAsync(orderId);
                    Console.WriteLine($"Order retrieved: ID={retrievedOrder.Id}, Quantity={retrievedOrder.Quantity}, Status={retrievedOrder.Status}");
                }
                catch (ApiException ex)
                {
                    Console.WriteLine($"GetOrderByIdAsync failed (Status {ex.StatusCode}): {ex.Message}");
                }
                await Task.Delay(500);

                // DELETE ORDER
                try
                {
                    Console.WriteLine("[4/4] DeleteOrderAsync");
                    await client.DeleteOrderAsync(orderId);
                    Console.WriteLine("Order deleted successfully!");
                }
                catch (ApiException ex)
                {
                    Console.WriteLine($"DeleteOrderAsync failed (Status {ex.StatusCode}): {ex.Message}");
                }
                await Task.Delay(500);
            
        }

        static async Task UserOperations(PetStoreClient client)
        {
            Console.WriteLine("\n=== USER METHODS ===");

            string testUsername = $"testuser_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}";

            // CREATE USER
            try
            {
                Console.WriteLine("[1/7] CreateUserAsync");
                var newUser = new User
                {
                    Id = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    Username = testUsername,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Password = "SecurePass123",
                    Phone = "+1234567890",
                    UserStatus = 1
                };
                var createdUser = await client.CreateUserAsync(newUser);
                Console.WriteLine($"User created: {createdUser.Username}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"CreateUserAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // CREATE MULTIPLE USERS
            try
            {
                Console.WriteLine("[2/7] CreateUsersWithListInputAsync");
                var users = new List<User>
            {
                new User { Id = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 1, Username = $"bulkuser1_{DateTime.Now.Ticks}", FirstName="Alice", LastName="Smith", Email="alice@example.com", Password="pass123", Phone="+1111111111", UserStatus=1 },
                new User { Id = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 2, Username = $"bulkuser2_{DateTime.Now.Ticks}", FirstName="Bob", LastName="Johnson", Email="bob@example.com", Password="pass456", Phone="+2222222222", UserStatus=1 }
            };
                await client.CreateUsersWithListInputAsync(users);
                Console.WriteLine("Multiple users created successfully!");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"CreateUsersWithListInputAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // LOGIN USER
            try
            {
                Console.WriteLine("[3/7] LoginUserAsync");
                var loginResult = await client.LoginUserAsync(testUsername, "SecurePass123");
                Console.WriteLine($"Login result: {loginResult?.Substring(0, Math.Min(50, loginResult?.Length ?? 0))}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"LoginUserAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // GET USER BY NAME
            User retrievedUser = null;
            try
            {
                Console.WriteLine("[4/7] GetUserByNameAsync");
                retrievedUser = await client.GetUserByNameAsync(testUsername);
                Console.WriteLine($"User retrieved: {retrievedUser.Username}");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"GetUserByNameAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // UPDATE USER
            try
            {
                Console.WriteLine("[5/7] UpdateUserAsync");
                if (retrievedUser != null)
                {
                    Console.WriteLine("[5/7] UpdateUserAsync");
                    retrievedUser.FirstName = "Jonathan";
                    retrievedUser.Email = "jonathan.doe@example.com";
                    await client.UpdateUserAsync(testUsername, retrievedUser);
                    Console.WriteLine($"User updated: {retrievedUser.FirstName}, {retrievedUser.Email}");
                }
                else { Console.WriteLine("Retrieved User = null"); }
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"UpdateUserAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // LOGOUT USER
            try
            {
                Console.WriteLine("[6/7] LogoutUserAsync");
                await client.LogoutUserAsync();
                Console.WriteLine("User logged out successfully!");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"LogoutUserAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);

            // DELETE USER
            try
            {
                Console.WriteLine("[7/7] DeleteUserAsync");
                await client.DeleteUserAsync(testUsername);
                Console.WriteLine("User deleted successfully!");
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"DeleteUserAsync failed (Status {ex.StatusCode}): {ex.Message}");
            }
            await Task.Delay(500);
        }
    }
}
